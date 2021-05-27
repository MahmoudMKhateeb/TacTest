using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Offers.Dtos;
using TACHYON.Offers.Exporting;
using TACHYON.Routs;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Offers
{
    [AbpAuthorize(AppPermissions.Pages_Offers)]
    public class OffersAppService : TACHYONAppServiceBase, IOffersAppService
    {
        private readonly IRepository<Offer> _offerRepository;
        private readonly IOffersExcelExporter _offersExcelExporter;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<TrailerType, int> _lookup_trailerTypeRepository;
        private readonly IRepository<GoodCategory, int> _lookup_goodCategoryRepository;
        private readonly IRepository<Route, int> _lookup_routeRepository;


        public OffersAppService(IRepository<Offer> offerRepository, IOffersExcelExporter offersExcelExporter, IRepository<TrucksType, long> lookup_trucksTypeRepository, IRepository<TrailerType, int> lookup_trailerTypeRepository, IRepository<GoodCategory, int> lookup_goodCategoryRepository, IRepository<Route, int> lookup_routeRepository)
        {
            _offerRepository = offerRepository;
            _offersExcelExporter = offersExcelExporter;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
            _lookup_trailerTypeRepository = lookup_trailerTypeRepository;
            _lookup_goodCategoryRepository = lookup_goodCategoryRepository;
            _lookup_routeRepository = lookup_routeRepository;

        }

        public async Task<PagedResultDto<GetOfferForViewDto>> GetAll(GetAllOffersInput input)
        {

            if (await FeatureChecker.IsEnabledAsync(AppFeatures.OffersMarketPlace))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return await GetAllPagedResultDto(input);
                }
            }
            else
            {
                return await GetAllPagedResultDto(input);

            }
        }

        protected virtual async Task<PagedResultDto<GetOfferForViewDto>> GetAllPagedResultDto(GetAllOffersInput input)
        {
            var filteredOffers = _offerRepository.GetAll()
                        .Include(e => e.TrucksTypeFk)
                        .Include(e => e.TrailerTypeFk)
                        .Include(e => e.GoodCategoryFk)
                        .ThenInclude(e=>e.Translations)
                        .Include(e => e.RouteFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.Translations.Any(x=>x.TranslatedDisplayName.Contains(input.TrucksTypeDisplayNameFilter)))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GoodCategoryDisplayNameFilter), e => e.GoodCategoryFk != null && e.GoodCategoryFk.Translations.Any(x=>x.DisplayName.Contains(input.GoodCategoryDisplayNameFilter)))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RouteDisplayNameFilter), e => e.RouteFk != null && e.RouteFk.DisplayName == input.RouteDisplayNameFilter);

            var pagedAndFilteredOffers = filteredOffers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var offers = from o in await pagedAndFilteredOffers.ToListAsync()
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_goodCategoryRepository.GetAll().Include(x=>x.Translations) on o.GoodCategoryId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_routeRepository.GetAll() on o.RouteId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetOfferForViewDto()
                         {
                             Offer = new OfferDto
                             {
                                 DisplayName = o.DisplayName,
                                 Description = o.Description,
                                 Price = o.Price,
                                 Id = o.Id
                             },
                             TrucksTypeDisplayName ="", //s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                             TrailerTypeDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(o.GoodCategoryFk).DisplayName,//s3 == null || s3.DisplayName == null ? "" : s3.DisplayName.ToString(),
                             RouteDisplayName = s4 == null || s4.DisplayName == null ? "" : s4.DisplayName.ToString()
                         };

            var totalCount = await filteredOffers.CountAsync();

            return new PagedResultDto<GetOfferForViewDto>(
                totalCount,
                 offers.ToList()
            );
        }

        public async Task<GetOfferForViewDto> GetOfferForView(int id)
        {
            var offer = await _offerRepository.GetAsync(id);

            var output = new GetOfferForViewDto { Offer = ObjectMapper.Map<OfferDto>(offer) };

            if (output.Offer.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync(output.Offer.TrucksTypeId.Value);
                output.TrucksTypeDisplayName = ObjectMapper.Map<TrucksTypeDto>(_lookupTrucksType)?.TranslatedDisplayName;//_lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.Offer.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.Offer.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            if (output.Offer.GoodCategoryId != null)
            {
                var _lookupGoodCategory = await _lookup_goodCategoryRepository.GetAllIncluding(x=>x.Translations).FirstOrDefaultAsync(x=>x.Id == (int)output.Offer.GoodCategoryId);
                output.GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(_lookupGoodCategory).DisplayName;//_lookupGoodCategory?.DisplayName?.ToString();
            }

            if (output.Offer.RouteId != null)
            {
                var _lookupRoute = await _lookup_routeRepository.FirstOrDefaultAsync((int)output.Offer.RouteId);
                output.RouteDisplayName = _lookupRoute?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Offers_Edit)]
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task<GetOfferForEditOutput> GetOfferForEdit(EntityDto input)
        {
            var offer = await _offerRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetOfferForEditOutput { Offer = ObjectMapper.Map<CreateOrEditOfferDto>(offer) };

            if (output.Offer.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync(output.Offer.TrucksTypeId.Value);
                output.TrucksTypeDisplayName = ObjectMapper.Map<TrucksTypeDto>(_lookupTrucksType)?.TranslatedDisplayName;
            }

            if (output.Offer.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.Offer.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            if (output.Offer.GoodCategoryId != null)
            {
                var _lookupGoodCategory = await _lookup_goodCategoryRepository.GetAllIncluding(x=>x.Translations).FirstOrDefaultAsync(x=>x.Id == (int)output.Offer.GoodCategoryId);
                output.GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(_lookupGoodCategory).DisplayName ;
            }

            if (output.Offer.RouteId != null)
            {
                var _lookupRoute = await _lookup_routeRepository.FirstOrDefaultAsync((int)output.Offer.RouteId);
                output.RouteDisplayName = _lookupRoute?.DisplayName?.ToString();
            }

            return output;
        }

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CreateOrEdit(CreateOrEditOfferDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Offers_Create)]
        [RequiresFeature(AppFeatures.Carrier)]
        protected virtual async Task Create(CreateOrEditOfferDto input)
        {
            var offer = ObjectMapper.Map<Offer>(input);


            if (AbpSession.TenantId != null)
            {
                offer.TenantId = (int)AbpSession.TenantId;
            }


            await _offerRepository.InsertAsync(offer);
        }

        [AbpAuthorize(AppPermissions.Pages_Offers_Edit)]
        [RequiresFeature(AppFeatures.Carrier)]
        protected virtual async Task Update(CreateOrEditOfferDto input)
        {
            var offer = await _offerRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, offer);
        }

        [AbpAuthorize(AppPermissions.Pages_Offers_Delete)]
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task Delete(EntityDto input)
        {
            await _offerRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetOffersToExcel(GetAllOffersForExcelInput input)
        {

            var filteredOffers = _offerRepository.GetAll()
                        .Include(e => e.TrucksTypeFk)
                        .Include(e => e.TrailerTypeFk)
                        .Include(e => e.GoodCategoryFk)
                        .ThenInclude(e=>e.Translations)
                        .Include(e => e.RouteFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter)
                        .WhereIf(input.MinPriceFilter != null, e => e.Price >= input.MinPriceFilter)
                        .WhereIf(input.MaxPriceFilter != null, e => e.Price <= input.MaxPriceFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.Translations.Any(x=>x.TranslatedDisplayName.Contains(input.TrucksTypeDisplayNameFilter)))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GoodCategoryDisplayNameFilter), e => e.GoodCategoryFk != null && e.GoodCategoryFk.Translations.Any(x=>x.DisplayName.Contains(input.GoodCategoryDisplayNameFilter)))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RouteDisplayNameFilter), e => e.RouteFk != null && e.RouteFk.DisplayName == input.RouteDisplayNameFilter);

            var query = (from o in await filteredOffers.ToListAsync()
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_goodCategoryRepository.GetAll() on o.GoodCategoryId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_routeRepository.GetAll() on o.RouteId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetOfferForViewDto()
                         {
                             Offer = new OfferDto
                             {
                                 DisplayName = o.DisplayName,
                                 Description = o.Description,
                                 Price = o.Price,
                                 Id = o.Id
                             },
                             TrucksTypeDisplayName =ObjectMapper.Map<TrucksTypeDto>(o.TrucksTypeFk).TranslatedDisplayName, //s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                             TrailerTypeDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             GoodCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(o.GoodCategoryFk).DisplayName, //s3 == null || s3.DisplayName == null ? "" : s3.DisplayName.ToString(),
                             RouteDisplayName = s4 == null || s4.DisplayName == null ? "" : s4.DisplayName.ToString()
                         });


           // var offerListDtos = await query.ToListAsync();

            return _offersExcelExporter.ExportToFile(query.ToList());
        }


        [AbpAuthorize(AppPermissions.Pages_Offers)]
        public async Task<List<TrucksTypeSelectItemDto>> GetAllTrucksTypeForTableDropdown()
        {
            var list = await _lookup_trucksTypeRepository.GetAll()
                .Include(x => x.Translations).ToListAsync();
            return ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(list);
            //.Select(trucksType => new OfferTrucksTypeLookupTableDto
            //{
            //    Id = trucksType.Id.ToString(),
            //    DisplayName = trucksType == null || ObjectMapper.Map<TrucksTypeDto>(trucksType).TranslatedDisplayName==null ?"" : ""//trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString()
            //}).ToListAsync();

        }

        [AbpAuthorize(AppPermissions.Pages_Offers)]
        public async Task<List<OfferTrailerTypeLookupTableDto>> GetAllTrailerTypeForTableDropdown()
        {
            return await _lookup_trailerTypeRepository.GetAll()
                .Select(trailerType => new OfferTrailerTypeLookupTableDto
                {
                    Id = trailerType.Id,
                    DisplayName = trailerType == null || trailerType.DisplayName == null ? "" : trailerType.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_Offers)]
        public async Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodCategoryForTableDropdown()
        {
            //return await _lookup_goodCategoryRepository.GetAll()
            //    .Select(goodCategory => new OfferGoodCategoryLookupTableDto
            //    {
            //        Id = goodCategory.Id,
            //        //DisplayName = goodCategory == null || goodCategory.DisplayName == null ? "" : goodCategory.DisplayName.ToString()
            //    }).ToListAsync();
            var list = await _lookup_goodCategoryRepository.GetAll()
                .Include(x => x.Translations).ToListAsync();

            return ObjectMapper.Map<List<GetAllGoodsCategoriesForDropDownOutput>>(list);
        }

        [AbpAuthorize(AppPermissions.Pages_Offers)]
        public async Task<List<OfferRouteLookupTableDto>> GetAllRouteForTableDropdown()
        {
            return await _lookup_routeRepository.GetAll()
                .Select(route => new OfferRouteLookupTableDto
                {
                    Id = route.Id,
                    DisplayName = route == null || route.DisplayName == null ? "" : route.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}