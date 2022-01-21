using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Invoices.Groups.Dto;
using TACHYON.Invoices.SubmitInvoices.Dto;

namespace TACHYON.Invoices.SubmitInvoices
{
    public interface ISubmitInvoiceAppService : IApplicationService
    {
        Task<SubmitInvoiceInfoDto> GetById(EntityDto input);

        Task Claim(SubmitInvoiceClaimCreateInput Input);
        Task Accepted(long id);
        Task Rejected(SubmitInvoiceRejectedInput Input);

        Task<FileDto> GetFileDto(long id);
        Task<FileDto> Exports(SubmitInvoiceFilterInput input);

        Task<FileDto> ExportItems(long id);
    }
}