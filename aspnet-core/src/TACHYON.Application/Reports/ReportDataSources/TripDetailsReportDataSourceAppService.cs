using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Reports.ReportModels;
using TACHYON.Reports.ReportParameters;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Reports.ExtensionMethods;

namespace TACHYON.Reports.ReportDataSources
{
    [DontWrapResult(WrapOnSuccess = false, WrapOnError = true)]
    [AbpAuthorize]
    [RequiresFeature(AppFeatures.TripDetailsReport)]
    public class TripDetailsReportDataSourceAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _tripRepository;
        private readonly IReportParameterManager _parameterManager;

        public TripDetailsReportDataSourceAppService(
            IRepository<ShippingRequestTrip> tripRepository,
            IReportParameterManager parameterManager)
        {
            _parameterManager = parameterManager;
            _tripRepository = tripRepository;
        }


        public async Task<IEnumerable<TripDetailsItem>> GetAll(Guid reportId)
        {

            var parameterList = await _parameterManager.GetReportParameters(reportId);
            bool isShipper = await IsShipper();
            DisableTenancyFilters();
            
            var tripDetailItems = _tripRepository.GetAll().AsNoTracking()
                .Where(x => x.ShippingRequestId.HasValue
                    ? (x.ShippingRequestFk.TenantId == AbpSession.TenantId ||
                       x.ShippingRequestFk.CarrierTenantId == AbpSession.TenantId)
                    : (x.CarrierTenantId == AbpSession.TenantId || x.ShipperTenantId == AbpSession.TenantId))
                .ApplyReportParameters(parameterList)
                .Select(trip => new TripDetailsItem
                {
                    ShipperName =
                        trip.ShippingRequestId.HasValue
                            ? trip.ShippingRequestFk.Tenant.Name
                            : trip.ShipperTenantFk.Name,
                    CarrierName = trip.ShippingRequestId.HasValue
                        ? (trip.ShippingRequestFk.CarrierTenantId.HasValue
                            ? trip.ShippingRequestFk.CarrierTenantFk.Name
                            : "__")
                        : (trip.CarrierTenantId.HasValue ? trip.CarrierTenantFk.Name : "__"),
                    ActualDeliveryDate =
                        trip.ActualDeliveryDate.HasValue ? trip.ActualDeliveryDate.Value.ToString("dd/MM/yyyy") : "__",
                    ActualPickupDate =
                        trip.ActualPickupDate.HasValue ? trip.ActualPickupDate.Value.ToString("dd/MM/yyyy") : "__",
                    DestinationCity =
                        trip.DestinationFacilityId.HasValue ? trip.DestinationFacilityFk.CityFk.DisplayName : "__",
                    DriverName =
                        trip.AssignedDriverUserId.HasValue
                            ? $"{trip.AssignedDriverUserFk.Name} {trip.AssignedDriverUserFk.Surname}"
                            : "__",
                    GoodsCategory = trip.ShippingRequestId.HasValue
                        ? trip.ShippingRequestFk.GoodCategoryFk.Key
                        : trip.GoodCategoryFk.Key,
                    TruckType = trip.ShippingRequestId.HasValue && trip.ShippingRequestFk.TrucksTypeId.HasValue
                        ? trip.ShippingRequestFk.TrucksTypeFk.Key
                        : (trip.AssignedTruckId.HasValue ? trip.AssignedTruckFk.TrucksTypeFk.Key : "__"),
                    TruckPlateNumber = trip.AssignedTruckId.HasValue ? trip.AssignedTruckFk.PlateNumber : "__",
                    RouteType =
                        trip.ShippingRequestId.HasValue
                            ? LocalizationSource.GetString(trip.ShippingRequestFk.RouteTypeId.ToString())
                            : LocalizationSource.GetString(trip.RouteType.ToString()),
                    SubWaybills = trip.RoutPoints != null
                        ? string.Join(',', trip.RoutPoints.Select(x => x.WaybillNumber).ToList())
                        : "__",
                    WaybillNumber = trip.WaybillNumber,
                    ShippingType = trip.ShippingRequestId.HasValue
                        ? trip.ShippingRequestFk.ShippingTypeId.ToString()
                        : trip.ShippingTypeId.ToString(),
                    OriginCity = trip.OriginCityId.HasValue ? trip.OriginCityFk.DisplayName : "__",
                    TripStatus = LocalizationSource.GetString(trip.Status.ToString()),
                    NumberOfDrops =
                        trip.ShippingRequestId.HasValue ? trip.ShippingRequestFk.NumberOfDrops : trip.NumberOfDrops,
                    InvoiceStatus = LocalizationSource.GetString(isShipper ? (trip.IsShipperHaveInvoice ? "InvoiceIssued" : "InvoiceNotIssued") : (trip.IsCarrierHaveInvoice ? "InvoiceIssued" : "InvoiceNotIssued")),
                    RequestReferenceNumber =
                        trip.ShippingRequestId.HasValue
                            ? trip.ShippingRequestFk.ReferenceNumber
                            : LocalizationSource.GetString("DirectTrip"),
                    ExpectedDeliveryDate =
                        trip.ExpectedDeliveryTime.HasValue
                            ? trip.ExpectedDeliveryTime.Value.ToString("dd/MM/yyyy")
                            : "__",
                    ExpectedPickupDate = trip.StartTripDate.ToString("dd/MM/yyyy"),
                });
            return await tripDetailItems.ToListAsync();
        }
    }
}