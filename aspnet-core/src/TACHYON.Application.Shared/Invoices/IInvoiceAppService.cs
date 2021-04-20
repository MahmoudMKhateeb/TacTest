using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices
{
    public interface IInvoiceAppService : IApplicationService
    {
        Task<PagedResultDto<InvoiceListDto>> GetAll(InvoiceFilterInput input);

        Task<InvoiceInfoDto> GetById(EntityDto input);

        Task<bool> MakePaid(long InvoiceId);
        Task MakeUnPaid(long InvoiceId);

        Task Delete(EntityDto Input);
        
        Task<FileDto> Exports(InvoiceFilterInput input);


    }
}
