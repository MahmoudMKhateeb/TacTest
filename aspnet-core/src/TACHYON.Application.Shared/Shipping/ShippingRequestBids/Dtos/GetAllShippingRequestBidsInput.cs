using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class GetAllShippingRequestBidsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? ShipperId { get; set; }
        public int? CarrierId { get; set; }
    }
}
