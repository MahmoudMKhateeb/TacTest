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
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public long ShippingRequestId { get; set; }

    }
}
