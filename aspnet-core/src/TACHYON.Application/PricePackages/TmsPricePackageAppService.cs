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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages.Dto.TmsPricePackages;
using TACHYON.PricePackages.TmsPricePackages;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
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
        private readonly IShippingRequestDirectRequestAppService _directRequestAppService;
        private readonly PriceOfferManager _priceOfferManager;

        public TmsPricePackageAppService(
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            NormalPricePackageManager normalPricePackageManager,
            ITmsPricePackageManager tmsPricePackageManager, 
            IRepository<NormalPricePackage> normalPricePackageRepository, 
            IRepository<ShippingRequestDirectRequest, long> directRequestRepository,
            IShippingRequestDirectRequestAppService directRequestAppService,
            PriceOfferManager priceOfferManager)
        {
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _tenantRepository = tenantRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _normalPricePackageManager = normalPricePackageManager;
            _tmsPricePackageManager = tmsPricePackageManager;
            _normalPricePackageRepository = normalPricePackageRepository;
            _directRequestRepository = directRequestRepository;
            _directRequestAppService = directRequestAppService;
            _priceOfferManager = priceOfferManager;
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

        public async Task ApplyPricePackage(int pricePackageId,long srId)
        {
            await ApplyPricingOnShippingRequest(pricePackageId, srId);
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

            
            var matchedNormalPricePackage = await (from shippingRequest in _shippingRequestRepository.GetAll()
                where shippingRequest.Id == input.ShippingRequestId
                from normalPricePackage in _normalPricePackageRepository
                    .GetAll().AsNoTracking()
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

            var pricePackagesHasDirectRequest = (from pricePackageDto in pageResult
                join directRequest in _directRequestRepository.GetAll().AsNoTracking()
                on input.ShippingRequestId  equals directRequest.ShippingRequestId 
                where directRequest.CarrierTenantId == pricePackageDto.CompanyTenantId
                    select pricePackageDto.Id).ToList();

            foreach (var item in pageResult)
                item.HasDirectRequest = pricePackagesHasDirectRequest.Any(pricePackageId => pricePackageId == item.Id);
            

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
        
        private async Task SendOfferToShipperByPricePackage(long srId, ShippingRequest shippingRequest,
            TmsPricePackage pricePackage)
        {
            var itemDetails = shippingRequest.ShippingRequestVases?
                .Select(item => new PriceOfferDetailDto() { ItemId = item.Id, Price = 0 }).ToList();
            
            decimal vat = decimal.Parse(await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.TaxVat));
            decimal quantity = shippingRequest.NumberOfTrips;
            decimal itemPriceWithVat = pricePackage.TotalPrice / quantity;
            decimal itemPrice = (itemPriceWithVat * 100) / (vat + 100);

            var priceOfferDto = new CreateOrEditPriceOfferInput()
            {
                ShippingRequestId = srId, ItemPrice = itemPrice, ItemDetails = itemDetails,
                CommissionType = PriceOfferCommissionType.CommissionValue,
                CommissionPercentageOrAddValue = 0,
                VasCommissionType = PriceOfferCommissionType.CommissionValue, VasCommissionPercentageOrAddValue = 0
            };

            var offerId = await _priceOfferManager.CreateOrEdit(priceOfferDto);

            pricePackage.OfferId = offerId;
            pricePackage.Status = PricePackageOfferStatus.SentAndWaitingResponse;
        }

        private async Task AcceptOfferByPricePackage(TmsPricePackage pricePackage)
        {
            if (!pricePackage.OfferId.HasValue) throw new UserFriendlyException(L("ThereIsNoOfferInThisPricePackage"));
                
            await _priceOfferManager.AcceptOffer(pricePackage.OfferId.Value);

            pricePackage.Status = PricePackageOfferStatus.Confirmed;
        }
        
        /// <summary>
        /// this method used for apply the pricing that the shipper/carrier agree it by price package
        /// the stage can be use after price package appendix is confirmed 
        /// </summary>
        /// <param name="pricePackageId"></param>
        /// <param name="srId"></param>
        /// <exception cref="UserFriendlyException"></exception>
        private async Task ApplyPricingOnShippingRequest(int pricePackageId,long srId)
        {
            DisableTenancyFilters();
            
            var pricePackage = await _tmsPricePackageRepository.GetAll().SingleAsync(x=> x.Id == pricePackageId);

            if (pricePackage.DestinationTenantId is null)
                throw new UserFriendlyException(L("PricePackageMustHaveDestinationTenant"));
            
            var shippingRequest = await _shippingRequestRepository.GetAllIncluding(x=> x.ShippingRequestVases)
                .AsNoTracking().SingleAsync(x => x.Id == srId);

            if (pricePackage.ProposalId.HasValue)
            {
                await SendOfferToShipperByPricePackage(srId, shippingRequest, pricePackage);
                await CurrentUnitOfWork.SaveChangesAsync();
                await AcceptOfferByPricePackage(pricePackage);

            } else if (pricePackage.AppendixId.HasValue)
            {
                // send direct request to carrier by price package
                
                await _directRequestAppService.Create(new CreateShippingRequestDirectRequestInput()
                {
                    CarrierTenantId = pricePackage.DestinationTenantId.Value, ShippingRequestId = srId
                });

            }
            else throw new UserFriendlyException(L("PricePackageMustHaveAppendixOrProposal"));

        }
    }
}