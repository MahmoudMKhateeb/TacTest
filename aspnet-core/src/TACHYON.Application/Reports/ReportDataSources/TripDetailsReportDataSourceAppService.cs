using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;

namespace TACHYON.Reports.ReportDataSources
{

    [DontWrapResult(WrapOnSuccess = false,WrapOnError = true)]
    [AbpAuthorize] // todo Add Permission Here
    public class TripDetailsReportDataSourceAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _tripRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public TripDetailsReportDataSourceAppService(IRepository<ShippingRequestTrip> tripRepository,IRepository<ShippingRequest, long> shippingRequestRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _tripRepository = tripRepository;
        }
        
        
        public async Task<IEnumerable<TripDetailsItem>> GetAll(string reportUrl)
        {
            DisableTenancyFilters();
            var tripDetailItems = (from trip in _tripRepository.GetAll().OrderByDescending(x=> x.CreationTime)
                join shippingRequest in _shippingRequestRepository.GetAll() on trip.ShippingRequestId equals shippingRequest.Id
                select new TripDetailsItem
                {
                    ShipperName = shippingRequest.Tenant.Name,
                    CarrierName = shippingRequest.CarrierTenantFk.Name,
                    ActualDeliveryDate = trip.ActualDeliveryDate,
                    ActualPickupDate = trip.ActualPickupDate,
                    DestinationCity = trip.DestinationFacilityFk.CityFk.DisplayName,
                    DriverName = $"{trip.AssignedDriverUserFk.Name} {trip.AssignedDriverUserFk.Surname}",
                    GoodsCategory = trip.ShippingRequestId.HasValue
                        ? shippingRequest.GoodCategoryFk.Key
                        : trip.GoodCategoryFk.Key,
                    TruckType = trip.ShippingRequestFk.TrucksTypeFk.Key,
                    TruckPlateNumber = trip.AssignedTruckFk.PlateNumber,
                    RouteType = trip.ShippingRequestId.HasValue ? LocalizationSource.GetString(trip.ShippingRequestFk.RouteTypeId.ToString()) :
                         LocalizationSource.GetString(trip.RouteType.ToString()),
                    SubWaybills = trip.RoutPoints != null ? trip.RoutPoints.Select(x=> x.WaybillNumber).ToList() : new List<long?>(),
                    WaybillNumber = trip.WaybillNumber,
                    ShippingType = trip.ShippingRequestId.HasValue? trip.ShippingRequestFk.ShippingTypeId.ToString() : trip.ShippingRequestFk.ToString()
                });

            return await tripDetailItems.Take(25).ToListAsync();
        }
    }

}