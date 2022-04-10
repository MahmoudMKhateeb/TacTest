using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public class ViewSrPostPriceUpdateDto : EntityDto<long>
    {
        public SrPostPriceUpdateAction Action { get; set; }
        
        public string ActionTitle { get; set; }

        public string RejectionReason { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsApplied { get; set; }
        
        public long? PriceOfferId { get; set; }

        public SrPostPriceUpdateOfferStatus OfferStatus { get; set; }

        public List<SrUpdateChangeItem> Changes { get; set; }
    }
}