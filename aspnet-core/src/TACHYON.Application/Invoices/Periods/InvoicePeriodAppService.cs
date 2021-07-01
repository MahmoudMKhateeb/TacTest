using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Threading;
using Abp.UI;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Invoices.Periods.Dto;
using TACHYON.Invoices.Periods.Exporting;
namespace TACHYON.Invoices.Periods
{
    [AbpAuthorize(AppPermissions.Pages_Invoices)]
    public class InvoicePeriodAppService : TACHYONAppServiceBase, IInvoicePeriodAppService
    {
        private readonly IRepository<InvoicePeriod> _PeriodRepository;
        private readonly IInvoicePeriodExport _export;
        private readonly InvoiceManager _invoiceManager;
        private readonly IHostApplicationLifetime _appLifetime;
        public InvoicePeriodAppService(
            IRepository<InvoicePeriod> PeriodRepository,
            IInvoicePeriodExport export,
            InvoiceManager invoiceManager,
            IHostApplicationLifetime appLifetime)
        {
            _PeriodRepository = PeriodRepository;
            _export = export;
            _invoiceManager = invoiceManager;
            _appLifetime = appLifetime;
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Periods)]

        public ListResultDto<InvoicePeriodDto> GetAll(FilterInput input)
        {


            return new ListResultDto<InvoicePeriodDto>(GetData(input));
        }

        public PeriodCommonDto GetAllCommon()
        {
            return new PeriodCommonDto { Months = Dates.DateHeleper.MonthNamesList(), Weeks = Dates.DateHeleper.WeeksDayNamesList() };

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Periods)]

        public async Task CreateEdit(InvoicePeriodDto input)
        {
            var Period = await _PeriodRepository.FirstOrDefaultAsync(x => (!input.Id.HasValue || x.Id != (int)input.Id) && x.DisplayName.ToLower() == input.DisplayName.Trim().ToLower());
            if (Period != null)
            {
                throw new UserFriendlyException(L("DisplayNameExists"));
            }
            if (!input.Id.HasValue)
            {
                await Create(input);

                await Task.Run(() => {_appLifetime.StopApplication();});
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Period_Create)]

        public async Task Create(InvoicePeriodDto input)
        {
            var Period = ObjectMapper.Map<InvoicePeriod>(input);

            await _PeriodRepository.InsertAsync(Period);

            if (Period.Enabled & Period.PeriodType != InvoicePeriodType.PayInAdvance && Period.PeriodType != InvoicePeriodType.PayuponDelivery)
            await _invoiceManager.CreateTiggerAsync(Period);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Period_Edit)]

        protected async Task Update(InvoicePeriodDto input)
        {
            var Period = await _PeriodRepository.FirstOrDefaultAsync((int)input.Id);

            ObjectMapper.Map(input, Period);

            await _invoiceManager.UpdateTriggerAsync(Period);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Period_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _PeriodRepository.DeleteAsync(input.Id);
        }



        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Period_Enabled)]
        public async Task Enabled(int PeriodId, bool IsEnabled)
        {
            var Period = await _PeriodRepository.FirstOrDefaultAsync(PeriodId);
            if (Period != null)
            {

                Period.Enabled = IsEnabled;

                await _invoiceManager.CreateTiggerAsync(Period);

            }
        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Periods)]
        public FileDto ExportToExcel(FilterInput input)
        {
            return _export.ExportToFile(GetData(input));
        }


        private List<InvoicePeriodDto> GetData(FilterInput input)
        {
            var Results = _PeriodRepository
                            .GetAll()
                            .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.DisplayName.Contains(input.Filter))
                            .OrderBy(p => p.PeriodType);

            return ObjectMapper.Map<List<InvoicePeriodDto>>(Results);
        }


  
    }
}
