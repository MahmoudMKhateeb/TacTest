using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Features;
using TACHYON.Invoices.ActorInvoices.Dto;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.Transactions;

namespace TACHYON.Invoices.ActorInvoices
{
    public class ActorSubmitInvoiceAppService : TACHYONAppServiceBase, IApplicationService
    {
        private readonly IRepository<ActorSubmitInvoice, long> _actorSubmitInvoiceRepository;
        private readonly CommonManager _commonManager;

        public ActorSubmitInvoiceAppService(IRepository<ActorSubmitInvoice, long> actorSubmitInvoiceRepository,
            CommonManager commonManager)
        {
            _actorSubmitInvoiceRepository = actorSubmitInvoiceRepository;
            _commonManager = commonManager;
        }

        public async Task<LoadResult> GetAll(string filter)

        {
            var query = _actorSubmitInvoiceRepository
                .GetAll()
                .ProjectTo<ActorSubmitInvoiceListDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            return await LoadResultAsync<ActorSubmitInvoiceListDto>(query, filter);
        }


        public async Task MakeActorSubmitInvoicePaid(long SubmitinvoiceId)
        {
           // CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            var Invoice = await GetActorSubmitInvoice(SubmitinvoiceId);
            if (Invoice != null && Invoice.Status != SubmitInvoiceStatus.Paid)
            {
                await _commonManager.ExecuteMethodIfHostOrTenantUsers(() => {
                    Invoice.Status = SubmitInvoiceStatus.Paid;
                    return Task.CompletedTask;
                });
            }
        }
        public async Task MakeActorSubmitInvoiceUnPaid(long SubmitinvoiceId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            var Invoice = await GetActorSubmitInvoice(SubmitinvoiceId);
            if (Invoice != null && Invoice.Status != SubmitInvoiceStatus.UnPaid)
            {
                Invoice.Status = SubmitInvoiceStatus.UnPaid;
            }
        }


        private async Task<ActorSubmitInvoice> GetActorSubmitInvoice(long id)
        {
            DisableTenancyFilters();
            return await _actorSubmitInvoiceRepository.GetAll()
                .Include(x => x.Tenant)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), e => e.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

    }
}
