using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.PriceOffers;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetShippingRequestForPricingInput : EntityDto<long>
    {
        public PriceOfferChannel? Channel { get; set; }
    }
}
