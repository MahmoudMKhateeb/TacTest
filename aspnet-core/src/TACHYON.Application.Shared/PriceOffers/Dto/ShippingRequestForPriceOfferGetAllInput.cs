using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PriceOffers.Dto
{
    public class ShippingRequestForPriceOfferGetAllInput: PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string Filter { get; set; }
        public string Carrier { get; set; }
        public long? ShippingRequestId { get; set; }
        public PriceOfferChannel? Channel { get; set; }
        public ShippingRequestType? RequestType { get; set; }

        public int? TruckTypeId { get; set; }

        public int? OriginId { get; set; }

        public int? DestinationId { get; set; }

        public DateTime? PickupFromDate { get; set; }

        public DateTime? PickupToDate { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public ShippingRequestRouteType? RouteTypeId { get; set; }
        public int? Status { get; set; }
        public bool IsTachyonDeal { get; set; }
        public bool isTMSRequest { get; set; }

        public void Normalize()
        {
            if (!string.IsNullOrEmpty(Filter)) Filter = Filter.Trim().ToLower();
            if (!string.IsNullOrEmpty(Carrier)) Carrier = Carrier.Trim().ToLower();

        }
    }
}
