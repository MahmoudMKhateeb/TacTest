using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests
{
    public interface IShippingRequestsAppService : IApplicationService
    {
        Task<GetAllShippingRequestsOutput> GetAll(GetAllShippingRequestsInput input);

        Task<GetShippingRequestForViewOutput> GetShippingRequestForView(long id);

        Task<GetShippingRequestForEditOutput> GetShippingRequestForEdit(EntityDto<long> input);

        Task UpdatePrice(UpdatePriceInput input);
        Task CreateOrEdit(CreateOrEditShippingRequestDto input);
        Task AcceptOrRejectShippingRequestPrice(AcceptShippingRequestPriceInput input);
        Task RejectShippingRequest(long id);
        Task Delete(EntityDto<long> input);

   
        //Task<FileDto> GetShippingRequestsToExcel(GetAllShippingRequestsForExcelInput input);
        IEnumerable<GetMasterWaybillOutput> GetMasterWaybill(long shippingRequestId);

    }
}