using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Invoices.InvoicesProformas.dto;

namespace TACHYON.Invoices.InvoicesProformas
{

    public class InvoicesProformaAppService : TACHYONAppServiceBase, IInvoicesProformaAppService
    {
        private readonly IRepository<InvoiceProforma, long> _invoiceProformaRepository;

        public InvoicesProformaAppService(IRepository<InvoiceProforma, long> invoiceProformaRepository)
        {
            _invoiceProformaRepository = invoiceProformaRepository;
        }

        public async Task<PagedResultDto<InvoicesProformaListDto>> GetAll(InvoicesProformaFilterInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            var query = await GetResult(input);
            var pages = query.PageBy(input);

            var totalCount = await query.CountAsync();

            return new PagedResultDto<InvoicesProformaListDto>(
                totalCount,
                ObjectMapper.Map<List<InvoicesProformaListDto>>(pages)
            );
        }


        private async Task<IOrderedQueryable<InvoiceProforma>> GetResult(InvoicesProformaFilterInput input)
        {
            DisableTenancyFilters();
            var query = _invoiceProformaRepository
                .GetAll()
                .AsNoTracking()
                .Include(t => t.Tenant)
                 .ThenInclude(e => e.Edition)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), i => i.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), i => true)
                .WhereIf(input.TenantId.HasValue, i => i.TenantId == input.TenantId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, i => i.CreationTime >= input.FromDate.Value && i.CreationTime <= input.ToDate.Value)
                .WhereIf(input.MinAmount.HasValue && input.MaxAmount.HasValue, i => i.TotalAmount >= input.MinAmount.Value && i.TotalAmount <= input.MaxAmount.Value)             
                .OrderBy(!string.IsNullOrEmpty(input.Sorting) ? input.Sorting : "CreationTime desc");

            return query;
        }
    }
}
