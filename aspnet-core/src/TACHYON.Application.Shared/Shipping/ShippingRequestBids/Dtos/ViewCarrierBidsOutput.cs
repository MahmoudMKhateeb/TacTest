using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestStatuses;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class ViewCarrierBidsOutput
    {
        public List<ShippingRequestBidsDto> ShippingRequestBidsListDto { get; set; }
        public double Price { get; set; }
        public bool IsCancled { get; set; }
        public bool? IsAccepted { get; set; }
        public string CancledReason { get; set; }
        public DateTime AcceptedDate { get; set; }
        public DateTime CancledDate { get; set; }
    }
}
