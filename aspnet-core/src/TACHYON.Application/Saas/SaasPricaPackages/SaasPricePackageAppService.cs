

using TACHYON.Trucks.TrucksTypes;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Actors;
using TACHYON.AddressBook;
using TACHYON.Authorization;
using TACHYON.Cities;
using TACHYON.Common;
using TACHYON.Dto;
using TACHYON.PricePackages.Dto;
using TACHYON.Saas.SaasPricePackages;
using TACHYON.Saas.SaasPricePackages.Dto;
using TransportType = TACHYON.Trucks.TruckCategories.TransportTypes.TransportType;
using Abp.Collections.Extensions;
using DevExpress.Data.ODataLinq.Helpers;
using System.Linq.Dynamic.Core;
using TACHYON.Trucks;
using Abp.Runtime.Validation;

namespace TACHYON.Saas.SaasPricaPackages
{
    [AbpAuthorize(AppPermissions.Pages_PricePackages)]
    public class SaasPricePackageAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<SaasPricePackage,long> _saasPricePackageRepository;
        private readonly IRepository<TrucksType,long> _truckTypeRepository;
        private readonly IRepository<TransportType> _transportTypeRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Truck, long> _truckRepository;

        public SaasPricePackageAppService(
            IRepository<SaasPricePackage, long> saasPricePackageRepository, 
            IRepository<TrucksType, long> truckTypeRepository,
            IRepository<City> cityRepository, IRepository<Actor> actorRepository, IRepository<TransportType> transportTypeRepository, IRepository<Truck, long> truckRepository)
        {
            _saasPricePackageRepository = saasPricePackageRepository;
            _truckTypeRepository = truckTypeRepository;
            _cityRepository = cityRepository;
            _actorRepository = actorRepository;
            _transportTypeRepository = transportTypeRepository;
            _truckRepository = truckRepository;
        }

        public async Task<LoadResult> GetAll(LoadOptionsInput input)
        {
           
            await DisableDraftedFilterIfTachyonDealerOrHost();
            
            var pricePackages = _saasPricePackageRepository.GetAll()
           .ProjectTo<SaasPricePackageListDto>(AutoMapperConfigurationProvider);

            return await LoadResultAsync(pricePackages, input.LoadOptions);
        }
    
       
        public async Task CreateOrEdit(CreateOrEditSaasPricePackageDto input)
        {
            
            if (input.Id.HasValue)
            {
                await Update(input);
                return;
            }

            await Create(input);
              
        }
        
        public async Task<SaasPricePackageForViewDto> GetForView(long pricePackageId)
        {
            var pricePackage = await _saasPricePackageRepository.GetAll()
                .Where(x => x.Id == pricePackageId)
                .Select(x=> new SaasPricePackageForViewDto()
                {
                     Id = x.Id,
                     DestinationCityDisplayName=  x.DestinationCity.DisplayName,
                     OriginCityDisplayName =  x.OriginCity.DisplayName,
                     DisplayName =  x.DisplayName,
                     ActorShipperFkCompanyName = x.ActorShipperFk.CompanyName,
                     TrucksTypDisplayName =  x.TrucksTypeFk.DisplayName,
                     ActorShipperPrice = x.ActorShipperPrice,
                     TransportType = x.TransportTypeFk.DisplayName
                }).FirstOrDefaultAsync();

            if (pricePackage == null) throw new EntityNotFoundException(L("NotFound"));

            return pricePackage;
        }

         public async Task<CreateOrEditSaasPricePackageDto> GetForEdit(long id)
        {
           
            
            var pricePackage = await _saasPricePackageRepository
                .FirstOrDefaultAsync(x=> x.Id == id);
            
            if (pricePackage == null) throw new EntityNotFoundException(L("NotFound"));
            
            return ObjectMapper.Map<CreateOrEditSaasPricePackageDto>(pricePackage);
        }

         public async Task Delete(long id)
        {
            var isExist = await _saasPricePackageRepository.GetAll().AnyAsync(x => x.Id == id);

            if (!isExist) throw new EntityNotFoundException(L("NotFound"));

            await _saasPricePackageRepository.DeleteAsync(x => x.Id == id);
        }

