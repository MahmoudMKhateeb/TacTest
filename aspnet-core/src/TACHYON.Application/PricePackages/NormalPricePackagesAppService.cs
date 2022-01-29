using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
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
using TACHYON.PricePackages.Dto.NormalPricePackage;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.PricePackages
{
    [AbpAuthorize]
    public class NormalPricePackagesAppService : TACHYONAppServiceBase, INormalPricePackageAppService
    {
        private readonly IRepository<NormalPricePackage> _pricePackageRepository;
        private readonly IRepository<City> _lookup_cityRepository;
        private readonly IRepository<TransportType> _lookup_transportTypeRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        public NormalPricePackagesAppService(IRepository<NormalPricePackage> pricePackageRepository, IRepository<City> lookup_cityRepository, IRepository<TransportType> lookup_transportTypeRepository, IRepository<TrucksType, long> lookup_trucksTypeRepository)
        {
            _pricePackageRepository = pricePackageRepository;
            _lookup_cityRepository = lookup_cityRepository;
            _lookup_transportTypeRepository = lookup_transportTypeRepository;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
        }

        #region Main Endpoints 

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
            var nameIsExsist = await CheckIfNamePricePackageIsExist(input.DisplayName, input.Id);
            if (nameIsExsist) throw new UserFriendlyException(L("ThePricePackageNameIsExists"));

            if (!input.Id.HasValue)
                await Create(input);
            else
                await Update(input);
        }

        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages_Delete)]
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.NormalPricePackages, RequiresAll = true)]
        public async Task Delete(EntityDto input)
        {
            await _pricePackageRepository.DeleteAsync(input.Id);
        }

        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages)]
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer)]
        public async Task<PagedResultDto<NormalPricePackageDto>> GetAll(GetAllNormalPricePackagesInput input)
        {
            if (!await IsEnabledAsync(AppFeatures.NormalPricePackages)) throw new UserFriendlyException(L("YouCannotAccessThisPage"));

            DisableTenancyFilters();
            var tenentId = AbpSession.TenantId;
            var filteredPricePackages = _pricePackageRepository
                .GetAllIncluding(x => x.TrucksTypeFk, c => c.OriginCityFK, f => f.DestinationCityFK).AsNoTracking()
                .WhereIf(tenentId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), p => p.TenantId == tenentId)
                .WhereIf(!tenentId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.DisplayName.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DestinationFilter), e => e.DestinationCityFK.DisplayName.Contains(input.DestinationFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.OriginFilter), e => e.OriginCityFK.DisplayName.Contains(input.OriginFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TruckTypeFilter), e => e.TrucksTypeFk.DisplayName.Contains(input.TruckTypeFilter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.PricePackageIdFilter), e => e.PricePackageId.Contains(input.PricePackageIdFilter));

            var pagedAndFilteredPricePackages = filteredPricePackages
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);

            var pricePackages = ObjectMapper.Map<List<NormalPricePackageDto>>(await pagedAndFilteredPricePackages.ToListAsync());

            var totalCount = await filteredPricePackages.CountAsync();

            return new PagedResultDto<NormalPricePackageDto>(
                totalCount,
                pricePackages
            );
        }

        public async Task<NormalPricePackageDto> GetNormalPricePackage(int id)
        {
            DisableTenancyFilters();
            var tenentId = AbpSession.TenantId;
            var pricePackage = await _pricePackageRepository
                .GetAllIncluding(x => x.TrucksTypeFk, c => c.OriginCityFK, f => f.DestinationCityFK).AsNoTracking()
                .WhereIf(tenentId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), p => p.TenantId == tenentId)
                .WhereIf(!tenentId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), e => true)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pricePackage == null) throw new UserFriendlyException(L("ThePricePackageWasNotExists"));

            return ObjectMapper.Map<NormalPricePackageDto>(pricePackage);
        }
        #endregion

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
        #endregion

        #region Helpers
        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages_Create)]
        protected virtual async Task Create(CreateOrEditNormalPricePackageDto input)
        {
            var pricePackage = ObjectMapper.Map<NormalPricePackage>(input);


            if (AbpSession.TenantId != null)
                pricePackage.TenantId = (int)AbpSession.TenantId;

            await _pricePackageRepository.InsertAndGetIdAsync(pricePackage);
            pricePackage.PricePackageId = GeneratePricePackageReferanceNumber(pricePackage.Id, pricePackage.IsMultiDrop, pricePackage.CreationTime);

        }
        [AbpAuthorize(AppPermissions.Pages_NormalPricePackages_Edit)]
        protected virtual async Task Update(CreateOrEditNormalPricePackageDto input)
        {
            var pricePackage = await _pricePackageRepository.FirstOrDefaultAsync(input.Id.Value);

            if (pricePackage == null) throw new UserFriendlyException(L("ThePricePackageWasNotExists"));

            if (pricePackage.IsMultiDrop != input.IsMultiDrop)
            {
                pricePackage.PricePackageId = GeneratePricePackageReferanceNumber(pricePackage.Id, input.IsMultiDrop, pricePackage.CreationTime);
                if (!input.IsMultiDrop) input.PricePerExtraDrop = null;

            }

            ObjectMapper.Map(input, pricePackage);

        }
        private async Task<bool> CheckIfNamePricePackageIsExist(string pricePackageName, int? id)
        {
            return await _pricePackageRepository.GetAll()
                .WhereIf(id.HasValue, c => c.Id != id)
                .AnyAsync(x => x.DisplayName.ToLower().Equals(pricePackageName.ToLower()));
        }
        private string GeneratePricePackageReferanceNumber(int id, bool isMultipleDrop, DateTime creationDate)
        {
            string routType = isMultipleDrop ? "MUL" : "SDR";
            string formatDate = creationDate.ToString("ddMMyy");
            var referanceId = id + 1000;
            var referanceNumber = "{0}-{1}-{2}";
            return string.Format(referanceNumber, formatDate, routType, referanceId);
        }

        #endregion
    }
}