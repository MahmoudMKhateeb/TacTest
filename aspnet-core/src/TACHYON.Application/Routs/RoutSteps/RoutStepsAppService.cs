using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Dtos;
using TACHYON.Authorization;
using TACHYON.Cities;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Routs.RoutSteps.Exporting;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.Routs.RoutSteps
{
    [AbpAuthorize(AppPermissions.Pages_RoutSteps)]
    [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
    public class RoutStepsAppService : TACHYONAppServiceBase, IRoutStepsAppService
    {
        private readonly IRepository<RoutStep, long> _routStepRepository;
        private readonly IRoutStepsExcelExporter _routStepsExcelExporter;
        private readonly IRepository<City, int> _lookup_cityRepository;
        private readonly IRepository<Route, int> _lookup_routeRepository;
        private readonly IRepository<Facility, long> _lookup_FacilityRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<TrailerType, int> _lookup_trailerTypeRepository;
        private readonly IRepository<GoodsDetail, long> _lookup_goodsDetailRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;


        public RoutStepsAppService(IRepository<RoutStep, long> routStepRepository,
            IRoutStepsExcelExporter routStepsExcelExporter,
            IRepository<City, int> lookup_cityRepository,
            IRepository<Route, int> lookup_routeRepository,
            IRepository<Facility, long> lookupFacilityRepository,
            IRepository<GoodsDetail, long> lookupGoodsDetailRepository,
            IRepository<TrailerType, int> lookupTrailerTypeRepository,
            IRepository<TrucksType, long> lookupTrucksTypeRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository)
        {
            _routStepRepository = routStepRepository;
            _routStepsExcelExporter = routStepsExcelExporter;
            _lookup_cityRepository = lookup_cityRepository;
            _lookup_routeRepository = lookup_routeRepository;
            _lookup_FacilityRepository = lookupFacilityRepository;
            _lookup_goodsDetailRepository = lookupGoodsDetailRepository;
            _lookup_trailerTypeRepository = lookupTrailerTypeRepository;
            _lookup_trucksTypeRepository = lookupTrucksTypeRepository;
            _shippingRequestRepository = shippingRequestRepository;
        }

        public async Task<PagedResultDto<GetRoutStepForViewOutput>> GetAll(GetAllRoutStepsInput input)
        {
            var filteredRoutSteps = _routStepRepository.GetAll()
                //.Include(e => e.TrucksTypeFk)
                // .Include(e => e.TrailerTypeFk)
                .Include(e => e.SourceRoutPointFk)
                .ThenInclude(e => e.FacilityFk)
                .ThenInclude(e => e.CityFk)
                .Include(e => e.SourceRoutPointFk)
                // .ThenInclude(e=>e.RoutPointGoodsDetails)
                .Include(e => e.DestinationRoutPointFk)
                .ThenInclude(e => e.FacilityFk)
                .ThenInclude(e => e.CityFk)
                .Include(e => e.DestinationRoutPointFk)
                // .ThenInclude(e=>e.RoutPointGoodsDetails)
                .Where(e => e.ShippingRequestId == input.ShippingRequestId)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.GoodsDetailNameFilter), e => e.GoodsDetailFk != null && e.GoodsDetailFk.Name == input.GoodsDetailNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),
                    e => e.DisplayName == input.DisplayNameFilter)
                .WhereIf(input.MinOrderFilter != null, e => e.Order >= input.MinOrderFilter)
                .WhereIf(input.MaxOrderFilter != null, e => e.Order <= input.MaxOrderFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter),
                    e => e.SourceRoutPointFk.FacilityFk.CityFk.DisplayName == input.CityDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayName2Filter),
                    e => e.DestinationRoutPointFk.FacilityFk.CityFk.DisplayName == input.CityDisplayName2Filter);


            var pagedAndFilteredRoutSteps = filteredRoutSteps
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var routSteps = from o in pagedAndFilteredRoutSteps.ToList()
                //join o1 in _lookup_cityRepository.GetAll() on o.SourceRoutPointFk.FacilityFk.CityId equals o1.Id into j1
                //from s1 in j1.DefaultIfEmpty()

                //join o2 in _lookup_cityRepository.GetAll() on o.DestinationRoutPointFk.FacilityFk.CityId equals o2.Id into j2
                //from s2 in j2.DefaultIfEmpty()

                //join o4 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o4.Id into j4
                //from s4 in j4.DefaultIfEmpty()

                // join o5 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o5.Id into j5
                // from s5 in j5.DefaultIfEmpty()
                select new GetRoutStepForViewOutput()
                {
                    //RoutStep = new RoutStepDto
                    //{
                    //    DisplayName = o.DisplayName,
                    //    Order = o.Order,
                    //    Id = o.Id
                    //},
                    RoutStep = ObjectMapper.Map<RoutStepDto>(o),
                    SourceRoutPointDto = new GetRoutPointForViewOutput
                    {
                        //PickingTypeDisplayName = o.SourceRoutPointFk.PickingTypeFk?.DisplayName,
                        RoutPointDto = ObjectMapper.Map<RoutPointDto>(o.SourceRoutPointFk),
                        //RoutPointGoodsDetailsList = ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(o.SourceRoutPointFk.RoutPointGoodsDetails),
                        facilityDto = new GetFacilityForViewOutput
                        {
                            Facility = ObjectMapper.Map<FacilityDto>(o.SourceRoutPointFk.FacilityFk),
                            CityDisplayName = o.SourceRoutPointFk.FacilityFk.CityFk.DisplayName,
                            FacilityName = o.SourceRoutPointFk.FacilityFk.Name
                        }
                    },
                    DestinationRoutPointDto = new GetRoutPointForViewOutput
                    {
                        //PickingTypeDisplayName = o.DestinationRoutPointFk.PickingTypeFk?.DisplayName,
                        RoutPointDto = ObjectMapper.Map<RoutPointDto>(o.DestinationRoutPointFk),
                        // RoutPointGoodsDetailsList = ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(o.DestinationRoutPointFk.RoutPointGoodsDetails),
                        facilityDto = new GetFacilityForViewOutput
                        {
                            Facility = ObjectMapper.Map<FacilityDto>(o.SourceRoutPointFk.FacilityFk),
                            CityDisplayName = o.DestinationRoutPointFk.FacilityFk.CityFk.DisplayName,
                            FacilityName = o.DestinationRoutPointFk.FacilityFk.Name
                        }
                    }
                };

            var totalCount = await filteredRoutSteps.CountAsync();

            return new PagedResultDto<GetRoutStepForViewOutput>(
                totalCount,
                routSteps.ToList()
            );
        }

        public async Task<GetRoutStepForViewOutput> GetRoutStepForView(long id)
        {
            var routStep = await _routStepRepository.GetAll()
                .Where(x => x.Id == id)
                .Include(x => x.SourceRoutPointFk)
                .ThenInclude(x => x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .Include(x => x.SourceRoutPointFk)
                .Include(x => x.DestinationRoutPointFk)
                .ThenInclude(x => x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .Include(x => x.DestinationRoutPointFk)
                //.ThenInclude(x => x.PickingTypeFk)
                .FirstOrDefaultAsync();

            var output = new GetRoutStepForViewOutput
            {
                RoutStep = ObjectMapper.Map<RoutStepDto>(routStep),
                SourceRoutPointDto = new GetRoutPointForViewOutput
                {
                    RoutPointDto = ObjectMapper.Map<RoutPointDto>(routStep.SourceRoutPointFk),
                    facilityDto = new GetFacilityForViewOutput
                    {
                        Facility = ObjectMapper.Map<FacilityDto>(routStep.SourceRoutPointFk.FacilityFk),
                        CityDisplayName = routStep.DestinationRoutPointFk.FacilityFk.CityFk.DisplayName,
                        FacilityName = routStep.DestinationRoutPointFk.FacilityFk.Name
                    },
                    // PickingTypeDisplayName = routStep.SourceRoutPointFk.PickingTypeFk?.DisplayName,
                    //RoutPointGoodsDetailsList =ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(routStep.SourceRoutPointFk.RoutPointGoodsDetails)
                },
                DestinationRoutPointDto = new GetRoutPointForViewOutput
                {
                    RoutPointDto = ObjectMapper.Map<RoutPointDto>(routStep.SourceRoutPointFk),
                    facilityDto = new GetFacilityForViewOutput
                    {
                        Facility = ObjectMapper.Map<FacilityDto>(routStep.SourceRoutPointFk.FacilityFk),
                        CityDisplayName = routStep.DestinationRoutPointFk.FacilityFk.CityFk.DisplayName,
                        FacilityName = routStep.DestinationRoutPointFk.FacilityFk.Name
                    },
                    // PickingTypeDisplayName = routStep.SourceRoutPointFk.PickingTypeFk?.DisplayName,
                    //RoutPointGoodsDetailsList = ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(routStep.SourceRoutPointFk.RoutPointGoodsDetails)
                }
            };
            // var FinalOutput = ObjectMapper.Map<GetRoutStepForViewOutput>(routStep);

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_RoutSteps_Edit)]
        public async Task<GetRoutStepForEditOutput> GetRoutStepForEdit(EntityDto<long> input)
        {
            var routStep = await _routStepRepository.GetAll()
                .Where(x => x.Id == input.Id)
                .Include(x => x.SourceRoutPointFk)
                .ThenInclude(x => x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .Include(x => x.SourceRoutPointFk)
                .Include(x => x.DestinationRoutPointFk)
                .ThenInclude(x => x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .Include(x => x.DestinationRoutPointFk)
                .FirstOrDefaultAsync();

            var output = new GetRoutStepForEditOutput
            {
                RoutStep = ObjectMapper.Map<RoutStepDto>(routStep),
                SourceRoutPointDto = new GetRoutPointForViewOutput
                {
                    RoutPointDto = ObjectMapper.Map<RoutPointDto>(routStep.SourceRoutPointFk),
                    facilityDto = new GetFacilityForViewOutput
                    {
                        Facility = ObjectMapper.Map<FacilityDto>(routStep.SourceRoutPointFk.FacilityFk),
                        CityDisplayName = routStep.SourceRoutPointFk.FacilityFk.CityFk.DisplayName,
                        FacilityName = routStep.SourceRoutPointFk.FacilityFk.Name
                    },
                    // PickingTypeDisplayName = routStep.SourceRoutPointFk.PickingTypeFk.DisplayName,
                    //RoutPointGoodsDetailsList = ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(routStep.SourceRoutPointFk.RoutPointGoodsDetails)
                },
                DestinationRoutPointDto = new GetRoutPointForViewOutput
                {
                    RoutPointDto = ObjectMapper.Map<RoutPointDto>(routStep.DestinationRoutPointFk),
                    facilityDto = new GetFacilityForViewOutput
                    {
                        Facility = ObjectMapper.Map<FacilityDto>(routStep.DestinationRoutPointFk.FacilityFk),
                        CityDisplayName = routStep.DestinationRoutPointFk.FacilityFk.CityFk.DisplayName,
                        FacilityName = routStep.DestinationRoutPointFk.FacilityFk.Name
                    },
                    // PickingTypeDisplayName = routStep.DestinationRoutPointFk.PickingTypeFk.DisplayName,
                    //RoutPointGoodsDetailsList = ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(routStep.DestinationRoutPointFk.RoutPointGoodsDetails)
                }
            };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditRoutStepDto input)
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

        [AbpAuthorize(AppPermissions.Pages_RoutSteps_Create)]
        protected virtual async Task Create(CreateOrEditRoutStepDto input)
        {
            var routStep = ObjectMapper.Map<RoutStep>(input);
            // routStep.SourceRoutPointFk.RoutPointGoodsDetails

            int? tenantId = AbpSession.TenantId;
            if (tenantId != null)
            {
                routStep.TenantId = (int)tenantId;
                //    routStep.SourceRoutPointFk.TenantId = (int)tenantId;
                //    routStep.DestinationRoutPointFk.TenantId = (int)tenantId;
            }

            await _routStepRepository.InsertAsync(routStep);
        }

        [AbpAuthorize(AppPermissions.Pages_RoutSteps_Edit)]
        protected virtual async Task Update(CreateOrEditRoutStepDto input)
        {
            var routStep = await _routStepRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, routStep);
        }

        [AbpAuthorize(AppPermissions.Pages_RoutSteps_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _routStepRepository.DeleteAsync(input.Id);
        }

        //public async Task<FileDto> GetRoutStepsToExcel(GetAllRoutStepsForExcelInput input)
        //{

        //    var filteredRoutSteps = _routStepRepository.GetAll()
        //                .Include(e => e.OriginCityFk)
        //                .Include(e => e.DestinationCityFk)
        //                .Include(e => e.RouteFk)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter) || e.Latitude.Contains(input.Filter) || e.Longitude.Contains(input.Filter))
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.LatitudeFilter), e => e.Latitude == input.LatitudeFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.LongitudeFilter), e => e.Longitude == input.LongitudeFilter)
        //                .WhereIf(input.MinOrderFilter != null, e => e.Order >= input.MinOrderFilter)
        //                .WhereIf(input.MaxOrderFilter != null, e => e.Order <= input.MaxOrderFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter), e => e.OriginCityFk != null && e.OriginCityFk.DisplayName == input.CityDisplayNameFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayName2Filter), e => e.DestinationCityFk != null && e.DestinationCityFk.DisplayName == input.CityDisplayName2Filter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.RouteDisplayNameFilter), e => e.RouteFk != null && e.RouteFk.DisplayName == input.RouteDisplayNameFilter);

        //    var query = (from o in filteredRoutSteps
        //                 join o1 in _lookup_cityRepository.GetAll() on o.OriginCityId equals o1.Id into j1
        //                 from s1 in j1.DefaultIfEmpty()

        //                 join o2 in _lookup_cityRepository.GetAll() on o.DestinationCityId equals o2.Id into j2
        //                 from s2 in j2.DefaultIfEmpty()

        //                 join o3 in _lookup_routeRepository.GetAll() on o.RouteId equals o3.Id into j3
        //                 from s3 in j3.DefaultIfEmpty()

        //                 select new GetRoutStepForViewDto()
        //                 {
        //                     RoutStep = new RoutStepDto
        //                     {
        //                         DisplayName = o.DisplayName,
        //                         Latitude = o.Latitude,
        //                         Longitude = o.Longitude,
        //                         Order = o.Order,
        //                         Id = o.Id
        //                     },
        //                     CityDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
        //                     CityDisplayName2 = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
        //                     RouteDisplayName = s3 == null || s3.DisplayName == null ? "" : s3.DisplayName.ToString()
        //                 });


        //    var routStepListDtos = await query.ToListAsync();

        //    return _routStepsExcelExporter.ExportToFile(routStepListDtos);
        //}


        [AbpAuthorize(AppPermissions.Pages_RoutSteps)]
        public async Task<List<RoutStepCityLookupTableDto>> GetAllCityForTableDropdown()
        {
            return await _lookup_cityRepository.GetAll()
                .Select(city => new RoutStepCityLookupTableDto
                {
                    Id = city.Id,
                    DisplayName = city == null || city.DisplayName == null ? "" : city.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_RoutSteps)]
        public async Task<List<RoutStepRouteLookupTableDto>> GetAllRouteForTableDropdown()
        {
            return await _lookup_routeRepository.GetAll()
                .Select(route => new RoutStepRouteLookupTableDto
                {
                    Id = route.Id,
                    DisplayName = route == null || route.DisplayName == null ? "" : route.DisplayName.ToString()
                }).ToListAsync();
        }

        public async Task<List<FacilityForDropdownDto>> GetAllFacilitiesForDropdown(long? shippingRequestId)
        {
            int? shipperId = null;
            // TMS can see any shipper facility in order to select it in Create Trip action 
            if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                if (shippingRequestId != null)
                {
                    DisableTenancyFilters();
                    var sr = await _shippingRequestRepository.GetAsync(shippingRequestId.Value);
                    shipperId = sr.TenantId;
                }
            }

            return await _lookup_FacilityRepository.GetAll()
                .WhereIf(shipperId.HasValue, x => x.TenantId == shipperId.Value)
                .Select(x => new FacilityForDropdownDto
                {
                    Id = x.Id, DisplayName = x.Name, Long = x.Location.X, Lat = x.Location.Y
                }).ToListAsync();
        }

        public async Task<List<FacilityForDropdownDto>> GetAllFacilitiesByCityAndTenantForDropdown(long? shippingRequestId)
        {
            int? shipperId = null;
            var sr = await _shippingRequestRepository.FirstOrDefaultAsync(shippingRequestId.Value);
            int? originCityId = sr.OriginCityId;
            int? destinationCityId = sr.DestinationCityId;
            int? ShippingTypeId = sr.ShippingTypeId;
            // TMS can see any shipper facility in order to select it in Create Trip action 
            if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                if (shippingRequestId != null)
                {
                    DisableTenancyFilters();
                    shipperId = sr.TenantId;
                }
            }

            return await _lookup_FacilityRepository.GetAll().AsNoTracking()
            .WhereIf(shipperId.HasValue, x => x.TenantId == shipperId.Value)
            .WhereIf(ShippingTypeId.HasValue && ShippingTypeId == 1, x => x.CityId == originCityId) //inside city
            .WhereIf(ShippingTypeId.HasValue && ShippingTypeId == 2, x => x.CityId == originCityId || x.CityId == destinationCityId) //between city
            .Select(x => new FacilityForDropdownDto
            {
                Id = x.Id,
                DisplayName = x.Name,
                Long = x.Location.X,
                Lat = x.Location.Y,
                CityId = x.CityId
            }).ToListAsync();
        }

        public async Task<List<FacilityForDropdownDto>> GetAllFacilitiesByCityIdForDropdown(long cityId)
        {
            return await _lookup_FacilityRepository.GetAll()
                .Where(x => x.CityId == cityId)
                .Select(x => new FacilityForDropdownDto
                {
                    Id = x.Id, DisplayName = x.Name, Long = x.Location.X, Lat = x.Location.Y
                }).ToListAsync();
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<TrucksTypeSelectItemDto>> GetAllTrucksTypeForTableDropdown()
        {
            var list = await _lookup_trucksTypeRepository.GetAll()
                .Include(x => x.Translations).ToListAsync();
            return ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(list);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<SelectItemDto>> GetAllTrailerTypeForTableDropdown()
        {
            return await _lookup_trailerTypeRepository.GetAll()
                .Select(trailerType => new SelectItemDto
                {
                    Id = trailerType.Id.ToString(),
                    DisplayName = trailerType == null || trailerType.DisplayName == null
                        ? ""
                        : trailerType.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<SelectItemDto>> GetAllGoodsDetailForTableDropdown()
        {
            //todo i am not sure about disabling this filter here
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                return await _lookup_goodsDetailRepository.GetAll()
                    .Select(goodsDetail => new SelectItemDto
                    {
                        Id = goodsDetail.Id.ToString(),
                        //DisplayName = goodsDetail == null || goodsDetail.GoodCategoryFk.DisplayName == null ? "" : goodsDetail.GoodCategoryFk.DisplayName.ToString()
                    }).ToListAsync();
            }
        }
    }
}