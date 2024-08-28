using Abp.Domain.Repositories;
using Abp.Json;
using Abp.Webhooks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TACHYON.Actors.Dtos;
using TACHYON.Authorization.Users.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Trucks.Dtos;

namespace TACHYON.WebHooks
{
    public class AppWebhookPublisher : TACHYONDomainServiceBase, IAppWebhookPublisher
    {
        private readonly IWebhookPublisher _webHookPublisher;
        
        private readonly IRepository<ShippingRequestTrip> _tripRepository;

        public AppWebhookPublisher(IWebhookPublisher webHookPublisher, IRepository<ShippingRequestTrip> tripRepository)
        {
            _webHookPublisher = webHookPublisher;
            _tripRepository = tripRepository;
        }

        public async Task PublishTestWebhook()
        {
            var separator = DateTime.Now.Millisecond;
            await _webHookPublisher.PublishAsync(AppWebHookNames.TestWebhook,
                new { UserName = "Test Name " + separator, EmailAddress = "Test Email " + separator }
            );
        }
        
        public async Task PublishNewTripCreatedWebhook(CreateOrEditShippingRequestTripDto dto)
        {
            await _webHookPublisher.PublishAsync(AppWebHookNames.NewTripCreated, dto.ToJsonString());
        }
        
        public async Task PublishDeliveredTripUpdatedWebhook(int  tripId)
        {
            var trip = await _tripRepository.GetAll()
                            .Include(x => x.ActorShipperPrice)
                            .Include(x=> x.ActorShipperPrice)
                            .FirstOrDefaultAsync(x=> x.Id == tripId);
            
            var json = trip.ToJsonString();
            
            await _webHookPublisher.PublishAsync(AppWebHookNames.DeliveredTripUpdated, json);
        }

        public async Task PublishNewActorCreatedWebhook(CreateOrEditActorDto dto)
        {
            await _webHookPublisher.PublishAsync(AppWebHookNames.NewActorCreated, dto.ToJsonString());
        }

        public async Task PublishNewDriverCreatedWebhook(UserEditDto dto){
            await _webHookPublisher.PublishAsync(AppWebHookNames.DriverCreated, dto.ToJsonString());
        }
        
        public async Task PublishDriverUpdatedWebhook(UserEditDto dto){
            await _webHookPublisher.PublishAsync(AppWebHookNames.DriverUpdated, dto.ToJsonString());
        }


         public async Task PublishNewTruckCreatedWebhook(CreateOrEditTruckDto dto){
            await _webHookPublisher.PublishAsync(AppWebHookNames.TruckCreated, dto.ToJsonString());
        }
        
        public async Task PublishTruckUpdatedWebhook(CreateOrEditTruckDto dto){
            await _webHookPublisher.PublishAsync(AppWebHookNames.TruckUpdated, dto.ToJsonString());
        }


    }
}