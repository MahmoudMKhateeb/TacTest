using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetAllShippingRequestsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public decimal? MaxVasFilter { get; set; }
        public decimal? MinVasFilter { get; set; }

        public  bool? IsBid { get; set; }

        public  bool? IsTachyonDeal { get; set; }


    }
}