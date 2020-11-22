using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids
{
    [Serializable]
    public class RejectOtherBidsJobArgs
    {
        /// <summary>
        /// the shipping request id that we use to obtain the bids from it
        /// </summary>
        public long ShippingReuquestId { get; set; }
        /// <summary>
        /// accepted bid id to remove it from rejected list
        /// </summary>
        public long AcceptedBidId { get; set; }
    }
}