        private async Task Update(CreateOrEditSaasPricePackageDto input)
        {
           var saasPricePackage = await _saasPricePackageRepository
               .FirstOrDefaultAsync(x => x.Id == input.Id.Value);
           ObjectMapper.Map(input, saasPricePackage);
        }

        private async Task Create(CreateOrEditSaasPricePackageDto input)
        {
            var createdPricePackage = ObjectMapper.Map<SaasPricePackage>(input);

            var check = await _saasPricePackageRepository.GetAll()
                .Where(x => x.ActorShipperId == input.ActorShipperId)
                .Where(x => x.OriginCityId == input.OriginCityId)
                .Where(x => x.DestinationCityId == input.DestinationCityId)
                .Where(x=> x.ShippingTypeId == input.ShippingTypeId)
                .Where(x=> x.tripLoadingType == input.tripLoadingType)
                .Where(x=> x.RoundTripType == input.RoundTripType)
                .AnyAsync();

            if (check)
            {

                throw new UserFriendlyException("SaasPricePackageDoublicatedValidationMessege");
            }

            await _saasPricePackageRepository.InsertAsync(createdPricePackage);
        }

        public async Task<decimal?> GetForPricing(GetSaasPricePackageForPricingInput input)
        {
            
            //truck type
            long? truckTypeId = null;
            
            if (input.TruckId.HasValue)
            {
                 truckTypeId = (await _truckRepository.GetAll()
                    .Where(x => x.Id == input.TruckId)
                    .FirstOrDefaultAsync()).TrucksTypeId;
            }
           
            
            var p = await  _saasPricePackageRepository.GetAll()
                .WhereIf(input.ActorShipperId.HasValue ,x => x.ActorShipperId == input.ActorShipperId)
                .WhereIf(input.OriginCityId.HasValue ,x => x.OriginCityId == input.OriginCityId)
                .WhereIf(input.DestinationCityId.HasValue ,x => x.DestinationCityId == input.DestinationCityId)
                .WhereIf(truckTypeId.HasValue ,x => x.TruckTypeId == truckTypeId)
                .WhereIf(input.ShippingTypeId.HasValue ,x => x.ShippingTypeId == input.ShippingTypeId)
                //.WhereIf(input.GoodCategoryId.HasValue ,x => x.GoodCategoryId == input.GoodCategoryId)
                .WhereIf(input.tripLoadingType.HasValue ,x => x.tripLoadingType == input.tripLoadingType)
                .WhereIf(input.RoundTripType.HasValue ,x => x.RoundTripType == input.RoundTripType)
                .WhereIf(input.TruckTypeId.HasValue ,x => x.TruckTypeId == input.TruckTypeId)
                .FirstOrDefaultAsync();
            
            if (p != null)
            {
                return p.ActorShipperPrice;
            }

            return null;
        }
        
        


        #region LOOKUPs & Dropdowns
        
        
        public async Task<List<SelectItemDto>> GetAllTruckTypeForDropdown(int? transportTypeId)
        {
            return await _truckTypeRepository.GetAll().AsNoTracking()
                .WhereIf(transportTypeId.HasValue , c => c.TransportTypeId == transportTypeId)
                .Select(p => new SelectItemDto { DisplayName = p.DisplayName ?? "", Id = p.Id.ToString() })
                .ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllTransportTypeForDropdown()
        {
            return await _transportTypeRepository.GetAllIncluding(x => x.Translations)
                .Select(p => new SelectItemDto
                {
                    DisplayName = p.DisplayName,
                    Id = p.Id.ToString()
                }).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllCitiesForDropdown(int? countryId)
        {
            
                return await _cityRepository.GetAll()
                    .WhereIf(countryId.HasValue,x=> x.CountyId == countryId)
                    .Select(city => new SelectItemDto
                    {
                        Id = city.Id.ToString(),
                        DisplayName = city == null ? string.Empty : city.DisplayName,
                        
                    }).ToListAsync();
            
        }
        
        public async Task<List<SelectItemDto>> GetAllActorShippersForDropdown()
        {
            return await _actorRepository.GetAll()
                .Where(x=> x.ActorType == ActorTypesEnum.Shipper).AsNoTracking()
                .Select(p => new SelectItemDto
                {
                    DisplayName = p.CompanyName,
                    Id = p.Id.ToString()
                }).ToListAsync();
        }
        
        #endregion
    }
}