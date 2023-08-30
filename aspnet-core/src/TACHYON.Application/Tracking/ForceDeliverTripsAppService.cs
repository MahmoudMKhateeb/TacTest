using Abp;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Importing;
using TACHYON.Shipping.Trips.Importing.Dto;
using TACHYON.Storage;
using TACHYON.Tracking.Dto;

namespace TACHYON.Tracking
{
    public class ForceDeliverTripsAppService: TACHYONAppServiceBase, IForceDeliverTripsAppService
    {
        private readonly ForceDeliverTripExcelDataReader _excelDataReader;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IBackgroundJobManager BackgroundJobManager;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<RoutPoint, long> _pointRepository;

        public ForceDeliverTripsAppService(ForceDeliverTripExcelDataReader excelDataReader, IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager, IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<RoutPoint, long> pointRepository)
        {
            _excelDataReader = excelDataReader;
            _binaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _pointRepository = pointRepository;
        }

        public async Task<List<ImportTripTransactionFromExcelDto>> ReadForceDeliverTripsFromExcel(ForceDeliverTripInput args)
        {
            var binaryObject = await _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId);
            UserIdentifier userIdentifier = new UserIdentifier(args.RequestedByTenantId,
                args.RequestedByUserId);
            if (binaryObject == null)
            {
                throw new Abp.UI.UserFriendlyException(L("NotFoundOrDeletedFileError"));
            }

            List<ImportTripTransactionFromExcelDto> importedTripDeliveryDetails =
                _excelDataReader.GetTripDeliveryDetails(binaryObject.Bytes).ToList();

            // check waybill in DB

            var waybillsList = importedTripDeliveryDetails.Select(x => x.WaybillNumber);

            var allWaybillsList  =_shippingRequestTripRepository.GetAll()
                .WhereIf(!await IsTachyonDealer(), x => x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                .Select(x => x.WaybillNumber);

            var allTripsIQ = _shippingRequestTripRepository.GetAll();
            foreach (var item in importedTripDeliveryDetails)
            {
                var waybill = item.WaybillNumber;
                var isMasterWaybill =await allTripsIQ.AnyAsync(x => x.WaybillNumber == waybill);
                var isSubWaybill = await allTripsIQ.AnyAsync(x => x.RoutPoints.Any(y => y.WaybillNumber == waybill));

                if (!isMasterWaybill && !isSubWaybill)
                {
                    item.Exception += "InCorrect waybill No; ";
                    continue;
                }
                if (isMasterWaybill)
                {
                    if (!await allTripsIQ.Where(x => x.WaybillNumber == waybill)
                    .AnyAsync(x => x.ShippingRequestFk.RouteTypeId == Shipping.ShippingRequests.ShippingRequestRouteType.SingleDrop ||
                    x.RouteType == Shipping.ShippingRequests.ShippingRequestRouteType.SingleDrop))
                    {
                        item.Exception += "Master waybill must be for single drop trip; ";
                        continue;
                    }
                    if (!await allTripsIQ.Where(x => x.WaybillNumber == waybill)
                    .AnyAsync(x => x.Status == ShippingRequestTripStatus.New))
                    {
                        item.Exception += "Status of trip must be New; ";
                    }
                    var driver = await allTripsIQ.Where(x => x.WaybillNumber == waybill)
                    .AnyAsync(x => x.AssignedDriverUserId != null);
                    if (!driver)
                    {
                        item.Exception += "Driver not assigned; ";
                    }
                }
                if(isSubWaybill)
                {
                    var pointIQ = _pointRepository.GetAll().Where(x => x.WaybillNumber == waybill);
                    if (!await pointIQ.AnyAsync(x => x.Status == RoutePointStatus.StandBy))
                    {
                        item.Exception += "Status of SubWaybill must be Standby; ";
                    }

                    if (!await pointIQ.AnyAsync(x => x.ShippingRequestTripFk.AssignedDriverUserId != null))
                    {
                        item.Exception += "Driver not assigned; ";
                    }

                    await ValidatePortMovementPickupStatus(item, pointIQ);
                }
            }

            await _binaryObjectManager.DeleteAsync(args.BinaryObjectId);
            return importedTripDeliveryDetails;
        }

        public async Task ForceDeliverTripFromDto(List<ImportTripTransactionFromExcelDto> importedTripDeliveryDetails)
        {
            await BackgroundJobManager.EnqueueAsync<ForceDeliverTripJob, ForceDeliverTripJobArgs>(new ForceDeliverTripJobArgs {
                userIdentifier = new UserIdentifier(AbpSession.TenantId, AbpSession.UserId.Value),
                importedTripDeliveryDetails = importedTripDeliveryDetails });
        }


        #region Helper

        private async Task ValidatePortMovementPickupStatus(ImportTripTransactionFromExcelDto item, IQueryable<RoutPoint> pointIQ)
        {
            var isPortMovement = await pointIQ
                .AnyAsync(x => x.ShippingRequestTripFk.ShippingTypeId == Shipping.ShippingRequests.ShippingTypeEnum.ImportPortMovements ||
                x.ShippingRequestTripFk.ShippingTypeId == Shipping.ShippingRequests.ShippingTypeEnum.ExportPortMovements ||
                x.ShippingRequestTripFk.ShippingRequestFk.ShippingTypeId == Shipping.ShippingRequests.ShippingTypeEnum.ImportPortMovements ||
                x.ShippingRequestTripFk.ShippingRequestFk.ShippingTypeId == Shipping.ShippingRequests.ShippingTypeEnum.ExportPortMovements);

            if (isPortMovement)
            {
                var point = await pointIQ.Select(x => new { x.PointOrder, MasterWaybill = x.ShippingRequestTripFk.WaybillNumber }).FirstOrDefaultAsync();
                var isPickupPointStandByOrCompleted = await _pointRepository.GetAll().Where(x => x.ShippingRequestTripFk.WaybillNumber == point.MasterWaybill
                && x.PickingType == PickingType.Pickup && x.PointOrder == point.PointOrder - 1).AnyAsync(x => x.Status == RoutePointStatus.StandBy || x.IsComplete);

                if (!isPickupPointStandByOrCompleted)
                {
                    item.Exception += "Status of pickup point for this SubWaybill must be Standby or completed; ";
                }

            }
        }
        #endregion
    }
}
