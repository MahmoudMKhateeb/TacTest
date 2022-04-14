using Abp.Application.Services.Dto;
using System;
using TACHYON.Dto;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public class SrPostPriceUpdateListDto : EntityDto<long>
    {
        public SrPostPriceUpdateAction Action { get; set; }
        
        public string ActionTitle { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsApplied { get; set; }

        public long? PriceOfferId { get; set; }

        public SrPostPriceUpdateOfferStatus OfferStatus { get; set; }

        public string OfferStatusTitle { get; set; }

    }
}