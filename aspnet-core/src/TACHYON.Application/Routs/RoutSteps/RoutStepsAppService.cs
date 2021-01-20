using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using TACHYON.AddressBook;
using TACHYON.Authorization;
using TACHYON.Cities;
using TACHYON.Dto;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Routs.RoutSteps.Exporting;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Routs.RoutPoints.Dtos;
using NUglify.Helpers;
using Abp.Application.Features;
using TACHYON.Features;

namespace TACHYON.Routs.RoutSteps
{
    [AbpAuthorize(AppPermissions.Pages_RoutSteps)]
    [RequiresFeature(AppFeatures.Shipper)]
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


        public RoutStepsAppService(IRepository<RoutStep, long> routStepRepository, IRoutStepsExcelExporter routStepsExcelExporter, IRepository<City, int> lookup_cityRepository, IRepository<Route, int> lookup_routeRepository, IRepository<Facility, long> lookupFacilityRepository, IRepository<GoodsDetail, long> lookupGoodsDetailRepository, IRepository<TrailerType, int> lookupTrailerTypeRepository, IRepository<TrucksType, long> lookupTrucksTypeRepository)
        {
            _routStepRepository = routStepRepository;
            _routStepsExcelExporter = routStepsExcelExporter;
            _lookup_cityRepository = lookup_cityRepository;
            _lookup_routeRepository = lookup_routeRepository;
            _lookup_FacilityRepository = lookupFacilityRepository;
            _lookup_goodsDetailRepository = lookupGoodsDetailRepository;
            _lookup_trailerTypeRepository = lookupTrailerTypeRepository;
            _lookup_trucksTypeRepository = lookupTrucksTypeRepository;
        }

