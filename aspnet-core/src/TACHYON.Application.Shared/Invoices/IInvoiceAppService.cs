using Abp.Application.Services;
using Abp.Application.Services.Dto;
using DevExtreme.AspNet.Data.ResponseModel;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices
{
    public interface IInvoiceAppService : IApplicationService
    {
        Task<LoadResult> GetAll(string filter);

        Task<InvoiceInfoDto> GetById(EntityDto input);

        Task<bool> MakePaid(long InvoiceId);
        Task MakeUnPaid(long InvoiceId);

        Task OnDemand(int Id);

        Task<FileDto> Exports(InvoiceFilterInput input);
        Task<FileDto> ExportItems(long id);
    }
}