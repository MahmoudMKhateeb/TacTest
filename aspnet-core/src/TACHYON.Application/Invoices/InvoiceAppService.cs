using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices
{
    public class InvoiceAppService : TACHYONAppServiceBase, IInvoiceAppService
    {

        private readonly IRepository<Invoice, long> _InvoiceRepository;
        public InvoiceAppService(IRepository<Invoice, long> InvoiceRepository)
        {
            _InvoiceRepository = InvoiceRepository;
        }


        [AbpAuthorize(AppPermissions.Pages_Invoices)]
        public async Task<PagedResultDto<InvoiceListDto>> GetAll(InvoiceFilterInput input)

        {
            var user = GetCurrentUser();
            if (user.TenantId.HasValue)
            {
                return await GetInvoices(input);
            }
            else
            {
                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return await GetInvoices(input);

                }
            }

        }


        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_Delete)]

        public async Task Delete(EntityDto Input)
        {
            var Invoice = await GetInvoice(Input.Id); 
            if (Invoice !=null)
            {
                if (Invoice.IsPaid)
                {

                    

                }

                await _InvoiceRepository.DeleteAsync(Input.Id);

            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_MakePaid)]

        public async Task MakePaid(long invoiceId)
        {
            var Invoice = await GetInvoice(invoiceId);
            if (Invoice != null && !Invoice.IsPaid) Invoice.IsPaid = true;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Invoices_MakeUnPaid)]
        public async Task MakeUnPaid(long invoiceId)
        {
            var Invoice = await GetInvoice(invoiceId);
            if (Invoice != null && Invoice.IsPaid) Invoice.IsPaid = false;
        }


        #region Helper 
        private async Task<PagedResultDto<InvoiceListDto>> GetInvoices(InvoiceFilterInput input)
        {
            var query = _InvoiceRepository
                .GetAll()
                .Include(i=>i.InvoicePeriod)
                .Include(i => i.Tenant)
                 .ThenInclude(t=>t.Edition)
                .WhereIf(input.IsAccountReceivable.HasValue, e => e.IsAccountReceivable == input.IsAccountReceivable)
                .WhereIf(input.IsPaid.HasValue, e => e.IsPaid == input.IsPaid)
                .AsNoTracking();
            var pagedInvoices = query
              .OrderBy(input.Sorting ?? "id asc")
              .PageBy(input);

            var totalCount = await pagedInvoices.CountAsync();

            return new PagedResultDto<InvoiceListDto>(
                totalCount,
                ObjectMapper.Map<List<InvoiceListDto>>(pagedInvoices)
            );
        }

        public async Task<Invoice> GetInvoice(long  invoiceId)
        {
          return await _InvoiceRepository.SingleAsync(i => i.Id == invoiceId);
        }



        #endregion
    }
}
