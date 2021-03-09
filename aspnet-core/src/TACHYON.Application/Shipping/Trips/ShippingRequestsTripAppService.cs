using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.AddressBook.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Trucks.Dtos;

namespace TACHYON.Shipping.Trips
{
    public class ShippingRequestsTripAppService : TACHYONAppServiceBase, IShippingRequestsTripAppService
    {
        private readonly IRepository<ShippingRequestTrip> _ShippingRequestTripRepository;
        private readonly IRepository<ShippingRequest,long> _ShippingRequestRepository;


        public ShippingRequestsTripAppService(
            IRepository<ShippingRequestTrip> ShippingRequestTripRepository,
            IRepository<ShippingRequest, long> ShippingRequestRepository)
        {
            _ShippingRequestTripRepository = ShippingRequestTripRepository;
            _ShippingRequestRepository = ShippingRequestRepository;
        }

        public Task<ShippingRequestTripDto> GetAll(long ShippingRequestId)
        {
            throw new NotImplementedException();
        }

        public async Task<ShippingRequestTripForViewDto> GetShippingRequestTripForView(int id)
        {
            var shippingRequestTrip =await _ShippingRequestTripRepository.GetAll()
                .Include(x=>x.OriginFacilityFk)
                .Include(x=>x.DestinationFacilityFk)
                .Include(x=>x.AssignedDriverUserFk)
                .Include(x=>x.AssignedTruckFk)
                .ThenInclude(x=>x.TruckStatusFk)
                .Include(x => x.AssignedTruckFk)
                .ThenInclude(x => x.TrucksTypeFk)
                .Where(x=>x.Id==id)
                .FirstOrDefaultAsync();

            var output = new ShippingRequestTripForViewDto
            {
                shippingRequestTripDto = ObjectMapper.Map<ShippingRequestTripDto>(shippingRequestTrip),
                AssignedDriverDisplayName = shippingRequestTrip.AssignedDriverUserFk?.Name,
                AssignedTruckDto = new GetTruckForViewOutput
                {
                    Truck = ObjectMapper.Map<TruckDto>(shippingRequestTrip.AssignedTruckFk),
                    TruckStatusDisplayName = shippingRequestTrip.AssignedTruckFk?.TruckStatusFk.DisplayName,
                    TrucksTypeDisplayName = shippingRequestTrip.AssignedTruckFk?.TransportTypeFk.DisplayName
                },
                OriginalFacilityDto = ObjectMapper.Map<FacilityDto>(shippingRequestTrip.OriginFacilityFk),
                DestinationFacilityDto = ObjectMapper.Map<FacilityDto>(shippingRequestTrip.DestinationFacilityFk),
                TripStatus = (ShippingRequestTripStatus)shippingRequestTrip.StatusId
            };
            return output;
        }

        public Task CreateOrEdit(CreateOrEditShippingRequestTripDto input)
        {
           if (input.Id==0)
            {
                Create(input);
            }
           else
            {
                Update(input);
            }

           return Task.CompletedTask;
        }

        public async Task Delete(EntityDto input)
        {
            await _ShippingRequestTripRepository.DeleteAsync(input.Id);
        }
        private async void Create(CreateOrEditShippingRequestTripDto input)
        {
            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);

            await _ShippingRequestTripRepository.InsertAsync(trip);
        }
        private async void Update(CreateOrEditShippingRequestTripDto input)
        {

        }
       


    }
}
