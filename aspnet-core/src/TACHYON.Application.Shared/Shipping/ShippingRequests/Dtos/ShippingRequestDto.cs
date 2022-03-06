
using Abp.Application.Services.Dto;
using System;


namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestDto : EntityDto<long>
    {
        //public decimal Vas { get; set; }

        public bool IsBid { get; set; }
        public bool IsTachyonDeal { get; set; }
        public bool IsDirectRequest { get; set; }
        public ShippingRequestType RequestType { get; set; }
        public ShippingRequestRouteType? RouteTypeId { get; set; }

        public decimal? Price { get; set; }
        public bool? IsPriceAccepted { get; set; }
        public int? CarrierTenantId { get; set; }
        public decimal? CarrierPrice { get; set; }
        public bool? IsRejected { get; set; }
        public DateTime? StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }

        public int NumberOfTrips { get; set; }
        public int NumberOfDrops { get; set; }

        public int GoodCategoryId { get; set; }
        public int PackingTypeId { get; set; }
        public int NumberOfPacking { get; set; }
        public double TotalWeight { get; set; }
        public int ShippingTypeId { get; set; }

        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }

        public bool HasAccident { get; set; }
        public int TotalsTripsAddByShippier { get; set; }
        public ShippingRequestStatus Status { get; set; }
        public ShippingRequestBidStatus BidStatus { get; set; }

        public string OtherGoodsCategoryName { get; set; }
        public string OtherTransportTypeName { get; set; }
        public string OtherTrucksTypeName { get; set; }

        public bool AddTripsByTmsEnabled { get; set; }

        public bool CanAddTrip { get; set; }

        /// <summary>
        /// This reference shipper add it manually
        /// </summary>
        public string ShipperReference { get; set; }
        /// <summary>
        /// shipper add his invoice number manually
        /// </summary>
        public string ShipperInvoiceNo { get; set; }

        public bool IsSaas { get; set; }
        public string StatusTitle
        {
            get { return Status.GetEnumDescription(); }
        }

        public string BidStatusTitle
        {
            get { return BidStatus.GetEnumDescription(); }
        }
    }
}