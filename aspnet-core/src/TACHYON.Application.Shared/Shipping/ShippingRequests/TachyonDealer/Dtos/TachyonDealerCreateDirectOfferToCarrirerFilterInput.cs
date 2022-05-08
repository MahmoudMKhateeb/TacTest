using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
    public class TachyonDealerCreateDirectOfferToCarrirerFilterInput : PagedAndSortedResultRequestDto
    {
        public long? RequestId { get; set; }
    }
}