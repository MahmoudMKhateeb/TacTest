using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestStatuses;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class ViewCarrierBidsOutput
    {
         public ShippingRequestBidsDto ShippingRequestBidDto { get; set; }
        public ShippingRequestDto ShippingRequestDto { get; set; }
        //public long ShippingRequestId { get; set; }
        //public double price { get; set; }
        //public bool IsCancled { get; set; }
        //public bool IsAccepted { get; set; }
        //public bool IsRejected { get; set; }
        //public string CancledReason { get; set; }
        //public DateTime? AcceptedDate { get; set; }
        //public DateTime? CanceledDate { get; set; }
        public string ShipperTenancyName { get; set; }
    }
}
