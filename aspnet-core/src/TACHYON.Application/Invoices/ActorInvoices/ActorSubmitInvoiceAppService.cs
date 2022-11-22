using Abp.Application.Features;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Invoices.ActorInvoices.Dto;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.SubmitInvoices.Dto;
using TACHYON.Invoices.Transactions;

namespace TACHYON.Invoices.ActorInvoices
{

    [AbpAuthorize]
    [RequiresFeature(AppFeatures.CarrierClients, AppFeatures.TachyonDealer)]

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

            var Invoice = await GetActorSubmitInvoice(SubmitinvoiceId);
            if (Invoice != null && Invoice.Status != SubmitInvoiceStatus.UnPaid)
            {
                Invoice.Status = SubmitInvoiceStatus.UnPaid;
            }
        }

        public async Task Claim(SubmitInvoiceClaimCreateInput Input)
        {
            var submit = await GetActorSubmitInvoice(Input.Id);
            submit.DueDate = Clock.Now;

            if (submit.Status == SubmitInvoiceStatus.Claim || submit.Status == SubmitInvoiceStatus.Accepted) return;

            var document = await _commonManager.UploadDocumentAsBase64(ObjectMapper.Map<DocumentUpload>(Input), AbpSession.TenantId);
            submit.Status = SubmitInvoiceStatus.Claim;
            ObjectMapper.Map(document, submit);

        }

        public async Task<FileDto> GetFileDto(long GroupId)
        {
            DisableTenancyFilters();
            var documentFile = await _actorSubmitInvoiceRepository.FirstOrDefaultAsync(g =>
                g.Id == GroupId && g.Status != SubmitInvoices.SubmitInvoiceStatus.New);
            if (documentFile == null)
                throw new UserFriendlyException(L("TheRequestNotFound"));

            return await _commonManager.GetDocument(ObjectMapper.Map<IHasDocument>(documentFile));
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
