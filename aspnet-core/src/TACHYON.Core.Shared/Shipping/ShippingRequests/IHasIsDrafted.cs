using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests
{
    public interface IHasIsDrafted
    {
        bool IsDrafted { get; set; }
    }
}