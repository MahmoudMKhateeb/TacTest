using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Shipping.ShippingRequests
{
    public class GetShippingRequestsForInvoiceDomainService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;

        public GetShippingRequestsForInvoiceDomainService(IRepository<ShippingRequest, long> shippingRequestRepository)
        {
            _shippingRequestRepository = shippingRequestRepository;
        }

        public async Task<List<ShippingRequest>> GetShippingRequestsByTenant(int id)
        {
            var shippingRequests =await _shippingRequestRepository.GetAll()
                .Where(e => e.TenantId == id)
                // .Where(x=>x.InvoiceId==null)
                //.Where(x=>x.IsPrePayed==false)
                .Where(x => x.IsPriceAccepted == true)
                .ToListAsync();

            return shippingRequests;
        }
    }
}
