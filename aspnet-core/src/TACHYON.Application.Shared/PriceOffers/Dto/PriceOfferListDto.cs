using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PriceOffers.Dto
{
    public class PriceOfferListDto:EntityDto<long>
    {
        public long ShippingRequestId { get; set; }
        public string Name { get; set; }
        public PriceOfferStatus Status { get; set; }
        public PriceOfferType PriceType { get; set; }
        public PriceOfferChannel Channel { get; set; }
        public decimal TotalAmount { get; set; }
        public  DateTime CreationTime { get; set; }
        public string StatusTitle { get; set; }
        public string PriceTypeTitle { get { return PriceType.GetEnumDescription(); } }

        public string ChannelTitle { get { return Channel.GetEnumDescription(); } }


    }
}
