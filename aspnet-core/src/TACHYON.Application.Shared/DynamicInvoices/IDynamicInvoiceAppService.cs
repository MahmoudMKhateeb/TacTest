using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.DynamicInvoices.Dto;

namespace TACHYON.DynamicInvoices
{
    public interface IDynamicInvoiceAppService : IApplicationService
    {
        Task<PagedResultDto<DynamicInvoiceListDto>> GetAll(GetDynamicInvoicesInput input);

        Task<DynamicInvoiceForViewDto> GetForView(long dynamicInvoiceId);

        Task CreateOrEdit(CreateOrEditDynamicInvoiceDto input);

        Task Delete(long dynamicInvoiceId);
    }
}