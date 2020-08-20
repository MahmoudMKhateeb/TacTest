
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestDto : EntityDto<long>
    {
        public decimal Vas { get; set; }


        public Guid? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }

        public long? GoodsDetailId { get; set; }

        public int? RouteId { get; set; }

        public  bool IsBid { get; set; }

        public  bool IsTachyonDeal { get; set; }

        public decimal? Price { get; set; }
        public bool? IsPriceAccepted { get; set; }
        public bool? IsRejected { get; set; }


    }
}