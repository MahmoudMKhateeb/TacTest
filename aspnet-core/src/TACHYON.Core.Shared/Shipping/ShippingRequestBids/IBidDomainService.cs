using Abp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Shipping.ShippingRequestBids
{
    public interface IBidDomainService
    {
         Task<UserIdentifier[]> GetCarriersByTruckTypeArrayAsync(long trucksTypeId);
    }
}
