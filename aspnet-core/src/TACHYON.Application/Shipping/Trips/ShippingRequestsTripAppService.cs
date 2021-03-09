using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;

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

        public Task<ShippingRequestsTripListDto> GetAll(long RequestId)
        {
            throw new NotImplementedException();
        }

        public Task<ShippingRequestsTripForViewDto> GetForView(long id)
        {
            throw new NotImplementedException();
        }

        public Task CreateOrEdit(ShippingRequestsTripCreateOrEditDto input)
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


        private async void Create(ShippingRequestsTripCreateOrEditDto input)
        {
            ShippingRequestTrip trip = ObjectMapper.Map<ShippingRequestTrip>(input);

            await _ShippingRequestTripRepository.InsertAsync(trip);
        }
        private void Update(ShippingRequestsTripCreateOrEditDto input)
        {

        }
        public async Task Delete(EntityDto input)
        {
            await _ShippingRequestTripRepository.DeleteAsync(input.Id);
        }


    }
}
