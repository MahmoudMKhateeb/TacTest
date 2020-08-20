using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp;
using Abp.Collections.Extensions;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Notifications;
using TACHYON.Routs;
using TACHYON.Routs.RoutSteps;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.Exporting;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.TrucksTypes;
using Abp.Notifications;

namespace TACHYON.Shipping.ShippingRequests
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
    public class ShippingRequestsAppService : TACHYONAppServiceBase, IShippingRequestsAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IShippingRequestsExcelExporter _shippingRequestsExcelExporter;
        private readonly IRepository<TrucksType, Guid> _lookup_trucksTypeRepository;
        private readonly IRepository<TrailerType, int> _lookup_trailerTypeRepository;
        private readonly IRepository<GoodsDetail, long> _lookup_goodsDetailRepository;
        private readonly IRepository<Route, int> _lookup_routeRepository;
        private readonly IAppNotifier _appNotifier;


        public ShippingRequestsAppService(IRepository<ShippingRequest, long> shippingRequestRepository, IShippingRequestsExcelExporter shippingRequestsExcelExporter, IRepository<TrucksType, Guid> lookup_trucksTypeRepository, IRepository<TrailerType, int> lookup_trailerTypeRepository, IRepository<GoodsDetail, long> lookup_goodsDetailRepository, IRepository<Route, int> lookup_routeRepository, IAppNotifier appNotifier)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestsExcelExporter = shippingRequestsExcelExporter;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
            _lookup_trailerTypeRepository = lookup_trailerTypeRepository;
            _lookup_goodsDetailRepository = lookup_goodsDetailRepository;
            _lookup_routeRepository = lookup_routeRepository;
            _appNotifier = appNotifier;
        }

        public async Task<PagedResultDto<GetShippingRequestForViewDto>> GetAll(GetAllShippingRequestsInput input)
        {
            if (await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    input.IsTachyonDeal = true;
                    return await GetAllPagedResultDto(input);
                }
            }
            else
            {
                return await GetAllPagedResultDto(input);

            }
        }

        protected virtual async Task<PagedResultDto<GetShippingRequestForViewDto>> GetAllPagedResultDto(GetAllShippingRequestsInput input)
        {
            var filteredShippingRequests = _shippingRequestRepository.GetAll()
                .Include(e => e.TrucksTypeFk)
                .Include(e => e.TrailerTypeFk)
                .Include(e => e.GoodsDetailFk)
                .Include(e => e.RouteFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                .WhereIf(input.MinVasFilter != null, e => e.Vas >= input.MinVasFilter)
                .WhereIf(input.MaxVasFilter != null, e => e.Vas <= input.MaxVasFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.GoodsDetailNameFilter), e => e.GoodsDetailFk != null && e.GoodsDetailFk.Name == input.GoodsDetailNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.RouteDisplayNameFilter), e => e.RouteFk != null && e.RouteFk.DisplayName == input.RouteDisplayNameFilter)
                .WhereIf(input.IsTachyonDeal.HasValue, e => e.IsTachyonDeal == input.IsTachyonDeal.Value);

            var pagedAndFilteredShippingRequests = filteredShippingRequests
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var shippingRequests = from o in pagedAndFilteredShippingRequests
                                   join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                                   from s1 in j1.DefaultIfEmpty()

                                   join o2 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o2.Id into j2
                                   from s2 in j2.DefaultIfEmpty()

                                   join o3 in _lookup_goodsDetailRepository.GetAll() on o.GoodsDetailId equals o3.Id into j3
                                   from s3 in j3.DefaultIfEmpty()

                                   join o4 in _lookup_routeRepository.GetAll() on o.RouteId equals o4.Id into j4
                                   from s4 in j4.DefaultIfEmpty()

                                   select new GetShippingRequestForViewDto()
                                   {
                                       ShippingRequest = new ShippingRequestDto
                                       {
                                           Vas = o.Vas,
                                           Id = o.Id,
                                           IsBid = o.IsBid,
                                           IsTachyonDeal = o.IsTachyonDeal,
                                           Price = o.Price
                                       },
                                       TrucksTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                                       TrailerTypeDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                                       GoodsDetailName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),
                                       RouteDisplayName = s4 == null || s4.DisplayName == null ? "" : s4.DisplayName.ToString()
                                   };

            var totalCount = await filteredShippingRequests.CountAsync();

            return new PagedResultDto<GetShippingRequestForViewDto>(
                totalCount,
                await shippingRequests.ToListAsync()
            );
        }

        public async Task<GetShippingRequestForViewDto> GetShippingRequestForView(long id)
        {
            ShippingRequest shippingRequest;

            if (await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return await _GetShippingRequestForView(id);

                }
            }
            else
            {
                return await _GetShippingRequestForView(id);
            }

        }
        protected virtual async Task<GetShippingRequestForViewDto> _GetShippingRequestForView(long id)
        {
            var shippingRequest = await _shippingRequestRepository.GetAsync(id);

            var output = new GetShippingRequestForViewDto { ShippingRequest = ObjectMapper.Map<ShippingRequestDto>(shippingRequest) };

            if (output.ShippingRequest.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((Guid)output.ShippingRequest.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.ShippingRequest.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.ShippingRequest.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            if (output.ShippingRequest.GoodsDetailId != null)
            {
                var _lookupGoodsDetail = await _lookup_goodsDetailRepository.FirstOrDefaultAsync((long)output.ShippingRequest.GoodsDetailId);
                output.GoodsDetailName = _lookupGoodsDetail?.Name?.ToString();
            }

            if (output.ShippingRequest.RouteId != null)
            {
                var _lookupRoute = await _lookup_routeRepository.FirstOrDefaultAsync((int)output.ShippingRequest.RouteId);
                output.RouteDisplayName = _lookupRoute?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        public async Task<GetShippingRequestForEditOutput> GetShippingRequestForEdit(EntityDto<long> input)
        {
            ShippingRequest shippingRequest;

            if (await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return await _GetShippingRequestForEdit(input);
                }
            }
            else
            {
                return await _GetShippingRequestForEdit(input);
            }

        }
        protected virtual async Task<GetShippingRequestForEditOutput> _GetShippingRequestForEdit(EntityDto<long> input)
        {
            var shippingRequest = await _shippingRequestRepository.GetAsync(input.Id);

            var output = new GetShippingRequestForEditOutput
            {
                ShippingRequest = ObjectMapper.Map<CreateOrEditShippingRequestDto>(shippingRequest)
            };

            if (output.ShippingRequest.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((Guid)output.ShippingRequest.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.ShippingRequest.TrailerTypeId != null)
            {
                var _lookupTrailerType = await _lookup_trailerTypeRepository.FirstOrDefaultAsync((int)output.ShippingRequest.TrailerTypeId);
                output.TrailerTypeDisplayName = _lookupTrailerType?.DisplayName?.ToString();
            }

            if (output.ShippingRequest.RouteId != null)
            {
                var _lookupRoute = await _lookup_routeRepository.FirstOrDefaultAsync((int)output.ShippingRequest.RouteId);
                output.RouteDisplayName = _lookupRoute?.DisplayName?.ToString();
            }

            return output;
        }

        [RequiresFeature(AppFeatures.ShippingRequest)]
        public async Task CreateOrEdit(CreateOrEditShippingRequestDto input)
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

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Create)]
        protected virtual async Task Create(CreateOrEditShippingRequestDto input)
        {
            var shippingRequest = ObjectMapper.Map<ShippingRequest>(input);
            shippingRequest.GoodsDetailFk = ObjectMapper.Map<GoodsDetail>(input.CreateOrEditGoodsDetailDto);
            shippingRequest.RoutSteps = ObjectMapper.Map<List<RoutStep>>(input.CreateOrEditRoutStepDtoList);


            if (AbpSession.TenantId != null)
            {
                shippingRequest.TenantId = (int)AbpSession.TenantId;
                shippingRequest.GoodsDetailFk.TenantId = (int)AbpSession.TenantId;
                shippingRequest.RoutSteps.ForEach(x => x.TenantId = (int)AbpSession.TenantId);
            }


            await _shippingRequestRepository.InsertAsync(shippingRequest);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        protected virtual async Task Update(CreateOrEditShippingRequestDto input)
        {
            var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync((long)input.Id);
            ObjectMapper.Map(input, shippingRequest);
        }


        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task UpdatePrice(UpdatePriceInput input)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(input.Id);
                shippingRequest.Price = input.Price;

                await _appNotifier.UpdateShippingRequestPrice(new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value), input.Id, input.Price);
            }
        }

        public async Task AcceptOrRejectShippingRequestPrice(AcceptShippingRequestPriceInput input)
        {

            var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(input.Id);
            shippingRequest.IsPriceAccepted = input.IsPriceAccepted;


            await _appNotifier.AcceptShippingRequestPrice(input.Id, input.IsPriceAccepted);
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task RejectShippingRequest(long id )
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(id);
                shippingRequest.IsRejected = true;

                //todo send notification to shipper 
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Delete)]
        [RequiresFeature(AppFeatures.ShippingRequest)]
        public async Task Delete(EntityDto<long> input)
        {
            await _shippingRequestRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetShippingRequestsToExcel(GetAllShippingRequestsForExcelInput input)
        {

            var filteredShippingRequests = _shippingRequestRepository.GetAll()
                        .Include(e => e.TrucksTypeFk)
                        .Include(e => e.TrailerTypeFk)
                        .Include(e => e.GoodsDetailFk)
                        .Include(e => e.RouteFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                        .WhereIf(input.MinVasFilter != null, e => e.Vas >= input.MinVasFilter)
                        .WhereIf(input.MaxVasFilter != null, e => e.Vas <= input.MaxVasFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.GoodsDetailNameFilter), e => e.GoodsDetailFk != null && e.GoodsDetailFk.Name == input.GoodsDetailNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RouteDisplayNameFilter), e => e.RouteFk != null && e.RouteFk.DisplayName == input.RouteDisplayNameFilter);

            var query = (from o in filteredShippingRequests
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_goodsDetailRepository.GetAll() on o.GoodsDetailId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_routeRepository.GetAll() on o.RouteId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetShippingRequestForViewDto()
                         {
                             ShippingRequest = new ShippingRequestDto
                             {
                                 Vas = o.Vas,
                                 Id = o.Id
                             },
                             TrucksTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                             TrailerTypeDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             GoodsDetailName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),
                             RouteDisplayName = s4 == null || s4.DisplayName == null ? "" : s4.DisplayName.ToString()
                         });


            var shippingRequestListDtos = await query.ToListAsync();

            return _shippingRequestsExcelExporter.ExportToFile(shippingRequestListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<ShippingRequestTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown()
        {
            return await _lookup_trucksTypeRepository.GetAll()
                .Select(trucksType => new ShippingRequestTrucksTypeLookupTableDto
                {
                    Id = trucksType.Id.ToString(),
                    DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<ShippingRequestTrailerTypeLookupTableDto>> GetAllTrailerTypeForTableDropdown()
        {
            return await _lookup_trailerTypeRepository.GetAll()
                .Select(trailerType => new ShippingRequestTrailerTypeLookupTableDto
                {
                    Id = trailerType.Id,
                    DisplayName = trailerType == null || trailerType.DisplayName == null ? "" : trailerType.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<ShippingRequestGoodsDetailLookupTableDto>> GetAllGoodsDetailForTableDropdown()
        {
            return await _lookup_goodsDetailRepository.GetAll()
                .Select(goodsDetail => new ShippingRequestGoodsDetailLookupTableDto
                {
                    Id = goodsDetail.Id,
                    DisplayName = goodsDetail == null || goodsDetail.Name == null ? "" : goodsDetail.Name.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<ShippingRequestRouteLookupTableDto>> GetAllRouteForTableDropdown()
        {
            return await _lookup_routeRepository.GetAll()
                .Select(route => new ShippingRequestRouteLookupTableDto
                {
                    Id = route.Id,
                    DisplayName = route == null || route.DisplayName == null ? "" : route.DisplayName.ToString()
                }).ToListAsync();
        }

    }
}