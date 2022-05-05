using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.DataExporting;
using TACHYON.Dto;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Trucks;

namespace TACHYON.Waybills
{
    public class WaybillsAppService : TACHYONAppServiceBase
    {
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly ShippingRequestsAppService _shippingRequestAppService;
        private readonly GoodsDetailsAppService _goodsDetailsAppService;
        private readonly RoutPointsAppService _routPointAppService;
        private readonly IRepository<RoutPoint, long> _routPointReqpsitory;

        public WaybillsAppService(PdfExporterBase pdfExporterBase,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            ShippingRequestsAppService shippingRequestAppService,
                GoodsDetailsAppService goodsDetailsAppService,
            RoutPointsAppService routPointAppService, IRepository<RoutPoint, long> routPointReqpsitory)
        {
            _pdfExporterBase = pdfExporterBase;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestAppService = shippingRequestAppService;
            _goodsDetailsAppService = goodsDetailsAppService;
            _routPointAppService = routPointAppService;
            _routPointReqpsitory = routPointReqpsitory;
        }


        public async Task<FileDto> GetMultipleDropWaybillPdf(long RoutPointId)
        {
            DisableTenancyFilters();
            if (IsSingleDropShippingRequest(RoutPointId))
                throw new UserFriendlyException(L("Cannot download drop waybill for single drop shipping request"));

            var tripId = await _routPointReqpsitory.GetAll().Where(x => x.Id == RoutPointId).Select(x => x.ShippingRequestTripId).FirstOrDefaultAsync();
            return GetWaybillPdf(tripId, RoutPointId);
        }


        public FileDto GetSingleDropOrMasterWaybillPdf(int shippingRequestTripId)
        {
            DisableTenancyFilters();
            if (IsSingleDropShippingRequest(shippingRequestTripId))
            {
                return GetWaybillPdf(shippingRequestTripId);
            }
            else
            {
                return GetMasterWaybillPdf(shippingRequestTripId);
            }
        }

        #region Helper

        private bool IsSingleDropShippingRequest(int shippingRequestTripId)
        {
            var item = _shippingRequestTripRepository
                .GetAll()
                .Include(e => e.ShippingRequestFk)
                .FirstOrDefault(e => e.Id == shippingRequestTripId);
            return item.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.SingleDrop;
        }

        private bool IsSingleDropShippingRequest(long routPointId)
        {
            var item = _shippingRequestTripRepository
                .GetAll()
                .Include(e => e.ShippingRequestFk)
                .Include(e => e.RoutPoints)
                .FirstOrDefault(e => e.RoutPoints.Any(x => x.Id == routPointId));

            return item.ShippingRequestFk.RouteTypeId == ShippingRequestRouteType.SingleDrop;
        }

        private FileDto GetMasterWaybillPdf(int shippingRequestTripId)
        {
            DisableTenancyFilters();
            var reportPath = "/Waybills/Reports/Master_Waybill.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();
            var masterWaybill = _shippingRequestAppService.GetMasterWaybill(shippingRequestTripId);
            names.Add("DataSet1");
            data.Add(_shippingRequestAppService.GetMasterWaybill(shippingRequestTripId));

            names.Add("GetDropDetailsDS");
            data.Add(_routPointAppService.GetDropsDetailsForMasterWaybill(shippingRequestTripId));

            names.Add("SingleDropVasDataSet");
            data.Add(_shippingRequestAppService.GetShippingRequestVasesForSingleDropWaybill(shippingRequestTripId));
            var number = masterWaybill.FirstOrDefault()?.MasterWaybillNo.ToString();
            return _pdfExporterBase.CreateRdlcPdfPackageFromList(number, reportPath, names, data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shippingRequestTripId"></param>
        /// <returns></returns>
        private FileDto GetWaybillPdf(int shippingRequestTripId, long? dropOffId = null)
        {
            DisableTenancyFilters();
            var reportPath = "/Waybills/Reports/Single_Drop_Waybill.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            var dropOff = _shippingRequestAppService.GetDropWaybill(shippingRequestTripId, dropOffId);
            names.Add("SingleDropDataSet");
            data.Add(dropOff);

            names.Add("SingleDropGoodsDetailsDataSet");
            data.Add(_goodsDetailsAppService.GetShippingrequestGoodsDetailsForSingleDropWaybill(shippingRequestTripId, dropOffId));

            names.Add("SingleDropVasDataSet");
            data.Add(_shippingRequestAppService.GetShippingRequestVasesForSingleDropWaybill(shippingRequestTripId));

            var number = dropOffId.HasValue ? dropOff.FirstOrDefault()?.WaybillNumber?.ToString() : dropOff.FirstOrDefault()?.MasterWaybillNo.ToString();
            return _pdfExporterBase.CreateRdlcPdfPackageFromList(number, reportPath, names, data);
        }

        private string GetTripWaybilNo(int? tripId, long? pointId)
        {
            var item = _shippingRequestTripRepository
               .GetAll()
               .Include(x => x.RoutPoints)
               .WhereIf(tripId != null, e => e.Id == tripId)
               .WhereIf(pointId != null, e => e.RoutPoints.Any(x => x.Id == pointId))
               .FirstOrDefault();

            return item.WaybillNumber?.ToString();
        }

        #endregion
    }
}