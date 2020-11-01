using Abp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequestBids
{
    public interface IBidDomainService
    {
         UserIdentifier[] GetCarriersByTruckTypeArray(long trucksTypeId);
    }
}
