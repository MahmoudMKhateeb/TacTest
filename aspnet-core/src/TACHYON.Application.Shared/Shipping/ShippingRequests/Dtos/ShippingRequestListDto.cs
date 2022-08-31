using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class ShippingRequestListDto : EntityDto<long>
    {
        public string Tenant { get; set; }
        public bool IsBid { get; set; }
        public bool IsTachyonDeal { get; set; }
        public bool IsDirectRequest { get; set; }
        public int TotalBids { get; set; }
        public int NumberOfTrips { get; set; }
        public int NumberOfDrops { get; set; }
        public bool HasAccident { get; set; }
        public int TotalsTripsAddByShippier { get; set; }
        public ShippingRequestStatus Status { get; set; }
        public bool CreatedByTachyonDealer { get; set; }
        public string OthersGoodsCategory { get; set; }
        public string OthersTransportType { get; set; }
        public string OthersTrucksType { get; set; }

        public string StatusTitle
        {
            get { return Enum.GetName(typeof(ShippingRequestStatus), Status); }
        }

        public string Origin { get; set; }
        //public string Destination { get; set; }

        public string RouteType { get; set; }
        public bool IsDrafted { get; set; }
        public int DraftStep { get; set; }
    }
}