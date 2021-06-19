using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.InvoicesProformas.dto;

namespace TACHYON.Invoices.InvoicesProformas
{
    public interface IInvoicesProformaAppService: IApplicationService
    {
        Task<PagedResultDto<InvoicesProformaListDto>> GetAll(InvoicesProformaFilterInput input);

    }
}
