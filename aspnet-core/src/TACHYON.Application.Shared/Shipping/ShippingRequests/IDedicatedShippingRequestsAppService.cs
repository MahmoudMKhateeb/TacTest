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
        Task<GetAllTrucksAndDriversForRequestOutput> GetAllTrucksAndDriversForRequest(long shippingRequestId);
        Task<long> CreateOrEditStep1(CreateOrEditDedicatedStep1Dto input);
        Task<CreateOrEditDedicatedStep1Dto> GetStep1ForEdit(long id);
        Task EditStep2(EditDedicatedStep2Dto input);
        Task<EditDedicatedStep2Dto> GetStep2ForEdit(long id);
        Task PublishDedicatedShippingRequest(long id);
        Task AssignDedicatedTrucksAndDrivers(AssignDedicatedTrucksAndDriversInput input);
    }
}
