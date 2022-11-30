using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.PricePackages.Dto.TmsPricePackages;
using TACHYON.PricePackages.TmsPricePackages;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages
{
    [AbpAuthorize(AppPermissions.Pages_TmsPricePackages)]
    public class TmsPricePackageAppService : TACHYONAppServiceBase, ITmsPricePackageAppService
    {
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly NormalPricePackageManager _normalPricePackageManager;
        private readonly ITmsPricePackageManager _tmsPricePackageManager;

        public TmsPricePackageAppService(
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            NormalPricePackageManager normalPricePackageManager,
            ITmsPricePackageManager tmsPricePackageManager)
        {
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _tenantRepository = tenantRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _normalPricePackageManager = normalPricePackageManager;
            _tmsPricePackageManager = tmsPricePackageManager;
        }


        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            DisableTenancyFilters();
            var isTmsOrHost = !AbpSession.TenantId.HasValue || await IsTachyonDealer();
            
            var pricePackages = _tmsPricePackageRepository.GetAll().AsNoTracking()
                .WhereIf(!isTmsOrHost, x => x.DestinationTenantId == AbpSession.TenantId)
                .ProjectTo<TmsPricePackageListDto>(AutoMapperConfigurationProvider);
            
            return await LoadResultAsync(pricePackages,input.LoadOptions);
        }

        public async Task<TmsPricePackageForViewDto> GetForView(int pricePackageId)
        {
            var pricePackage = await _tmsPricePackageRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pricePackageId);

            if (pricePackage == null) throw new EntityNotFoundException(L("NotFound"));
            
            return ObjectMapper.Map<TmsPricePackageForViewDto>(pricePackage);
        }

        
        public async Task CreateOrEdit(CreateOrEditTmsPricePackageDto input)
        {

            if (input.Id.HasValue)
            {
                await Update(input);
                return;
            }

            await Create(input);
        }

        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Create)]
        protected virtual async Task Create(CreateOrEditTmsPricePackageDto input)
        {
            var createdTmsPricePackage = ObjectMapper.Map<TmsPricePackage>(input);
            
            var id = await _tmsPricePackageRepository.InsertAndGetIdAsync(createdTmsPricePackage);
            var isMultiDrop = input.RouteType == ShippingRequestRouteType.MultipleDrops;
            
            createdTmsPricePackage.PricePackageId = _normalPricePackageManager
                .GeneratePricePackageReferanceNumber(id,isMultiDrop, createdTmsPricePackage.CreationTime);
        }

        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Update)]
        protected virtual async Task Update(CreateOrEditTmsPricePackageDto input)
        {
            if (!input.Id.HasValue) return;
            
            var updatedTmsPricePackage = await _tmsPricePackageRepository.FirstOrDefaultAsync(input.Id.Value);

            ObjectMapper.Map(input, updatedTmsPricePackage);
        }
        
        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Update)]
        public async Task<CreateOrEditTmsPricePackageDto> GetForEdit(int pricePackageId)
        {
            var tmsPricePackage = await _tmsPricePackageRepository.GetAll().AsNoTracking()
                .FirstOrDefaultAsync(x=> x.Id == pricePackageId);
            
            if (tmsPricePackage == null) throw new EntityNotFoundException(L("NotFound"));
            
            return ObjectMapper.Map<CreateOrEditTmsPricePackageDto>(tmsPricePackage);
        }

        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Delete)]
        public async Task Delete(int pricePackageId)
        {
            var isExist = await _tmsPricePackageRepository.GetAll().AnyAsync(x => x.Id == pricePackageId);

            if (!isExist) throw new EntityNotFoundException(L("NotFound"));

            await _tmsPricePackageRepository.DeleteAsync(x => x.Id == pricePackageId);
        }

        public async Task SendOfferByPricePackage(int pricePackageId,long srId)
        {
            await _tmsPricePackageManager.SendOfferByPricePackage(pricePackageId, srId);
        }

        public async Task AcceptOfferByPricePackage(int pricePackageId)
        {
            DisableTenancyFilters();
            var pricePackage = await _tmsPricePackageRepository.GetAll()
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(),x=> x.DestinationTenantId == AbpSession.TenantId)
                .SingleAsync(x => x.Id == pricePackageId);
            
            await _tmsPricePackageManager.AcceptOfferByPricePackage(pricePackage);
        }
        public async Task<TmsPricePackageForPricingDto> GetForPricing(int pricePackageId)
        {
            DisableTenancyFilters();
            
            var pricePackage = await _tmsPricePackageRepository.GetAll().AsNoTracking()
                .Where(x => x.Id == pricePackageId)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(),x=> x.DestinationTenantId == AbpSession.TenantId)
                .ProjectTo<TmsPricePackageForPricingDto>(AutoMapperConfigurationProvider)
                .SingleAsync();
            return pricePackage;
        }
        public async Task<PagedResultDto<TmsPricePackageForViewDto>> GetMatchingPricePackages(GetMatchingPricePackagesInput input)
        {
            DisableTenancyFilters();

            var isTmsOrHost = !AbpSession.TenantId.HasValue || await IsTachyonDealer();
            var matchedPricePackages = (from shippingRequest in _shippingRequestRepository.GetAll()
                    where shippingRequest.Id == input.ShippingRequestId
                    from tmsPricePackage in _tmsPricePackageRepository.GetAll()
                    where (tmsPricePackage.ProposalId.HasValue || tmsPricePackage.AppendixId.HasValue) &&
                          (!tmsPricePackage.ProposalId.HasValue ||
                           (tmsPricePackage.Proposal.Status == ProposalStatus.Approved &&
                            tmsPricePackage.Proposal.AppendixId.HasValue &&
                            tmsPricePackage.Proposal.Appendix.Status == AppendixStatus.Confirmed &&
                            tmsPricePackage.Proposal.ShipperId == shippingRequest.TenantId)) &&
                          (!tmsPricePackage.AppendixId.HasValue ||
                           (tmsPricePackage.Appendix.Status == AppendixStatus.Confirmed &&
                            !tmsPricePackage.ProposalId.HasValue)) &&
                          (isTmsOrHost || tmsPricePackage.DestinationTenantId == AbpSession.TenantId) &&
                          tmsPricePackage.Type == PricePackageType.PerTrip &&
                          tmsPricePackage.TrucksTypeId == shippingRequest.TrucksTypeId &&
                          tmsPricePackage.OriginCityId == shippingRequest.OriginCityId &&
                          tmsPricePackage.RouteType == shippingRequest.RouteTypeId
                          && shippingRequest.ShippingRequestDestinationCities.Any(i =>
                              i.CityId == tmsPricePackage.DestinationCityId)
                    orderby tmsPricePackage.Id
                    select tmsPricePackage)
                .ProjectTo<TmsPricePackageForViewDto>(AutoMapperConfigurationProvider);

            var pageResult = await matchedPricePackages.PageBy(input).ToListAsync();

            var totalCount = await matchedPricePackages.CountAsync();

            return new PagedResultDto<TmsPricePackageForViewDto>()
            {
                Items = ObjectMapper.Map<List<TmsPricePackageForViewDto>>(pageResult), TotalCount = totalCount
            };
            
        }

        public async Task<List<SelectItemDto>> GetCompanies()
        {
            DisableTenancyFilters();
            return await (from tenant in _tenantRepository.GetAll()
                    where tenant.EditionId == CarrierEditionId || tenant.EditionId == ShipperEditionId
                    select new SelectItemDto { DisplayName = tenant.Name, Id = tenant.Id.ToString() })
                .ToListAsync();
        }

        public async Task ChangeActivateStatus(int pricePackageId,bool isActive)
        {
            var isExist = await _tmsPricePackageRepository.GetAll().AnyAsync(x => x.Id == pricePackageId);
            if (!isExist) throw new UserFriendlyException(L("NotFound"));
            
            _tmsPricePackageRepository.Update(pricePackageId, x => x.IsActive = isActive);
        }
        
        public async Task<LoadResult> GetAllForDropdown(GetTmsPricePackagesInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var tmsPricePackages = _tmsPricePackageRepository.GetAll()
                .AsNoTracking().Where(x=> x.IsActive && x.DestinationTenantId == input.DestinationTenantId)
                .WhereIf(input.ProposalId.HasValue,x=> !x.ProposalId.HasValue || x.ProposalId == input.ProposalId)
                .WhereIf(input.AppendixId.HasValue,x=> !x.AppendixId.HasValue || x.AppendixId == input.AppendixId)
                .ProjectTo<TmsPricePackageSelectItemDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(tmsPricePackages, input.LoadOptions);
        }
    }
}