
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestDto : EntityDto<long>
    {
        //public decimal Vas { get; set; }

        public  bool IsBid { get; set; }
        public  bool IsTachyonDeal { get; set; }
        public decimal? Price { get; set; }
        public bool? IsPriceAccepted { get; set; }
        public bool? IsRejected { get; set; }
        public DateTime? StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }

        public int NumberOfTrips { get; set; }
        public int NumberOfDrops { get; set; }
        public bool StageOneFinish { get; set; }
        public bool StageTowFinish { get; set; }
        public bool StageThreeFinish { get; set; }
        public int GoodCategoryId { get; set; }
        public string PackingType { get; set; }
        public int NumberOfPacking { get; set; }
        public double TotalWeight { get; set; }
        public int ShippingTypeId { get; set; }

        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }


    }
}