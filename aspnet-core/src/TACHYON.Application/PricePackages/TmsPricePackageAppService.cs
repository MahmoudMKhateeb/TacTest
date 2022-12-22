using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.Dto.TmsPricePackages;
using TACHYON.PricePackages.PricePackageAppendices;
using TACHYON.PricePackages.TmsPricePackageOffers;
using TACHYON.PricePackages.TmsPricePackages;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages
{
    [AbpAuthorize(AppPermissions.Pages_TmsPricePackages)]
    public class TmsPricePackageAppService : TACHYONAppServiceBase, ITmsPricePackageAppService
    {
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<NormalPricePackage> _normalPricePackageRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _directRequestRepository;
        private readonly NormalPricePackageManager _normalPricePackageManager;
        private readonly ITmsPricePackageManager _tmsPricePackageManager;
        private readonly ITmsPricePackageOfferManager _tmsPricePackageOfferManager;
        private readonly IRepository<PricePackageAppendix> _appendixRepository;
        private readonly IRepository<TmsPricePackageOffer,long> _tmsOfferRepository;
        private readonly IRepository<PriceOffer,long> _priceOfferRepository;
        private readonly IPriceOfferAppService _priceOfferAppService;
        private readonly IRepository<User,long> _userRepository;

        public TmsPricePackageAppService(
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            NormalPricePackageManager normalPricePackageManager,
            ITmsPricePackageManager tmsPricePackageManager, 
            IRepository<NormalPricePackage> normalPricePackageRepository, 
            IRepository<ShippingRequestDirectRequest, long> directRequestRepository,
            ITmsPricePackageOfferManager tmsPricePackageOfferManager,
            IRepository<PricePackageAppendix> appendixRepository,
            IRepository<TmsPricePackageOffer, long> tmsOfferRepository,
            IRepository<PriceOffer, long> priceOfferRepository,
            IRepository<User, long> userRepository,
            IPriceOfferAppService priceOfferAppService)
        {
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _tenantRepository = tenantRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _normalPricePackageManager = normalPricePackageManager;
            _tmsPricePackageManager = tmsPricePackageManager;
            _normalPricePackageRepository = normalPricePackageRepository;
            _directRequestRepository = directRequestRepository;
            _tmsPricePackageOfferManager = tmsPricePackageOfferManager;
            _appendixRepository = appendixRepository;
            _tmsOfferRepository = tmsOfferRepository;
            _priceOfferRepository = priceOfferRepository;
            _userRepository = userRepository;
            _priceOfferAppService = priceOfferAppService;
        }


        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
            DisableTenancyFilters(); 
            var isTmsOrHost = !AbpSession.TenantId.HasValue || await IsTachyonDealer();

            var pricePackages = _tmsPricePackageRepository.GetAll().AsNoTracking()
                .WhereIf(!isTmsOrHost,
                    x => x.DestinationTenantId == AbpSession.TenantId &&
                         (x.Proposal.Appendix.Status == AppendixStatus.Confirmed ||
                          x.Appendix.Status == AppendixStatus.Confirmed) && (x.Proposal.Appendix.IsActive || x.Appendix.IsActive))
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

        public async Task ApplyPricePackage(int pricePackageId,long srId,bool isTmsPricePackage)
        {
            await _tmsPricePackageOfferManager.ApplyPricingOnShippingRequest(pricePackageId, srId,isTmsPricePackage);
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
            var matchedPricePackages = await (from shippingRequest in _shippingRequestRepository.GetAll()
                    where shippingRequest.Id == input.ShippingRequestId
                    from tmsPricePackage in _tmsPricePackageRepository.GetAll().AsNoTracking()
                    where (tmsPricePackage.ProposalId.HasValue || tmsPricePackage.AppendixId.HasValue) &&
                          (!tmsPricePackage.ProposalId.HasValue ||
                           (tmsPricePackage.Proposal.Status == ProposalStatus.Approved &&
                            tmsPricePackage.Proposal.AppendixId.HasValue &&
                            tmsPricePackage.Proposal.Appendix.Status == AppendixStatus.Confirmed &&
                            tmsPricePackage.Proposal.Appendix.IsActive &&
                            tmsPricePackage.Proposal.ShipperId == shippingRequest.TenantId)) &&
                          (!tmsPricePackage.AppendixId.HasValue ||
                           (tmsPricePackage.Appendix.Status == AppendixStatus.Confirmed &&
                            tmsPricePackage.Appendix.IsActive &&
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
                .ProjectTo<TmsPricePackageForViewDto>(AutoMapperConfigurationProvider).ToListAsync();

            
            var matchedNormalPricePackage = await (from shippingRequest in _shippingRequestRepository.GetAll().AsNoTracking()
                where shippingRequest.Id == input.ShippingRequestId 
                from normalPricePackage in _normalPricePackageRepository
                    .GetAll().Where(x=> x.AppendixId.HasValue && x.Appendix.IsActive && x.Appendix.Status == AppendixStatus.Confirmed)
                from appendix in _appendixRepository.GetAll().Where(x=> x.Id == normalPricePackage.AppendixId)
                where normalPricePackage.TrucksTypeId == shippingRequest.TrucksTypeId &&
                      (isTmsOrHost || normalPricePackage.TenantId == AbpSession.TenantId) &&
                      normalPricePackage.OriginCityId == shippingRequest.OriginCityId
                      && shippingRequest.ShippingRequestDestinationCities.Any(c =>
                          c.CityId == normalPricePackage.DestinationCityId)
                select normalPricePackage)
                .ProjectTo<TmsPricePackageForViewDto>(AutoMapperConfigurationProvider).ToListAsync();

            matchedPricePackages.AddRange(matchedNormalPricePackage);
            
            var pageResult = matchedPricePackages.Skip(input.SkipCount)
                .Take(input.MaxResultCount).ToList();

            var pricePackagesPricingLookup = (from pricePackageDto in pageResult
                let hasNormalDirectRequest = _directRequestRepository.GetAll().Any(x =>
                    x.ShippingRequestId == input.ShippingRequestId &&
                    x.CarrierTenantId == pricePackageDto.CompanyTenantId)
                from pricePackageOffer in _tmsOfferRepository.GetAllIncluding(x => x.DirectRequest, x => x.PriceOffer)
                    .AsNoTracking().Include(x => x.TmsPricePackage).Include(x => x.NormalPricePackage).DefaultIfEmpty()
                where hasNormalDirectRequest ||
                      (pricePackageOffer != null && (pricePackageOffer.TmsPricePackageId == pricePackageDto.Id ||
                                                     pricePackageOffer.NormalPricePackageId == pricePackageDto.Id) &&
                       (!pricePackageOffer.DirectRequestId.HasValue ||
                        pricePackageOffer.DirectRequest.ShippingRequestId == input.ShippingRequestId) &&
                       (!pricePackageOffer.PriceOfferId.HasValue ||
                        pricePackageOffer.PriceOffer.ShippingRequestId == input.ShippingRequestId))
                select new
                {
                    HasDirectRequest = pricePackageOffer.DirectRequestId.HasValue || hasNormalDirectRequest,
                    HasOffer = pricePackageOffer.PriceOfferId.HasValue,
                    pricePackageDto.PricePackageId
                }).ToList();

            var offerId = await _tmsPricePackageOfferManager.GetParentOfferId(input.ShippingRequestId);
            foreach (var item in pageResult)
            {
                var pricingLookupItem = pricePackagesPricingLookup.FirstOrDefault(x => x.PricePackageId == item.PricePackageId);

                if (pricingLookupItem == null) continue;
                
                item.HasOffer = pricingLookupItem.HasOffer;
                item.HasDirectRequest = pricingLookupItem.HasDirectRequest;
                item.IsRequestPriced = offerId != default;
            }
                
            

            return new PagedResultDto<TmsPricePackageForViewDto>()
            {
                Items = pageResult, TotalCount = pageResult.Count
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

        
        public async Task<LoadResult> GetAllForDropdown(GetTmsPricePackagesInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var tmsPricePackages = _tmsPricePackageRepository.GetAll()
                .AsNoTracking().Where(x=> x.DestinationTenantId == input.DestinationTenantId)
                .WhereIf(input.ProposalId.HasValue,x=> !x.ProposalId.HasValue || x.ProposalId == input.ProposalId)
                .WhereIf(!input.ProposalId.HasValue,x=> !x.ProposalId.HasValue)
                .ProjectTo<PricePackageSelectItemDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(tmsPricePackages, input.LoadOptions);
        }
        [AbpAuthorize(AppPermissions.Pages_TmsPricePackages_Create,AppPermissions.Pages_TmsPricePackages_Update)]
        public async Task<ListResultDto<PricePackageSelectItemDto>> GetPricePackagesForCarrierAppendix(int carrierId, int? appendixId)
        {
            var pricePackagesList = await _tmsPricePackageManager.GetPricePackagesForCarrierAppendix(carrierId, appendixId);

            return new ListResultDto<PricePackageSelectItemDto>(pricePackagesList);
        }
        
        
        private async Task CreateOfferAndAcceptOnBehalfOfCarrier(int pricePackageId, long shippingRequestId,bool isTmsPricePackage)
        {
            int? carrierTenantId;
            long carrierUserId;
            
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                if (isTmsPricePackage)
                {
                    carrierTenantId = await (from tmsPricePackage in _tmsPricePackageRepository.GetAll()
                            where tmsPricePackage.Id == pricePackageId
                            select tmsPricePackage.DestinationTenantId).FirstAsync();
                }
                else
                    carrierTenantId = await (
                        from normalPricePackage in _normalPricePackageRepository.GetAll()
                        where normalPricePackage.Id == pricePackageId
                        select normalPricePackage.TenantId).FirstAsync();

                carrierUserId = await _userRepository.GetAll().Where(x =>
                        x.TenantId == carrierTenantId && x.UserName == AbpUserBase.AdminUserName)
                    .Select(x => x.Id).FirstOrDefaultAsync();
            }

            // impersonate the carrier 
            long createdOfferId;
            using (AbpSession.Use(carrierTenantId,carrierUserId))
            {
                var priceOfferDto = await _priceOfferAppService.GetPriceOfferForCreateOrEdit(shippingRequestId, null);

                
                var priceOffer = ObjectMapper.Map<PriceOffer>(priceOfferDto);

                priceOffer.ShippingRequestId = shippingRequestId;
                var priceOfferInput = ObjectMapper.Map<CreateOrEditPriceOfferInput>(priceOffer);

                priceOfferInput.Channel = PriceOfferChannel.DirectRequest;
                createdOfferId = await _priceOfferAppService.CreateOrEdit(priceOfferInput);
                _priceOfferRepository.Update(createdOfferId, x => x.CreatorUserId = carrierUserId);
            }

            await _priceOfferAppService.Accept(createdOfferId);
        }
    }
}