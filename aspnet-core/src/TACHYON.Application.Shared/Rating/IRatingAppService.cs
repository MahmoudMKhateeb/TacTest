using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Rating.dtos;

namespace TACHYON.Rating
{
    public interface IRatingAppService
    {
        Task CreateCarrierRatingByShipper(CreateCarrierRatingByShipperDto input);
        Task CreateDriverAndDERatingByReceiver(CreateDriverAndDERatingByReceiverDto input);
        //Task CreateDeliveryExpRatingByReceiver(CreateDeliveryExpRateByReceiverDto input);
        Task CreateFacilityRatingByDriver(CreateFacilityRateByDriverDto input);
        Task CreateShippingExpRatingByDriver(CreateShippingExpRateByDriverDto input);
        Task CreateShipperRatingByCarrier(CreateShipperRateByCarrierDto input);
    }
}
