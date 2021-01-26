using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Ports;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Routs;
using TACHYON.Routs.RoutSteps;
using TACHYON.Routs.RoutTypes;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.Exporting;
using TACHYON.ShippingRequestVases;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Vases;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
    public class ShippingRequestsAppService : TACHYONAppServiceBase, IShippingRequestsAppService
    {
        public ShippingRequestsAppService(
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IShippingRequestsExcelExporter shippingRequestsExcelExporter,
            IRepository<Route, int> lookup_routeRepository,
            IAppNotifier appNotifier,
            IRepository<RoutStep, long> routStepRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<TrucksType, long> lookupTrucksTypeRepository,
            IRepository<Truck, long> truckRepository,
            IRepository<TrailerType, int> lookupTrailerTypeRepository,
            IRepository<RoutType, int> lookupRoutTypeRepository,
            IRepository<GoodCategory, int> lookupGoodCategoryRepository,
            IRepository<Facility, long> lookupFacilityRepository,
            IRepository<Vas, int> lookup_vasRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            IRepository<VasPrice> vasPriceRepository,
            IRepository<Port, long> lookupPortRepository, IRepository<ShippingRequestBid, long> shippingRequestBidRepository,
            BidDomainService bidDomainService
        )
        {
            _vasPriceRepository = vasPriceRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestsExcelExporter = shippingRequestsExcelExporter;
            _lookup_routeRepository = lookup_routeRepository;
            _appNotifier = appNotifier;
            _routStepRepository = routStepRepository;
            _tenantRepository = tenantRepository;
            _lookup_trucksTypeRepository = lookupTrucksTypeRepository;
            _truckRepository = truckRepository;
            _lookup_trailerTypeRepository = lookupTrailerTypeRepository;
            _lookup_routTypeRepository = lookupRoutTypeRepository;
            _lookup_goodCategoryRepository = lookupGoodCategoryRepository;
            _lookup_FacilityRepository = lookupFacilityRepository;
            _lookup_PortRepository = lookupPortRepository;
            _shippingRequestBidRepository = shippingRequestBidRepository;
            _bidDomainService = bidDomainService;
            _lookup_vasRepository = lookup_vasRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
        }

        private readonly IRepository<VasPrice> _vasPriceRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IShippingRequestsExcelExporter _shippingRequestsExcelExporter;
        private readonly IRepository<Vas, int> _lookup_vasRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;

        private readonly IRepository<Route, int> _lookup_routeRepository;
        private readonly IRepository<RoutStep, long> _routStepRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<TrailerType, int> _lookup_trailerTypeRepository;
        private readonly IRepository<RoutType, int> _lookup_routTypeRepository;
        private readonly IRepository<GoodCategory, int> _lookup_goodCategoryRepository;
        private readonly IRepository<Facility, long> _lookup_FacilityRepository;
        private readonly IRepository<Port, long> _lookup_PortRepository;
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidRepository;
        private readonly BidDomainService _bidDomainService;

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

            return await GetAllPagedResultDto(input);
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

            return await _GetShippingRequestForView(id);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        public async Task<GetShippingRequestForEditOutput> GetShippingRequestForEdit(EntityDto<long> input)
        {
            ShippingRequest shippingRequest;

            if (await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return _GetShippingRequestForEdit(input);
                }
            }

            return _GetShippingRequestForEdit(input);
        }

        [RequiresFeature(AppFeatures.ShippingRequest)]
        public async Task CreateOrEdit(CreateOrEditShippingRequestDto input)
        {
            if (input.IsTachyonDeal)
            {
                if (!await IsEnabledAsync(AppFeatures.SendTachyonDealShippingRequest))
                {
                    throw new UserFriendlyException(L("feature SendTachyonDealShippingRequest not enabled"));
                }
            }

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }


        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task UpdatePrice(UpdatePriceInput input)
        {

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {

                var pricedVases = input.PricedVasesList;
                foreach (var item in pricedVases)
                {
                    var vas =await _shippingRequestVasRepository.FirstOrDefaultAsync(x=>x.Id ==item.ShippingRequestVasId);
                    vas.ActualPrice = item.ActualPrice;
                    vas.DefualtPrice = item.DefaultPrice;
                    await _shippingRequestVasRepository.UpdateAsync(vas);
                }


                ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(input.Id);
                if ((shippingRequest.IsRejected != null && shippingRequest.IsRejected.Value) || (shippingRequest.IsPriceAccepted != null && shippingRequest.IsPriceAccepted.Value))
                {
                    throw new UserFriendlyException(L("cant update price for rejected request"));
                }

                shippingRequest.Price = input.Price;

                await _appNotifier.UpdateShippingRequestPrice(new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value), input.Id, input.Price);
            }
        }

        public async Task AcceptOrRejectShippingRequestPrice(AcceptShippingRequestPriceInput input)
        {
            ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(input.Id);
            if (shippingRequest.IsRejected.HasValue && shippingRequest.IsRejected.Value)
            {
                throw new UserFriendlyException(L("Cant accept or reject price for rejected request"));
            }

            shippingRequest.IsPriceAccepted = input.IsPriceAccepted;
            if (shippingRequest.IsPriceAccepted.Value)
            {
                shippingRequest.StageOneFinish = true;
            }

            await _appNotifier.AcceptShippingRequestPrice(input.Id, input.IsPriceAccepted);
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task RejectShippingRequest(long id)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(id);
                if (shippingRequest.IsPriceAccepted.HasValue && shippingRequest.IsPriceAccepted.Value)
                {
                    throw new UserFriendlyException(L("Cant reject accepted price request"));
                }

                shippingRequest.IsRejected = true;

                await _appNotifier.RejectShippingRequest(new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value), id);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Delete)]
        [RequiresFeature(AppFeatures.ShippingRequest)]
        public async Task Delete(EntityDto<long> input)
        {
            await _shippingRequestRepository.DeleteAsync(input.Id);
        }

        //g-#409
        //todo @suhila add driver permission here
        public async Task ShippingRequestChangeStatus(ShippingRequestChangeStatusInput input)
        {
            ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(input.Id);

            if (shippingRequest.AssignedDriverUserId != AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("the driver is not assigned to Shipping Request message"));
            }

            shippingRequest.ShippingRequestStatusId = input.ShippingRequestStatusId;
        }


        //public async Task<FileDto> GetShippingRequestsToExcel(GetAllShippingRequestsForExcelInput input)
        //{

        //    var filteredShippingRequests = _shippingRequestRepository.GetAll()
        //                .Include(e => e.TrucksTypeFk)
        //                .Include(e => e.TrailerTypeFk)
        //                .Include(e => e.GoodsDetailFk)
        //                .Include(e => e.RouteFk)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
        //                .WhereIf(input.MinVasFilter != null, e => e.Vas >= input.MinVasFilter)
        //                .WhereIf(input.MaxVasFilter != null, e => e.Vas <= input.MaxVasFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTypeDisplayNameFilter), e => e.TrailerTypeFk != null && e.TrailerTypeFk.DisplayName == input.TrailerTypeDisplayNameFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.GoodsDetailNameFilter), e => e.GoodsDetailFk != null && e.GoodsDetailFk.Name == input.GoodsDetailNameFilter)
        //                .WhereIf(!string.IsNullOrWhiteSpace(input.RouteDisplayNameFilter), e => e.RouteFk != null && e.RouteFk.DisplayName == input.RouteDisplayNameFilter);

        //    var query = (from o in filteredShippingRequests
        //                 join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
        //                 from s1 in j1.DefaultIfEmpty()

        //                 join o2 in _lookup_trailerTypeRepository.GetAll() on o.TrailerTypeId equals o2.Id into j2
        //                 from s2 in j2.DefaultIfEmpty()

        //                 join o3 in _lookup_goodsDetailRepository.GetAll() on o.GoodsDetailId equals o3.Id into j3
        //                 from s3 in j3.DefaultIfEmpty()

        //                 join o4 in _lookup_routeRepository.GetAll() on o.RouteId equals o4.Id into j4
        //                 from s4 in j4.DefaultIfEmpty()

        //                 select new GetShippingRequestForViewDto()
        //                 {
        //                     ShippingRequest = new ShippingRequestDto
        //                     {
        //                         Vas = o.Vas,
        //                         Id = o.Id
        //                     },
        //                     TrucksTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
        //                     TrailerTypeDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
        //                     GoodsDetailName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),
        //                     RouteDisplayName = s4 == null || s4.DisplayName == null ? "" : s4.DisplayName.ToString()
        //                 });


        //    var shippingRequestListDtos = await query.ToListAsync();

        //    return _shippingRequestsExcelExporter.ExportToFile(shippingRequestListDtos);
        //}


        public async Task<List<CarriersForDropDownDto>> GetAllCarriersForDropDownAsync()
        {
            return await _tenantRepository.GetAll()
                .Where(x => x.Edition.Name == AppConsts.CarrierEditionName)
                .Select(x => new CarriersForDropDownDto {Id = x.Id, DisplayName = x.TenancyName}).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<SelectItemDto>> GetAllTrucksTypeForTableDropdown()
        {
            return await _lookup_trucksTypeRepository.GetAll()
                .Select(trucksType => new SelectItemDto {Id = trucksType.Id.ToString(), DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString()}).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<SelectItemDto>> GetAllTrailerTypeForTableDropdown()
        {
            return await _lookup_trailerTypeRepository.GetAll()
                .Select(trailerType => new SelectItemDto {Id = trailerType.Id.ToString(), DisplayName = trailerType == null || trailerType.DisplayName == null ? "" : trailerType.DisplayName.ToString()}).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllRouteTypeForTableDropdown()
        {
            return await _lookup_routTypeRepository.GetAll()
                .Select(x => new SelectItemDto {Id = x.Id.ToString(), DisplayName = x == null || x.DisplayName == null ? "" : x.DisplayName.ToString()}).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllGoodCategoriesForTableDropdown()
        {
            return await _lookup_goodCategoryRepository.GetAll()
                .Select(x => new SelectItemDto {Id = x.Id.ToString(), DisplayName = x == null || x.DisplayName == null ? "" : x.DisplayName.ToString()}).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllPortsForDropdown()
        {
            return await _lookup_PortRepository.GetAll()
                .Select(x => new SelectItemDto {Id = x.Id.ToString(), DisplayName = x.Name})
                .ToListAsync();
        }

        protected virtual async Task<PagedResultDto<GetShippingRequestForViewDto>> GetAllPagedResultDto(GetAllShippingRequestsInput input)
        {
                IQueryable<ShippingRequest> filteredShippingRequests = _shippingRequestRepository.GetAll()
                    .Include(x => x.ShippingRequestBids)
                    .ThenInclude(b => b.Tenant)
                    .Include(e => e.RouteFk)
                    //get only this shipper shippingRequest
                    //.Where(x => x.TenantId == AbpSession.TenantId)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false)
                    //.WhereIf(input.MinVasFilter != null, e => e.Vas >= input.MinVasFilter)
                    //.WhereIf(input.MaxVasFilter != null, e => e.Vas <= input.MaxVasFilter)
                    .WhereIf(input.IsTachyonDeal.HasValue, e => e.IsTachyonDeal == input.IsTachyonDeal.Value);


                //paging
                IQueryable<ShippingRequest> pagedAndFilteredShippingRequests = filteredShippingRequests
                    .OrderBy(input.Sorting ?? "id asc")
                    .PageBy(input);


                // select result
                IEnumerable<GetShippingRequestForViewDto> shippingRequests = (await pagedAndFilteredShippingRequests.ToListAsync())
                    .Select(x =>
                        new GetShippingRequestForViewDto {ShippingRequest = ObjectMapper.Map<ShippingRequestDto>(x), ShippingRequestBidDtoList = ObjectMapper.Map<List<ShippingRequestBidDto>>(x.ShippingRequestBids)}
                    );


                int totalCount = await filteredShippingRequests.CountAsync();

                return new PagedResultDto<GetShippingRequestForViewDto>(totalCount, shippingRequests.ToList());
        }

        protected virtual async Task<GetShippingRequestForViewDto> _GetShippingRequestForView(long id)
        {
            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAsync(id);
            List<ShippingRequestBid> shippingRequestBidsList;

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                shippingRequestBidsList = await _shippingRequestBidRepository.GetAll()
                    .Where(x => x.ShippingRequestId == id).ToListAsync();
            }

            GetShippingRequestForViewDto output = new GetShippingRequestForViewDto {ShippingRequest = ObjectMapper.Map<ShippingRequestDto>(shippingRequest), ShippingRequestBidDtoList = ObjectMapper.Map<List<ShippingRequestBidDto>>(shippingRequestBidsList)};

            return output;
        }

        protected virtual GetShippingRequestForEditOutput _GetShippingRequestForEdit(EntityDto<long> input)
        {
            ShippingRequest shippingRequest = _shippingRequestRepository
                .GetAll()
                .Include(x => x.RouteFk)
                .Include(x => x.RoutSteps)
                .Single(x => x.Id == input.Id);

            GetShippingRequestForEditOutput output = new GetShippingRequestForEditOutput {ShippingRequest = ObjectMapper.Map<CreateOrEditShippingRequestDto>(shippingRequest)};

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Create)]
        protected virtual async Task Create(CreateOrEditShippingRequestDto input)
        {
            var vasList = input.ShippingRequestVasList;

            ShippingRequest shippingRequest = ObjectMapper.Map<ShippingRequest>(input);

            if (AbpSession.TenantId != null)
            {
                shippingRequest.TenantId = (int)AbpSession.TenantId;
                shippingRequest.RoutSteps.ForEach(x => x.TenantId = (int)AbpSession.TenantId);
                shippingRequest.RouteFk.TenantId = (int)AbpSession.TenantId;
                shippingRequest.ShippingRequestStatusId = TACHYONConsts.ShippingRequestStatusStandBy;
                // Bid start-date

                // Bid status
                if (shippingRequest.IsBid)
                {
                    if (!shippingRequest.BidStartDate.HasValue)
                    {
                        shippingRequest.BidStartDate = Clock.Now.Date;
                    }

                    shippingRequest.ShippingRequestBidStatusId = shippingRequest.BidStartDate.Value.Date == Clock.Now.Date ? TACHYONConsts.ShippingRequestStatusOnGoing : TACHYONConsts.ShippingRequestStatusStandBy;
                }
            }

            await _shippingRequestRepository.InsertAndGetIdAsync(shippingRequest);

            // Save shippingRequest VASes

            if (vasList.Count > 0)
            {

                foreach (var item in vasList)
                {
                    var vas = new ShippingRequestVas();
                    vas.RequestMaxAmount = item.MaxAmount;
                    vas.RequestMaxCount = item.MaxCount;
                    vas.VasId = item.Id;
                    vas.ShippingRequestId = shippingRequest.Id;

                    if (AbpSession.TenantId != null)
                    {
                        vas.TenantId = (int)AbpSession.TenantId;
                    }

                    await _shippingRequestVasRepository.InsertAsync(vas);
                }

            }
            await CurrentUnitOfWork.SaveChangesAsync();
            if (shippingRequest.IsBid)
            {
                //Notify Carrier with the same Truck type
                if (shippingRequest.ShippingRequestBidStatusId == TACHYONConsts.ShippingRequestStatusOnGoing)
                {
                    UserIdentifier[] users = await _bidDomainService.GetCarriersByTruckTypeArrayAsync(shippingRequest.TrucksTypeId);
                    await _appNotifier.ShippingRequestAsBidWithSameTruckAsync(users, shippingRequest.Id);
                }
            }
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        protected virtual async Task Update(CreateOrEditShippingRequestDto input)
        {
            ShippingRequest shippingRequest = await _shippingRequestRepository
                .GetAllIncluding(x => x.RouteFk)
                .Include(x => x.RoutSteps)
                .FirstOrDefaultAsync(x => x.Id == (long)input.Id);
            ObjectMapper.Map(input, shippingRequest);
        }


        [AbpAuthorize(AppPermissions.Pages_VasPrices)]
        public async Task<List<ShippingRequestVasListDto>> GetAllShippingRequestVasesForTableDropdown()
        {
            return await _lookup_vasRepository.GetAll()
                .Select(vas => new ShippingRequestVasListDto
                {
                    VasName = vas == null || vas.Name == null ? "" : vas.Name.ToString(),
                    HasAmount= vas.HasAmount,
                    HasCount= vas.HasCount,
                    MaxAmount= 0,
                    MaxCount= 0,
                    Id= vas.Id
                }).ToListAsync();
        }


        public async Task<List<ShippingRequestVasPriceDto>> GetAllShippingRequestVasForPricing(long shippingRequestId)
        {
            var carrierId = AbpSession.TenantId;
            var shippingRequestVases = _shippingRequestVasRepository.GetAll().Include(x => x.VasFk).Where(z=>z.TenantId==carrierId && z.ShippingRequestId == shippingRequestId);
            var result = from o in shippingRequestVases
                         join o1 in  _vasPriceRepository.GetAll() on o.VasId equals o1.VasId into j1
                         from s1 in j1.DefaultIfEmpty()

                select new ShippingRequestVasPriceDto ()
                {
                    ShippingRequestVas = new ShippingRequestVasListDto
                    {
                    VasName = o.VasFk.Name == null || o.VasFk.Name == null ? "" : o.VasFk.Name,
                    HasAmount =  o.VasFk.HasAmount,
                    HasCount = o.VasFk.HasCount,
                    MaxAmount = o.RequestMaxAmount,
                    MaxCount = o.RequestMaxCount,
                    },
                    ActualPrice = s1.Price,
                    ShippingRequestVasId= o.Id,
                    DefaultPrice = s1.Price
                };
            return await result.ToListAsync();
        }



        public async Task<ShippingRequestPricingOutputforView> GetAllShippingRequestPricingForView(long shippingRequestId )
        {
            var  pricedShippingRequest= new ShippingRequestPricingOutputforView();
            var carrierId = AbpSession.TenantId;

            pricedShippingRequest.PricedVasesList = await _shippingRequestVasRepository.GetAll().Include(x => x.VasFk).Include(s => s.ShippingRequestFk).Where(z => z.ShippingRequestId == shippingRequestId)
                .Select(x=> new ShippingRequestVasPriceDto
                {
                    ActualPrice= x.ActualPrice,
                    ShippingRequestVasId=x.Id,
                    
                    
                    ShippingRequestVas= new ShippingRequestVasListDto
                    {
                        VasName= x.VasFk.Name,
                        MaxAmount= x.RequestMaxAmount,
                        MaxCount= x.RequestMaxCount,
                    }
                }
            ).ToListAsync();

            var shippingRequest  = await _shippingRequestRepository.FirstOrDefaultAsync(x=>x.Id ==shippingRequestId);
            pricedShippingRequest.ShippingRequestPrice = shippingRequest.Price.Value;
            return  pricedShippingRequest;
        }

        
    }
}