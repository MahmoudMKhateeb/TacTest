using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.DedicatedDynamicInvoices
{
    public class DedicatedDynamicInvoiceManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public DedicatedDynamicInvoiceManager(IRepository<ShippingRequest, long> shippingRequestRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
        }

        public async Task<int> _getDefaultNumberOfDays(long ShippingRequestId)
        {
            var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(x => x.Id == ShippingRequestId &&
            x.ShippingRequestFlag == ShippingRequestFlag.Dedicated);

            switch (shippingRequest.RentalDurationUnit)
            {
                case TimeUnit.Days:
                    return shippingRequest.RentalDuration;
                case TimeUnit.Months:
                    return shippingRequest.RentalDuration * 26;
                case TimeUnit.Weeks:
                    return shippingRequest.RentalDuration * 7;
                default:
                    return 0;
            }
        }


    }
}
