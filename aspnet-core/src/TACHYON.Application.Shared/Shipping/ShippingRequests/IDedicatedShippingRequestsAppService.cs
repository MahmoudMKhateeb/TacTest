using Abp.Application.Services;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;

namespace TACHYON.Shipping.ShippingRequests
{
    public interface IDedicatedShippingRequestsAppService: IApplicationService
    {
        Task<LoadResult> GetAllDedicatedTrucks(GetAllDedicatedTrucksInput input);
        Task<LoadResult> GetAllDedicatedDrivers(GetAllDedicatedDriversInput input);
        Task<long> CreateOrEditStep1(CreateOrEditDedicatedStep1Dto input);
        Task<CreateOrEditDedicatedStep1Dto> GetStep1ForEdit(long id);
        Task EditStep2(EditDedicatedStep2Dto input);
        Task<EditDedicatedStep2Dto> GetStep2ForEdit(long id);
        Task PublishDedicatedShippingRequest(long id);
        Task AssignDedicatedTrucksAndDrivers(AssignDedicatedTrucksAndDriversInput input);
    }
}
