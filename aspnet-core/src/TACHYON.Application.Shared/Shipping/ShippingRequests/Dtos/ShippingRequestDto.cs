
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestDto : EntityDto<long>
    {
        public decimal Vas { get; set; }

        public  bool IsBid { get; set; }
        public  bool IsTachyonDeal { get; set; }
        public decimal? Price { get; set; }
        public bool? IsPriceAccepted { get; set; }
        public bool? IsRejected { get; set; }
        public DateTime? StartTripDate { get; set; }
        public int NumberOfTrips { get; set; }


    }
}