        public async Task<PagedResultDto<GetRoutStepForViewDto>> GetAll(GetAllRoutStepsInput input)
        {

            var filteredRoutSteps = _routStepRepository.GetAll()
                //.Include(e => e.TrucksTypeFk)
               // .Include(e => e.TrailerTypeFk)
                .Include(e => e.SourceRoutPointFk)
                .ThenInclude(e=>e.CityFk)
                .Include(e => e.DestinationRoutPointFk)
                .ThenInclude(e=>e.CityFk)
                //.Include(e => e.DestinationCityFk)
                .Where(e=>e.ShippingRequestId==input.ShippingRequestId)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.GoodsDetailNameFilter), e => e.GoodsDetailFk != null && e.GoodsDetailFk.Name == input.GoodsDetailNameFilter)

                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                   // e.SourceRoutPointFk.Latitude.Contains(input.Filter) || e.DestinationRoutPointFk.Latitude.Contains(input.Filter) ||
                    //e.SourceRoutPointFk.Longitude.Contains(input.Filter) || e.DestinationRoutPointFk.Longitude.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.LatitudeFilter), e => e.SourceRoutPointFk.Latitude == input.LatitudeFilter || e.DestinationRoutPointFk.Latitude==input.LatitudeFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.LongitudeFilter), e => e.SourceRoutPointFk.Longitude == input.LongitudeFilter || e.DestinationRoutPointFk.Longitude==input.LongitudeFilter)
                .WhereIf(input.MinOrderFilter != null, e => e.Order >= input.MinOrderFilter)
                .WhereIf(input.MaxOrderFilter != null, e => e.Order <= input.MaxOrderFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter), e =>  e.SourceRoutPointFk.CityFk.DisplayName == input.CityDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayName2Filter),e=>  e.DestinationRoutPointFk.CityFk.DisplayName == input.CityDisplayName2Filter);


            var pagedAndFilteredRoutSteps = filteredRoutSteps
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var routSteps = from o in pagedAndFilteredRoutSteps.ToList()
                            join o1 in _lookup_cityRepository.GetAll() on o.SourceRoutPointFk.CityId equals o1.Id into j1
                            from s1 in j1.DefaultIfEmpty()

                            join o2 in _lookup_cityRepository.GetAll() on o.DestinationRoutPointFk.CityId equals o2.Id into j2
                            from s2 in j2.DefaultIfEmpty()

                            //join o4 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o4.Id into j4
                            //from s4 in j4.DefaultIfEmpty()

                           // join o5 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o5.Id into j5
                           // from s5 in j5.DefaultIfEmpty()

                            //join o6 in _lookup_goodsDetailRepository.GetAll() on o.GoodsDetailId equals o6.Id into j6
                            //from s6 in j6.DefaultIfEmpty()

                            select new GetRoutStepForViewDto()
                            {
                                RoutStep = new RoutStepDto
                                {
                                    DisplayName = o.DisplayName,
                                    //Latitude = o.Latitude,
                                    //Longitude = o.Longitude,
                                    Order = o.Order,
                                    Id = o.Id
                                },
                               // CityDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                               // CityDisplayName2 = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                              //  TrucksTypeDisplayName = s4 == null || s4.DisplayName == null ? "" : s4.DisplayName.ToString(),
                               // TrailerTypeDisplayName = s5 == null || s5.DisplayName == null ? "" : s5.DisplayName.ToString(),
                               // GoodsDetailName = s6 == null || s6.Name == null ? "" : s6.Name.ToString(),
                               SourceRoutPointDto=new GetRoutPointForViewDto
                               {
                                   CityName=o.SourceRoutPointFk.CityFk.DisplayName,
                                   FacilityName=o.SourceRoutPointFk.FacilityFk.Name,
                                   PickingTypeDisplayName=o.SourceRoutPointFk.PickingTypeFk.DisplayName,
                                   RoutPointDto=ObjectMapper.Map<RoutPointDto>(o.SourceRoutPointFk),
                                   RoutPointGoodsDetailsList=ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(o.SourceRoutPointFk),
                               },
                                DestinationRoutPointDto = new GetRoutPointForViewDto
                                {
                                    CityName = o.DestinationRoutPointFk.CityFk.DisplayName,
                                    FacilityName = o.DestinationRoutPointFk.FacilityFk.Name,
                                    PickingTypeDisplayName = o.DestinationRoutPointFk.PickingTypeFk.DisplayName,
                                    RoutPointDto = ObjectMapper.Map<RoutPointDto>(o.DestinationRoutPointFk),
                                    RoutPointGoodsDetailsList = ObjectMapper.Map<List<RoutPointGoodsDetailDto>>(o.DestinationRoutPointFk),
                                }
                            };

            var totalCount = await filteredRoutSteps.CountAsync();

            return new PagedResultDto<GetRoutStepForViewDto>(
                totalCount,
                 routSteps.ToList()
            );
        }

        public async Task<GetRoutStepForViewDto> GetRoutStepForView(long id)
        {
            var routStep = await _routStepRepository.GetAsync(id);

            var output = new GetRoutStepForViewDto {
                RoutStep = ObjectMapper.Map<RoutStepDto>(routStep),
                SourceRoutPointDto= ObjectMapper.Map<GetRoutPointForViewDto>(routStep.SourceRoutPointFk),
                DestinationRoutPointDto= ObjectMapper.Map<GetRoutPointForViewDto>(routStep.DestinationRoutPointFk)
            };

            if (output.RoutStep.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync(output.RoutStep.TrucksTypeId.Value);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.RoutStep.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.RoutStep.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            //if (output.RoutStep.GoodsDetailId != null)
            //{
            //    var _lookupGoodsDetail = await _lookup_goodsDetailRepository.FirstOrDefaultAsync((long)output.RoutStep.GoodsDetailId);
            //    output.GoodsDetailName = _lookupGoodsDetail?.Name?.ToString();
            //}

            //if (output.SourceRoutPointDto.RoutPointDto.CityId != null)
            //{
            //    var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.SourceRoutPointDto.RoutPointDto.CityId);
            //    output.SourceRoutPointDto.CityName = _lookupCity?.DisplayName?.ToString();
            //}

            //if (output.DestinationRoutPointDto.RoutPointDto.CityId != null)
            //{
            //    var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.SourceRoutPointDto.RoutPointDto.CityId);
            //    output.DestinationRoutPointDto.CityName = _lookupCity?.DisplayName?.ToString();
            //}

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_RoutSteps_Edit)]
        public async Task<GetRoutStepForEditOutput> GetRoutStepForEdit(EntityDto<long> input)
        {
            var routStep = await _routStepRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetRoutStepForEditOutput { RoutStep = ObjectMapper.Map<CreateOrEditRoutStepDto>(routStep)
                ,SourceRoutPointDto=ObjectMapper.Map<GetRoutPointForViewDto>(routStep.SourceRoutPointFk)
                ,DestinationRoutPointDto= ObjectMapper.Map<GetRoutPointForViewDto>(routStep.DestinationRoutPointFk)};

            //if (output.SourceRoutPointDto.RoutPointDto.CityId != null)
            //{
            //    var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.SourceRoutPointDto.RoutPointDto.CityId);
            //    output.SourceRoutPointDto.CityName = _lookupCity?.DisplayName?.ToString();
            //}

            //if (output.DestinationRoutPointDto.RoutPointDto.CityId != null)
            //{
            //    var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.SourceRoutPointDto.RoutPointDto.CityId);
            //    output.DestinationRoutPointDto.CityName = _lookupCity?.DisplayName?.ToString();
            //}


            if (output.RoutStep.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync(output.RoutStep.TrucksTypeId.Value);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.RoutStep.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.RoutStep.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            //if (output.RoutStep.GoodsDetailId != null)
            //{
            //    var _lookupGoodsDetails = await _lookup_goodsDetailRepository.FirstOrDefaultAsync((int)output.RoutStep.GoodsDetailId);
            //    output.RoutStep.CreateOrEditGoodsDetailDto = ObjectMapper.Map<CreateOrEditGoodsDetailDto>(_lookupGoodsDetails);
            //}

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

            int? tenantId = AbpSession.TenantId;
            if (tenantId != null)
            {
                routStep.TenantId = (int)tenantId;
                routStep.SourceRoutPointFk.TenantId = (int)tenantId;
                routStep.DestinationRoutPointFk.TenantId = (int)tenantId;
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


        public async Task<List<FacilityForDropdownDto>> GetAllFacilitiesForDropdown()
        {
            return await _lookup_FacilityRepository.GetAll()
                .Select(x => new FacilityForDropdownDto { Id = x.Id, DisplayName = x.Name }).ToListAsync();
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<SelectItemDto>> GetAllTrucksTypeForTableDropdown()
        {
            return await _lookup_trucksTypeRepository.GetAll()
                .Select(trucksType => new SelectItemDto { Id = trucksType.Id.ToString(), DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString() }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<SelectItemDto>> GetAllTrailerTypeForTableDropdown()
        {
            return await _lookup_trailerTypeRepository.GetAll()
                .Select(trailerType => new SelectItemDto
                {
                    Id = trailerType.Id.ToString(),
                    DisplayName = trailerType == null || trailerType.DisplayName == null ? "" : trailerType.DisplayName.ToString()
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
                        DisplayName = goodsDetail == null || goodsDetail.GoodCategoryFk.DisplayName == null ? "" : goodsDetail.GoodCategoryFk.DisplayName.ToString()
                    }).ToListAsync();
            }

        }



    }
}