using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Cities;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.PriceOffers;
using TACHYON.PricePackages.Dto.NormalPricePackage;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestVases;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Vases;

namespace TACHYON.PricePackages
{
    [AbpAuthorize]
    public class NormalPricePackagesAppService : TACHYONAppServiceBase, INormalPricePackageAppService
    {
        private readonly IRepository<NormalPricePackage> _pricePackageRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly NormalPricePackageManager _normalPricePackageManager;
        private readonly IShippingRequestDirectRequestAppService _shippingRequestDirectRequestAppService;
        private readonly IRepository<City> _lookup_cityRepository;
        private readonly IRepository<TransportType> _lookup_transportTypeRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        public NormalPricePackagesAppService(
            IRepository<NormalPricePackage> pricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            NormalPricePackageManager normalPricePackageManager,
            IShippingRequestDirectRequestAppService shippingRequestDirectRequestAppService,
            IRepository<City> lookup_cityRepository,
            IRepository<TransportType> lookup_transportTypeRepository,
            IRepository<TrucksType, long> lookup_trucksTypeRepository,
            IRepository<Tenant> tenantRepository)
        {
            _pricePackageRepository = pricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestDirectRequestAppService = shippingRequestDirectRequestAppService;
            _normalPricePackageManager = normalPricePackageManager;
            _lookup_cityRepository = lookup_cityRepository;
            _lookup_transportTypeRepository = lookup_transportTypeRepository;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
            _tenantRepository = tenantRepository;
        }

        #region LookUps 
        public async Task<List<SelectItemDto>> GetAllTranspotTypesForTableDropdown()
        {
            return await _lookup_transportTypeRepository.GetAll().AsNoTracking()
               .Select(x => new SelectItemDto { DisplayName = x.DisplayName ?? "", Id = x.Id.ToString() }).ToListAsync();

        }
        public async Task<List<SelectItemDto>> GetAllTruckTypesForTableDropdown(int transpotTypeId)
        {
            return await _lookup_trucksTypeRepository.GetAll().AsNoTracking()
                .Where(c => c.TransportTypeId == transpotTypeId)
               .Select(p => new SelectItemDto { DisplayName = p.DisplayName ?? "", Id = p.Id.ToString() }).ToListAsync();

        }
        public async Task<List<SelectItemDto>> GetAllCitiesForTableDropdown()
        {
            return await _lookup_cityRepository.GetAll().AsNoTracking()
                .Select(t => new SelectItemDto { DisplayName = t.DisplayName ?? "", Id = t.Id.ToString() }).ToListAsync();

        }
        public async Task<List<SelectItemDto>> GetCarriers()
        {
            return await _tenantRepository
                            .GetAll().AsNoTracking().OrderBy("id desc")
                            .Select(r => new SelectItemDto
                            {
                                Id = r.Id.ToString(),
                                DisplayName = r.Name,
                            }).ToListAsync();
        }
        #endregion

