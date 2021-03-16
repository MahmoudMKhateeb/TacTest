using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
  public  enum ShippingRequestBidStatus:byte
    {
        StandBy,
        OnGoing,
        Closed,
        Cancled
    }
}
