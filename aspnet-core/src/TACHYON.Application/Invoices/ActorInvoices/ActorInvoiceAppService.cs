using Abp.Application.Features;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
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
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        public ActorInvoiceAppService(
            IRepository<ActorInvoice, long> actorInvoiceRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository)
        {
            _actorInvoiceRepository = actorInvoiceRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
        }


        public async Task<LoadResult> GetAll(string filter)
        {
            
            bool isCmsEnabled = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            
            List<long> userOrganizationUnits = null;
            if (isCmsEnabled)
            {
                userOrganizationUnits = await _userOrganizationUnitRepository.GetAll().Where(x => x.UserId == AbpSession.UserId)
                    .Select(x => x.OrganizationUnitId).ToListAsync();
            }
            
            var query = _actorInvoiceRepository
                .GetAll()
                .WhereIf(isCmsEnabled && !userOrganizationUnits.IsNullOrEmpty(),
                    x=> x.ShipperActorId.HasValue && userOrganizationUnits.Contains(x.ShipperActorFk.OrganizationUnitId))
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