        #region Crud Opiration
        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages_Edit)]
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.NormalPricePackages, RequiresAll = true)]
        public async Task<CreateOrEditNormalPricePackageDto> GetNormalPricePackageForEdit(int id)
        {
            var pricePackage = await _pricePackageRepository.FirstOrDefaultAsync(id);
            if (pricePackage == null) throw new UserFriendlyException(L("ThePricePackageWasNotExists"));

            return ObjectMapper.Map<CreateOrEditNormalPricePackageDto>(pricePackage);
        }
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.NormalPricePackages, RequiresAll = true)]
        public async Task CreateOrEdit(CreateOrEditNormalPricePackageDto input)
        {
            var nameIsExsist = await _normalPricePackageManager.CheckIfNamePricePackageIsExist(input.DisplayName, input.Id);
            if (nameIsExsist) throw new UserFriendlyException(L("ThePricePackageNameIsExists"));

            if (!input.Id.HasValue)
                await Create(input);
            else
                await Update(input);
        }
        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages_Create)]
        protected virtual async Task Create(CreateOrEditNormalPricePackageDto input)
        {
            var pricePackage = ObjectMapper.Map<NormalPricePackage>(input);

            await _pricePackageRepository.InsertAndGetIdAsync(pricePackage);
            pricePackage.PricePackageId = _normalPricePackageManager.GeneratePricePackageReferanceNumber(pricePackage.Id, pricePackage.IsMultiDrop, pricePackage.CreationTime);

        }
        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages_Edit)]
        protected virtual async Task Update(CreateOrEditNormalPricePackageDto input)
        {
            var pricePackage = await _pricePackageRepository.FirstOrDefaultAsync(input.Id.Value);

            if (pricePackage == null) throw new UserFriendlyException(L("ThePricePackageWasNotExists"));

            if (pricePackage.IsMultiDrop != input.IsMultiDrop)
            {
                pricePackage.PricePackageId = _normalPricePackageManager.GeneratePricePackageReferanceNumber(pricePackage.Id, input.IsMultiDrop, pricePackage.CreationTime);
                if (!input.IsMultiDrop) input.PricePerExtraDrop = null;

            }

            ObjectMapper.Map(input, pricePackage);

        }
        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages_Delete)]
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.NormalPricePackages, RequiresAll = true)]
        public async Task Delete(EntityDto input)
        {
            var pricePackage = await GetPricePackageFromDB(input.Id);
            if (pricePackage == null) throw new UserFriendlyException(L("ThePricePackageWasNotExists"));
            await _pricePackageRepository.DeleteAsync(input.Id);
        }
        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages)]
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer)]
        public async Task<LoadResult> GetAll(GetAllNormalPricePackagesInput input)
        {
            if (!await IsEnabledAsync(AppFeatures.NormalPricePackages)) throw new UserFriendlyException(L("YouCannotAccessThisPage"));

            DisableTenancyFilters();
            var tenentId = AbpSession.TenantId;
            var filteredPricePackages = _pricePackageRepository
                .GetAll().AsNoTracking()
                .WhereIf(tenentId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), p => p.TenantId == tenentId)
                .ProjectTo<NormalPricePackageDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(filteredPricePackages, input.LoadOptions);
        }
        public async Task<NormalPricePackageDto> GetNormalPricePackage(int id)
        {
            DisableTenancyFilters();
            var pricePackage = await GetPricePackageFromDB(id);
            if (pricePackage == null) throw new UserFriendlyException(L("ThePricePackageWasNotExists"));

            return ObjectMapper.Map<NormalPricePackageDto>(pricePackage);
        }

        private async Task<NormalPricePackage> GetPricePackageFromDB(int id)
        {
            var tenentId = AbpSession.TenantId;
            var pricePackage = await _pricePackageRepository
                .GetAllIncluding(x => x.TrucksTypeFk, c => c.OriginCityFK, f => f.DestinationCityFK).AsNoTracking()
                .WhereIf(tenentId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), p => p.TenantId == tenentId)
                .WhereIf(!tenentId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .FirstOrDefaultAsync(p => p.Id == id);
            return pricePackage;
        }

        public async Task<bool> CheckIfPricePackageNameAvailable(CheckIfPricePackageNameAvailableDto input)
        {
            return !await _normalPricePackageManager.CheckIfNamePricePackageIsExist(input.Name, input.Id);
        }
        #endregion

        #region ShippingRequestPricePackage 
        public async Task<PagedResultDto<PricePackageForRequestDto>> GetMatchingPricePackagesForRequest(GetAllPricePackagesForRequestInput input)
        {
            DisableTenancyFilters();
            var shippingRequest = await _shippingRequestRepository.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == input.ShippingRequestId);

            if (shippingRequest == null) throw new UserFriendlyException("TheShippingRequestDoesNotExist");

            var filteredPricePackages = _pricePackageRepository
                .GetAll().AsNoTracking()
                .WhereIf(input.CarrierId.HasValue, x => x.TenantId == input.CarrierId)
                .Where(p => p.TrucksTypeId == shippingRequest.TrucksTypeId
                && p.OriginCityId == shippingRequest.OriginCityId
                && p.DestinationCityId == shippingRequest.DestinationCityId);

            var pagedAndFilteredPricePackages = filteredPricePackages
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);

            var totalCount = await filteredPricePackages.CountAsync();

            var pricePackages = await pagedAndFilteredPricePackages
                .Select(p => new PricePackageForRequestDto
                {
                    Id = p.Id,
                    DisplayName = p.DisplayName,
                    PricePackageId = p.PricePackageId,
                    TruckType = p.TrucksTypeFk.DisplayName,
                    Destination = p.DestinationCityFK.DisplayName,
                    Origin = p.OriginCityFK.DisplayName,
                    CarrierName = p.Tenant.Name,
                    CarrierRate = p.Tenant.Rate,
                    CarrierTenantId = p.TenantId
                }).ToListAsync();

            return new PagedResultDto<PricePackageForRequestDto>(
                totalCount,
                pricePackages
            );

        }
        public async Task<PricePackageOfferDto> GetPricePackageOffer(int pricePackageOfferId, long shippingRequestId)
        {
            return await _normalPricePackageManager.GetPricePackageOffer(pricePackageOfferId, shippingRequestId);
        }
        public async Task<PricePackageOfferDto> GetPricePackageOfferForHandle(int pricePackageId, long shippingRequestId)
        {
            return await _normalPricePackageManager.GetPricePackageOfferDto(pricePackageId, shippingRequestId);
        }
        public async Task HandlePricePackageOfferToCarrier(int pricePackageId, long shippingRequestId)
        {
            var directRequestInput = await _normalPricePackageManager.GetDirectRequestToHandleByPricePackage(pricePackageId, shippingRequestId);
            await _shippingRequestDirectRequestAppService.Create(directRequestInput);
        }
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task<ShippingRequestDirectRequestStatus> AcceptPricePackageOffer(int pricePackageOfferId)
        {
            return await _normalPricePackageManager.AcceptPricePackageOffer(pricePackageOfferId);
        }
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task<long> SendPriceOfferByPricePackage(int pricePackageId, long shippingRequestId)
        {
            return await _normalPricePackageManager.SendPriceOfferByPricePackage(pricePackageId, shippingRequestId);
        }
        #endregion
    }
}