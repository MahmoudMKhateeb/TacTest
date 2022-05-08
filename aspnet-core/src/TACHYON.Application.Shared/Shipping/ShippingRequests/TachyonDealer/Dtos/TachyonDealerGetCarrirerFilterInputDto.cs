using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos
{
    public class TachyonDealerGetCarrirerFilterInputDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public long RequestId { get; set; }
    }
}