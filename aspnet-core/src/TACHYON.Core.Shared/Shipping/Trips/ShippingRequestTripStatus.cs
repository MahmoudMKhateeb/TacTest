using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips
{
  public  enum ShippingRequestTripStatus:byte
    {
        StandBy=0,
        PickupWay= 1,
        StartLoading = 2,
        Dropoffway = 3,
        DropoffArrived = 4,
        offloading =5,
        Finished=6
    }
}
