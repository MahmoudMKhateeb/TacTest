using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests
{
    public interface IShippingRequestsAppService : IApplicationService
    {
        Task<GetAllShippingRequestsOutputDto> GetAll(GetAllShippingRequestsInput Input);

        // Task<GetAllShippingRequestsOutput> GetForAll(GetAllShippingRequestsInput input);
        Task<GetShippingRequestForViewOutput> GetShippingRequestForView(long id);

        Task<GetShippingRequestForEditOutput> GetShippingRequestForEdit(EntityDto<long> input);

        Task UpdatePrice(UpdatePriceInput input);
        Task CreateOrEdit(CreateOrEditShippingRequestDto input);
        Task AcceptOrRejectShippingRequestPrice(AcceptShippingRequestPriceInput input);
        Task RejectShippingRequest(long id);
        Task Delete(EntityDto<long> input);


        //Task<FileDto> GetShippingRequestsToExcel(GetAllShippingRequestsForExcelInput input);
        IEnumerable<GetMasterWaybillOutput> GetMasterWaybill(int shippingRequestTripId);
        IEnumerable<GetSingleDropWaybillOutput> GetSingleDropWaybill(int shippingRequestTripId);

        IEnumerable<GetAllShippingRequestVasesOutput> GetShippingRequestVasesForSingleDropWaybill(
            int shippingRequestTripId);

        IEnumerable<GetMultipleDropWaybillOutput> GetMultipleDropWaybill(long routPointId);
        IEnumerable<GetAllShippingRequestVasesOutput> GetShippingRequestVasesForMultipleDropWaybill(long RoutPointId);
    }
}