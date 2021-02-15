using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Invoices.Periods.Dto;

namespace TACHYON.Invoices.Periods
{
    public interface IInvoicePeriodAppService : IApplicationService
    {
        ListResultDto<InvoicePeriodDto> GetAll(FilterInput input);
        PeriodCommonDto GetAllCommon();

        Task CreateEdit(InvoicePeriodDto input);
        Task Delete(EntityDto input);
        Task Enabled(int PeriodId,bool IsEnabled);
        FileDto ExportToExcel(FilterInput input);



    }
}
