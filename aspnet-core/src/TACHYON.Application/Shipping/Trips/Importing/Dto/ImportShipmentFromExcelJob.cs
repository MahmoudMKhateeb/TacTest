using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.ObjectMapping;
using Abp.Threading;
using Abp.UI;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Storage;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    public class ImportShipmentFromExcelJob : BackgroundJob<ImportShipmentFromExcelJobArgs>, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IShipmentListExcelDataReader _shipmentListExcelDataReader;
        private readonly IAppNotifier _appNotifier;
        private readonly IObjectMapper _objectMapper;
        private readonly ShippingRequestTripManager _shippingRequestTripManager;
        private readonly IInvalidShipmentExporter _invalidShipmentExporter;




        public ImportShipmentFromExcelJob(IUnitOfWorkManager unitOfWorkManager, IBinaryObjectManager binaryObjectManager, 
            IShipmentListExcelDataReader shipmentListExcelDataReader, IAppNotifier appNotifier, IObjectMapper objectMapper,
            ShippingRequestTripManager shippingRequestTripManager, IInvalidShipmentExporter invalidShipmentExporter)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _binaryObjectManager = binaryObjectManager;
            _shipmentListExcelDataReader = shipmentListExcelDataReader;
            _appNotifier = appNotifier;
            _objectMapper = objectMapper;
            _shippingRequestTripManager = shippingRequestTripManager;
            _invalidShipmentExporter = invalidShipmentExporter;
        }

        [AutomaticRetry(Attempts = 1)]
        public override void Execute(ImportShipmentFromExcelJobArgs args)
        {
            var trips=GetShipmentListFromExcelOrNull(args);
           
            if (trips == null || !trips.Any())
            {
                SendInvalidExcelNotification(args);
                return;
            }

            CreateShipments(args, trips);

        }

        private void CreateShipments(ImportShipmentFromExcelJobArgs args, List<ImportTripDto> Trips)
        {
            var invalidTrips = new List<ImportTripDto>();

            foreach (var trip in Trips)
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                    {
                        if (trip.CanBeImported())
                        {
                            try
                            {
                                AsyncHelper.RunSync(() => CreateShipmentAsync(trip));
                            }
                            catch (UserFriendlyException exception)
                            {
                                trip.Exception = exception.Message;
                                invalidTrips.Add(trip);
                            }
                            catch (Exception exception)
                            {
                                trip.Exception = exception.ToString();
                                invalidTrips.Add(trip);
                            }
                        }
                        else
                        {
                            invalidTrips.Add(trip);
                        }
                    }

                    uow.Complete();
                }
            }


            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    AsyncHelper.RunSync(() => ProcessImportShipmentsResultAsync(args, invalidTrips));
                }

                uow.Complete();
            }
        }

        private async Task ProcessImportShipmentsResultAsync(ImportShipmentFromExcelJobArgs args,
            List<ImportTripDto> invalidShipments)
        {
            if (invalidShipments.Any())
            {
                var file = _invalidShipmentExporter.ExportToFile(invalidShipments);
                await _appNotifier.SomeShipmentsCouldntBeImported(args.User, file.FileToken, file.FileType, file.FileName);
            }
            else
            {
                await _appNotifier.SendMessageAsync(
                    args.User,
                    new LocalizableString("AllShipmentssSuccessfullyImportedFromExcel",
                        TACHYONConsts.LocalizationSourceName),
                    null,
                    Abp.Notifications.NotificationSeverity.Success);
            }
        }


        private async Task CreateShipmentAsync(ImportTripDto input)
        {

            var trip = _objectMapper.Map<ShippingRequestTrip>(input);

            await _shippingRequestTripManager.CreateAsync(trip);
        }

        private List<ImportTripDto> GetShipmentListFromExcelOrNull(ImportShipmentFromExcelJobArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    try
                    {
                        var file = AsyncHelper.RunSync(() => _binaryObjectManager.GetOrNullAsync(args.BinaryObjectId));
                        return _shipmentListExcelDataReader.GetShipmentsFromExcel(file.Bytes,args.ShippingRequestId);
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        uow.Complete();
                    }
                }
            }
        }

        private void SendInvalidExcelNotification(ImportShipmentFromExcelJobArgs args)
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (CurrentUnitOfWork.SetTenantId(args.TenantId))
                {
                    AsyncHelper.RunSync(() => _appNotifier.SendMessageAsync(
                        args.User,
                        new LocalizableString("FileCantBeConvertedToShipmentList", TACHYONConsts.LocalizationSourceName),
                        null,
                        Abp.Notifications.NotificationSeverity.Warn));
                }

                uow.Complete();
            }
        }
    }
}
