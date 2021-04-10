using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
   public class TachyonDealerCreateDirectOfferToCarrirerInuptDto : EntityDto<long>
    {
        public int TenantId { get; set; }
    }
}
