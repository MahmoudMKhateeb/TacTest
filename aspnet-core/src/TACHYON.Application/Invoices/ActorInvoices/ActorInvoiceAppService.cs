using Abp.Application.Features;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Invoices.ActorInvoices.Dto;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices.ActorInvoices
{
    [AbpAuthorize(permissions: AppPermissions.Pages_Administration_ActorsInvoice)]
    public class ActorInvoiceAppService : TACHYONAppServiceBase, IApplicationService
    {

        private readonly IRepository<ActorInvoice, long> _actorInvoiceRepository;

        public ActorInvoiceAppService(IRepository<ActorInvoice, long> actorInvoiceRepository)
        {
            _actorInvoiceRepository = actorInvoiceRepository;
        }


        public async Task<LoadResult> GetAll(string filter)
        {
            var query = _actorInvoiceRepository
                .GetAll()
                .ProjectTo<ActorInvoiceListDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            return await LoadResultAsync<ActorInvoiceListDto>(query, filter);
        }

        public async Task<bool> MakePaid(long invoiceId)
        {
            var Invoice = await GetActorInvoice(invoiceId);
            if (Invoice != null && !Invoice.IsPaid)
            {
                Invoice.IsPaid = true;
                return true;
            }
            return false;
        }

        public async Task MakeUnPaid(long invoiceId)
        {
            var Invoice = await GetActorInvoice(invoiceId);
            if (Invoice != null && Invoice.IsPaid)
            {
                Invoice.IsPaid = false;
            }
        }


        private async Task<ActorInvoice> GetActorInvoice(long id)
        {
            await DisableDraftedFilterIfTachyonDealerOrHost();
            return await _actorInvoiceRepository.FirstOrDefaultAsync(id);
        }

    }


}
