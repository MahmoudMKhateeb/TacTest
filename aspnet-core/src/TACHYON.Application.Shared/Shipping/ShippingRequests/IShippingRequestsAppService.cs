using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests.Dtos;


namespace TACHYON.Shipping.ShippingRequests
{
    public interface IShippingRequestsAppService : IApplicationService
    {
        Task<GetAllShippingRequestsOutput> GetAll(GetAllShippingRequestsInput input);

        Task<GetShippingRequestForViewOutput> GetShippingRequestForView(long id);

        Task<GetShippingRequestForEditOutput> GetShippingRequestForEdit(EntityDto<long> input);

        Task CreateOrEdit(CreateOrEditShippingRequestDto input);

        Task Delete(EntityDto<long> input);

   
        //Task<FileDto> GetShippingRequestsToExcel(GetAllShippingRequestsForExcelInput input);

    }
}