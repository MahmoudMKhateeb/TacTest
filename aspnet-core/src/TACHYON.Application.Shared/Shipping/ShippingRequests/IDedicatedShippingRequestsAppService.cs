using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;

namespace TACHYON.Shipping.ShippingRequests
{
    public interface IDedicatedShippingRequestsAppService: IApplicationService
    {
        Task<long> CreateOrEditStep1(CreateOrEditDedicatedStep1Dto input);
        Task EditStep2(EditDedicatedStep2Dto input);
        Task PublishDedicatedShippingRequest(long id);

    }
}
