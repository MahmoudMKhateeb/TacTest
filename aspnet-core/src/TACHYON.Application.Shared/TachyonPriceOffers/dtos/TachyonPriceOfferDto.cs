using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.TachyonPriceOffers.dtos
{
    public class TachyonPriceOfferDto : EntityDto
    {
        public virtual long ShippingRequestId { get; set; }
        public decimal OfferedPrice { get; set; }
        public long? ShippingRequestBidId { get; set; }
        public string OfferStatusName { get; set; }
        public string RejectedReason { get; set; }
        public string PriceTypeName { get; set; }
        public decimal ActualPercentCommission { get; set; }
        public decimal ActualCommissionValue { get; set; }
        public decimal ActualMinCommissionValue { get; set; }

        public decimal TotalCommission { get; set; }
        //to do place direct request ID
    }
}