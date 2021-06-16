using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Ports;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Packing.PackingTypes;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutSteps;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.TachyonDealer;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.ShippingRequestTripVases;
using TACHYON.ShippingRequestVases;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.UnitOfMeasures;
using TACHYON.Vases;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
    public class ShippingRequestsAppService : TACHYONAppServiceBase, IShippingRequestsAppService
    {
        public ShippingRequestsAppService(
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository,
            IRepository<ShippingType, int> shippingTypeRepository,
            IRepository<PackingType, int> packingTypeRepository,
            IRepository<UnitOfMeasure, int> unitOdMeasureRepository,
            IAppNotifier appNotifier,
            IRepository<Tenant> tenantRepository,
            IRepository<TrucksType, long> lookupTrucksTypeRepository,
            IRepository<TrailerType, int> lookupTrailerTypeRepository,
            //IRepository<RoutType, int> lookupRoutTypeRepository,
            IRepository<GoodCategory, int> lookupGoodCategoryRepository,
            IRepository<Vas, int> lookup_vasRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            IRepository<VasPrice> vasPriceRepository,
            IRepository<Port, long> lookupPortRepository, IRepository<ShippingRequestBid, long> shippingRequestBidRepository,
            BidDomainService bidDomainService,
            IRepository<Capacity, int> capacityRepository, IRepository<TransportType, int> transportTypeRepository, IRepository<RoutPoint, long> routPointRepository,
            IRepository<ShippingRequestsCarrierDirectPricing> carrierDirectPricingRepository,
            CommissionManager commissionManager, IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository, PriceOfferManager priceOfferManager)
        {
            _vasPriceRepository = vasPriceRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _shippingRequestTripVasRepository = shippingRequestTripVasRepository;
            _shippingTypeRepository = shippingTypeRepository;
            _packingTypeRepository = packingTypeRepository;
            _unitOfMeasureRepository = unitOdMeasureRepository;
            _appNotifier = appNotifier;
            _tenantRepository = tenantRepository;
            _lookup_trucksTypeRepository = lookupTrucksTypeRepository;
            _lookup_trailerTypeRepository = lookupTrailerTypeRepository;
            _lookup_goodCategoryRepository = lookupGoodCategoryRepository;
            _lookup_PortRepository = lookupPortRepository;
            _shippingRequestBidRepository = shippingRequestBidRepository;
            _bidDomainService = bidDomainService;
            _lookup_vasRepository = lookup_vasRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _capacityRepository = capacityRepository;
            _transportTypeRepository = transportTypeRepository;
            _routPointRepository = routPointRepository;
            _carrierDirectPricingRepository = carrierDirectPricingRepository;
            _commissionManager = commissionManager;
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _priceOfferManager = priceOfferManager;
        }
        private readonly IRepository<ShippingRequestsCarrierDirectPricing> _carrierDirectPricingRepository;
        private readonly IRepository<VasPrice> _vasPriceRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<ShippingType, int> _shippingTypeRepository;
        private readonly IRepository<PackingType, int> _packingTypeRepository;
        private readonly IRepository<UnitOfMeasure, int> _unitOfMeasureRepository;
        private readonly IRepository<Vas, int> _lookup_vasRepository;
        private readonly IRepository<ShippingRequestVas, long> _shippingRequestVasRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        // private readonly IRepository<Route, int> _lookup_routeRepository;
        private readonly IRepository<RoutStep, long> _routStepRepository;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<TrailerType, int> _lookup_trailerTypeRepository;
        private readonly IRepository<GoodCategory, int> _lookup_goodCategoryRepository;
        private readonly IRepository<Port, long> _lookup_PortRepository;
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidRepository;
        private readonly BidDomainService _bidDomainService;
        private readonly IRepository<Capacity, int> _capacityRepository;
        private readonly IRepository<TransportType, int> _transportTypeRepository;
        private readonly CommissionManager _commissionManager;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly PriceOfferManager _priceOfferManager;
        public async Task<GetAllShippingRequestsOutputDto> GetAll(GetAllShippingRequestsInput Input)
        {
            DisableTenancyFilters();
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                IQueryable<ShippingRequest> query = _shippingRequestRepository
                .GetAll()
                .AsNoTracking()
                    .Include(t => t.Tenant)
                    .Include(x => x.OriginCityFk)
                    .Include(x => x.DestinationCityFk)
                .WhereIf(Input.IsBid.HasValue, e => e.IsBid == Input.IsBid.Value)
                .WhereIf(Input.Status.HasValue, e => e.Status == Input.Status.Value)
                .WhereIf(Input.IsPricedWihtoutTrips.HasValue, e => e.Status == ShippingRequestStatus.PostPrice && e.TotalsTripsAddByShippier == 0)
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer), e => e.IsTachyonDeal)//if the user is TachyonDealer
                .WhereIf(IsEnabled(AppFeatures.Carrier), e => e.CarrierTenantId == AbpSession.TenantId) //if the user is carrier
                .WhereIf(IsEnabled(AppFeatures.Shipper), e => e.TenantId == AbpSession.TenantId) //if the user is shipper
                .OrderBy(Input.Sorting ?? "id desc");


                var myDraftsOnly = query.Where(x => x.TenantId == AbpSession.TenantId)
                             .Where(x => x.IsDrafted);

                var withoutDrafts = query.Where(x => !x.IsDrafted);
                //concat all requests without draft with my draft requests
                var allWithMyDraftsOnly = myDraftsOnly.Concat(withoutDrafts);

                var ResultPage = allWithMyDraftsOnly.PageBy(Input);
                var totalCount = await allWithMyDraftsOnly.CountAsync();

                var output = ObjectMapper.Map<List<ShippingRequestListDto>>(await ResultPage.ToListAsync());
                foreach (var item in output.Where(x => IsEnabled(AppFeatures.Shipper) && x.IsTachyonDeal))
                {
                    item.TotalBids = 0;
                }
                return new GetAllShippingRequestsOutputDto()
                {
                    Data = new PagedResultDto<ShippingRequestListDto>(
                    totalCount, output
                )
                ,
                    NoOfPostPriceWithoutTrips = IsEnabled(AppFeatures.Shipper) ? _shippingRequestRepository.GetAll().Where(r => r.Status == ShippingRequestStatus.PostPrice && r.TotalsTripsAddByShippier == 0 && r.TenantId == AbpSession.TenantId).Count() : 0
                };
            }
        }

        public async Task<GetShippingRequestForViewOutput> GetShippingRequestForView(long id)
        {
            DisableTenancyFilters();
            return await _GetShippingRequestForView(id);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        [RequiresFeature(AppFeatures.Shipper)]
        public async Task<GetShippingRequestForEditOutput> GetShippingRequestForEdit(EntityDto<long> input)
        {
            if (await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
                {
                    return _GetShippingRequestForEdit(input);
                }
            }

            return _GetShippingRequestForEdit(input);
        }

        #region ShippingRequestWizard

        /// <summary>
        /// Basic Details - Shipping Request Wizard
        /// </summary>
        /// <returns></returns>
        public async Task CreateOrEditStep1(CreateOrEditShippingRequestStep1Dto input)
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
                await CreateStep1(input);
            }
            else
            {
                await UpdateStep1(input);
            }
        }

        /// <summary>
        /// Route Details - Shipping Request Wizard
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task EditStep2(EditShippingRequestStep2Dto input)
        {
            var shippingRequest = await GetDraftedShippingRequest(input.Id);
            if (shippingRequest.DraftStep < 2)
            {
                shippingRequest.DraftStep = 2;
            }
            ObjectMapper.Map(input, shippingRequest);
        }

        /// <summary>
        /// Goods Details - Shipping Request Wizard
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task EditStep3(EditShippingRequestStep3Dto input)
        {
            var shippingRequest = await GetDraftedShippingRequest(input.Id);
            if (shippingRequest.DraftStep < 3)
            {
                shippingRequest.DraftStep = 3;
            }
            ObjectMapper.Map(input, shippingRequest);
        }

        /// <summary>
        /// Services - Shipping Request Wizard
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task EditStep4(EditShippingRequestStep4Dto input)
        {
            var shippingRequest = await GetDraftedShippingRequest(input.Id);
            if (shippingRequest.DraftStep < 4)
            {
                shippingRequest.DraftStep = 4;
            }
            ObjectMapper.Map(input, shippingRequest);
        }

        public async Task PublishShippingRequest(long id)
        {
            var shippingRequest = await GetDraftedShippingRequest(id);
            if (shippingRequest.DraftStep < 4)
            {
                throw new UserFriendlyException(L("YouMustCompleteWizardStepsFirst"));
            }
            await ValidateShippingRequestBeforePublish(shippingRequest);
            _commissionManager.AddShippingRequestCommissionSettingInfo(shippingRequest);
            shippingRequest.IsDrafted = false;
        }


        private async Task UpdateStep1(CreateOrEditShippingRequestStep1Dto input)
        {
            var shippingRequest = await GetDraftedShippingRequest(input.Id.Value);

            ObjectMapper.Map(input, shippingRequest);
            // ValidateStep1(shippingRequest);
        }


        private async Task<ShippingRequest> GetDraftedShippingRequest(long id)
        {
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                  .Where(x => x.Id == id && x.IsDrafted == true)
                  .FirstOrDefaultAsync();
                return shippingRequest;
            }
        }
        private async Task CreateStep1(CreateOrEditShippingRequestStep1Dto input)
        {
            ShippingRequest shippingRequest = ObjectMapper.Map<ShippingRequest>(input);
            //ValidateStep1(shippingRequest);
            shippingRequest.IsDrafted = true;
            shippingRequest.DraftStep = 1;
            await _shippingRequestRepository.InsertAndGetIdAsync(shippingRequest);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task ValidateShippingRequestBeforePublish(ShippingRequest shippingRequest)
        {
            // Bid info
            if (shippingRequest.IsBid)
            {
                if (!shippingRequest.BidStartDate.HasValue)
                {
                    shippingRequest.BidStartDate = Clock.Now.Date;
                }
                else if (shippingRequest.BidStartDate.HasValue && shippingRequest.BidStartDate.Value < Clock.Now.Date)
                {
                    throw new UserFriendlyException(L("BidStartDateConnotBeBeforeToday"));
                }

                shippingRequest.BidStatus = shippingRequest.BidStartDate.Value.Date == Clock.Now.Date ? ShippingRequestBidStatus.OnGoing : ShippingRequestBidStatus.StandBy;
                await SendNotificationToCarriersWithTheSameTrucks(shippingRequest);
            }
        }

        #endregion

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

            // Vas validation
            await ShippingRequestVasListValidate(input);

            //Check create or edit
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
                    var vas = await _shippingRequestVasRepository.FirstOrDefaultAsync(x => x.Id == item.ShippingRequestVasId);
                    vas.ActualPrice = item.ActualPrice;
                    vas.DefualtPrice = item.DefaultPrice;
                    await _shippingRequestVasRepository.UpdateAsync(vas);
                }


                ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(input.Id);
                if ((shippingRequest.Status != ShippingRequestStatus.Cancled))
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
            if (shippingRequest.Status == ShippingRequestStatus.Cancled)
            {
                throw new UserFriendlyException(L("Cant accept or reject price for rejected request"));
            }

            shippingRequest.IsPriceAccepted = input.IsPriceAccepted;
            if (shippingRequest.IsPriceAccepted.Value)
            {
                shippingRequest.Status = ShippingRequestStatus.PrePrice;
            }

            await _appNotifier.AcceptShippingRequestPrice(input.Id, input.IsPriceAccepted);
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task RejectShippingRequest(long id)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant))
            {
                ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(id);
                if (shippingRequest.Status == ShippingRequestStatus.PostPrice)
                {
                    throw new UserFriendlyException(L("Cant reject accepted price request"));
                }

                shippingRequest.Status = ShippingRequestStatus.Cancled;

                await _appNotifier.RejectShippingRequest(new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value), id);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Delete)]
        [RequiresFeature(AppFeatures.ShippingRequest)]
        public async Task Delete(EntityDto<long> input)
        {
            await _shippingRequestRepository.DeleteAsync(input.Id);
        }

        public async Task<List<CarriersForDropDownDto>> GetAllCarriersForDropDownAsync()
        {
            return await _tenantRepository.GetAll()
                .Where(x => x.Edition.Name == AppConsts.CarrierEditionName)
                .Select(x => new CarriersForDropDownDto { Id = x.Id, DisplayName = x.TenancyName }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<TrucksTypeSelectItemDto>> GetAllTrucksTypeForTableDropdown()
        {
            var list = await _lookup_trucksTypeRepository.GetAll().Include(x => x.Translations).ToListAsync();
            return ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(list);
            //.Select(trucksType => new SelectItemDto { Id = trucksType.Id.ToString(), DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString() }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
        public async Task<List<SelectItemDto>> GetAllTrailerTypeForTableDropdown()
        {
            return await _lookup_trailerTypeRepository.GetAll()
                .Select(trailerType => new SelectItemDto { Id = trailerType.Id.ToString(), DisplayName = trailerType == null || trailerType.DisplayName == null ? "" : trailerType.DisplayName.ToString() }).ToListAsync();
        }

        public async Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodCategoriesForTableDropdown()
        {
            var list = await _lookup_goodCategoryRepository.GetAll()
                .Include(x => x.Translations).ToListAsync();

            return ObjectMapper.Map<List<GetAllGoodsCategoriesForDropDownOutput>>(list);
            //.Select(x => new GetAllGoodsCategoriesForDropDownOutput { Id = x.Id.ToString(), DisplayName = x == null || x.DisplayName == null ? "" : x.DisplayName.ToString() }).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllPortsForDropdown()
        {
            return await _lookup_PortRepository.GetAll()
                .Select(x => new SelectItemDto { Id = x.Id.ToString(), DisplayName = x.Name })
                .ToListAsync();
        }

        protected virtual async Task<GetShippingRequestForViewOutput> _GetShippingRequestForView(long id)
        {
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                    .Where(e => e.Id == id)
                    .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                    .WhereIf(await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                        .Include(e => e.ShippingRequestBids)
                        .Include(e => e.OriginCityFk)
                        .Include(e => e.DestinationCityFk)
                        .Include(e => e.AssignedDriverUserFk)
                        .Include(e => e.AssignedTruckFk)
                        .ThenInclude(e => e.TrucksTypeFk)
                        .Include(e => e.TrucksTypeFk)
                        .ThenInclude(e => e.Translations)
                        .Include(e => e.TransportTypeFk)
                        .Include(e => e.CapacityFk)
                        .Include(e => e.AssignedTruckFk)
                        .ThenInclude(e => e.TruckStatusFk)
                        .Include(e => e.GoodCategoryFk)
                        .ThenInclude(e => e.Translations)
                        .Include(e => e.ShippingTypeFk)
                        .Include(e => e.PackingTypeFk)
                        .Include(e => e.CarrierTenantFk)
                        .FirstOrDefaultAsync();

                if (shippingRequest.TenantId != AbpSession.TenantId && shippingRequest.IsDrafted)
                {
                    return null;
                }
                if (await IsEnabledAsync(AppFeatures.Carrier))
                {
                    if (shippingRequest.CarrierTenantId != AbpSession.TenantId)
                    {
                        if ((shippingRequest.Status == ShippingRequestStatus.PrePrice || shippingRequest.Status == ShippingRequestStatus.NeedsAction))
                        {
                            if (!_carrierDirectPricingRepository.GetAll().Any(e => e.RequestId == shippingRequest.Id && e.CarrirerTenantId == AbpSession.TenantId))
                            {
                                throw new ValidationException(L("NotShippingRequest"));
                            }
                        }
                        else
                        {
                            throw new ValidationException(L("NotShippingRequest"));
                        }
                    }
                }

                var shippingRequestVasList = await _shippingRequestVasRepository.GetAll()
                    .Where(x => x.ShippingRequestId == id)
                    .Select(e =>
                    new GetShippingRequestVasForViewDto
                    {
                        ShippingRequestVas = ObjectMapper.Map<ShippingRequestVasDto>(e),
                        VasName = e.VasFk.Name
                    }).ToListAsync();

                var shippingRequestBidDtoList = ObjectMapper.Map<List<ShippingRequestBidDto>>(shippingRequest.ShippingRequestBids);
                if (IsEnabled(AppFeatures.Shipper))
                {
                    if (shippingRequest.IsTachyonDeal)
                        shippingRequestBidDtoList = null;
                }

                GetShippingRequestForViewOutput output = ObjectMapper.Map<GetShippingRequestForViewOutput>(shippingRequest);
                output.ShippingRequestBidDtoList = shippingRequestBidDtoList;
                output.ShippingRequestVasDtoList = shippingRequestVasList;

                //return translated good category name by default language
                output.GoodsCategoryName = ObjectMapper.Map<GoodCategoryDto>(shippingRequest.GoodCategoryFk).DisplayName;

                //return translated truck type by default language
                output.TruckTypeDisplayName = ObjectMapper.Map<TrucksTypeDto>(shippingRequest.TrucksTypeFk).TranslatedDisplayName;
                output.TruckTypeFullName = ObjectMapper.Map<TransportTypeDto>(shippingRequest.TransportTypeFk).TranslatedDisplayName
                                        + "-" + output.TruckTypeDisplayName
                                        + "-" + ObjectMapper.Map<CapacityDto>(shippingRequest.CapacityFk).TranslatedDisplayName;

                return output;
            }
        }

        protected virtual GetShippingRequestForEditOutput _GetShippingRequestForEdit(EntityDto<long> input)
        {
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                ShippingRequest shippingRequest = _shippingRequestRepository
                    .GetAll()
                    .Include(x => x.ShippingRequestVases)
                    .Single(x => x.Id == input.Id);
                var Request = ObjectMapper.Map<CreateOrEditShippingRequestDto>(shippingRequest);
                Request.ShippingRequestVasList = ObjectMapper.Map<List<CreateOrEditShippingRequestVasListDto>>(shippingRequest.ShippingRequestVases);

                if (shippingRequest.TenantId != AbpSession.TenantId && shippingRequest.IsDrafted)
                {
                    return null;
                }

                GetShippingRequestForEditOutput output = new GetShippingRequestForEditOutput
                {
                    ShippingRequest = Request
                };
                return output;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Create)]
        protected virtual async Task Create(CreateOrEditShippingRequestDto input)
        {
            var vasList = input.ShippingRequestVasList;

            ShippingRequest shippingRequest = ObjectMapper.Map<ShippingRequest>(input);
            if (AbpSession.TenantId != null)
            {
                shippingRequest.TenantId = (int)AbpSession.TenantId;

                // Bid info
                if (shippingRequest.IsBid)
                {
                    if (!shippingRequest.BidStartDate.HasValue)
                    {
                        shippingRequest.BidStartDate = Clock.Now.Date;
                    }

                    shippingRequest.BidStatus = shippingRequest.BidStartDate.Value.Date == Clock.Now.Date ? ShippingRequestBidStatus.OnGoing : ShippingRequestBidStatus.StandBy;
                }

               // _commissionManager.AddShippingRequestCommissionSettingInfo(shippingRequest);
            }


            await _shippingRequestRepository.InsertAndGetIdAsync(shippingRequest);


            await CurrentUnitOfWork.SaveChangesAsync();
            if (shippingRequest.IsBid)
            {
                //Notify Carrier with the same Truck type
                await SendNotificationToCarriersWithTheSameTrucks(shippingRequest);
            }
        }

        private async Task SendNotificationToCarriersWithTheSameTrucks(ShippingRequest shippingRequest)
        {
            if (shippingRequest.BidStatus == ShippingRequestBidStatus.OnGoing)
            {
                UserIdentifier[] users = await _bidDomainService.GetCarriersByTruckTypeArrayAsync(shippingRequest.TrucksTypeId.Value);
                await _appNotifier.ShippingRequestAsBidWithSameTruckAsync(users, shippingRequest.Id);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        protected virtual async Task Update(CreateOrEditShippingRequestDto input)
        {
            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                .Where(x => x.Id == (long)input.Id)
                .FirstOrDefaultAsync();
            input.IsBid = shippingRequest.IsBid;
            input.IsTachyonDeal = shippingRequest.IsTachyonDeal;



            foreach (var vas in shippingRequest.ShippingRequestVases)
            {
                if (!input.ShippingRequestVasList.Any(x => x.Id == vas.Id))
                {
                    await _shippingRequestVasRepository.DeleteAsync(vas);
                }
            }
            ObjectMapper.Map(input, shippingRequest);
        }


        [AbpAuthorize(AppPermissions.Pages_VasPrices)]
        public async Task<List<ShippingRequestVasListOutput>> GetAllShippingRequestVasesForTableDropdown()
        {
            return await _lookup_vasRepository.GetAll()
                .Select(vas => new ShippingRequestVasListOutput
                {
                    VasName = vas == null || vas.Name == null ? "" : vas.Name.ToString(),
                    HasAmount = vas.HasAmount,
                    HasCount = vas.HasCount,
                    MaxAmount = 0,
                    MaxCount = 0,
                    Id = vas.Id
                }).ToListAsync();
        }


        public async Task<List<ShippingRequestVasPriceDto>> GetAllShippingRequestVasForPricing(long shippingRequestId)
        {
            var shippingRequestVases = _shippingRequestVasRepository.GetAll().Include(x => x.VasFk).Where(z => z.ShippingRequestId == shippingRequestId);
            var result = from o in shippingRequestVases
                         join o1 in _vasPriceRepository.GetAll() on o.VasId equals o1.VasId into j1
                         from s1 in j1.DefaultIfEmpty()

                         select new ShippingRequestVasPriceDto()
                         {
                             ShippingRequestVas = new ShippingRequestVasListOutput
                             {
                                 VasName = o.VasFk.Name == null || o.VasFk.Name == null ? "" : o.VasFk.Name,
                                 HasAmount = o.VasFk.HasAmount,
                                 HasCount = o.VasFk.HasCount,
                                 MaxAmount = o.RequestMaxAmount,
                                 MaxCount = o.RequestMaxCount,
                             },
                             ActualPrice = s1.Price,
                             ShippingRequestVasId = o.Id,
                             DefaultPrice = s1.Price
                         };
            return await result.ToListAsync();
        }



        public async Task<ShippingRequestPricingOutputforView> GetAllShippingRequestPricingForView(long shippingRequestId)
        {
            var pricedShippingRequest = new ShippingRequestPricingOutputforView();
            var carrierId = AbpSession.TenantId;

            pricedShippingRequest.PricedVasesList = await _shippingRequestVasRepository.GetAll().Include(x => x.VasFk).Include(s => s.ShippingRequestFk).Where(z => z.ShippingRequestId == shippingRequestId)
                .Select(x => new ShippingRequestVasPriceDto
                {
                    ActualPrice = x.ActualPrice,
                    ShippingRequestVasId = x.Id,


                    ShippingRequestVas = new ShippingRequestVasListOutput
                    {
                        VasName = x.VasFk.Name,
                        MaxAmount = x.RequestMaxAmount,
                        MaxCount = x.RequestMaxCount,
                    }
                }
            ).ToListAsync();

            var shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(x => x.Id == shippingRequestId);
            pricedShippingRequest.ShippingRequestPrice = shippingRequest.Price.Value;
            return pricedShippingRequest;
        }

        #region Truck Category DropDowns
        public async Task<IEnumerable<ISelectItemDto>> GetAllTransportTypesForDropdown()
        {
            List<TransportType> transportTypes = await _transportTypeRepository
                .GetAllIncluding(x => x.Translations)
                .ToListAsync();

            List<TransportTypeSelectItemDto> transportTypeDtos = ObjectMapper.Map<List<TransportTypeSelectItemDto>>(transportTypes);

            return transportTypeDtos;
        }

        public async Task<List<TrucksTypeSelectItemDto>> GetAllTruckTypesByTransportTypeIdForDropdown(int transportTypeId)
        {


            var list = await _lookup_trucksTypeRepository.GetAll()
                .Include(x => x.Translations)
                .Where(x => x.TransportTypeId == transportTypeId).ToListAsync();
            return ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(list);
            //.Select(x => new SelectItemDto()
            //{
            //    Id = x.Id.ToString(),
            //    DisplayName = x.DisplayName
            //}).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllTuckCapacitiesByTuckTypeIdForDropdown(int truckTypeId)
        {
            return await _capacityRepository.GetAll()
                .Where(x => x.TrucksTypeId == truckTypeId)
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }
        #endregion

        #region Waybills

        //Master Waybill
        public IEnumerable<GetMasterWaybillOutput> GetMasterWaybill(int shippingRequestTripId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var info = _shippingRequestTripRepository.GetAll()
                    .Include(e => e.ShippingRequestFk)
                    .Where(e => e.Id == shippingRequestTripId);

                var query = info.Select(x => new
                {
                    MasterWaybillNo = x.Id,
                    ShippingRequestStatus = "Draft",
                    SenderCompanyName = x.ShippingRequestFk.Tenant.companyName,
                    DriverName = x.AssignedDriverUserFk != null ? x.AssignedDriverUserFk.FullName : "",
                    DriverIqamaNo = "",
                    TruckTypeTranslationList = x.AssignedTruckFk.TrucksTypeFk.Translations,
                    TruckTypeDisplayName =
                    //x.AssignedTruckFk != null
                    //   ? (x.AssignedTruckFk.TransportTypeFk != null ?
                    //       x.AssignedTruckFk.TransportTypeFk.DisplayName :
                    //       "" + "-" + x.AssignedTruckFk.TrucksTypeFk != null ?
                    //           x.AssignedTruckFk.TrucksTypeFk.DisplayName :
                    //           "" + "-" + x.AssignedTruckFk.CapacityFk != null ?
                    //               x.AssignedTruckFk.CapacityFk.DisplayName : "")
                    //   : "",
                    (x.AssignedTruckFk.TransportTypeFk == null ? "" : ObjectMapper.Map<TransportTypeDto>(x.AssignedTruckFk.TransportTypeFk).TranslatedDisplayName) + "-" + //o.TransportTypeFk.DisplayName) + " - " +
                             (x.AssignedTruckFk.TrucksTypeFk == null ? "" : ObjectMapper.Map<TrucksTypeDto>(x.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName) + " - " +
                             (x.AssignedTruckFk.CapacityFk == null ? "" : ObjectMapper.Map<CapacityDto>(x.AssignedTruckFk.CapacityFk).DisplayName),
                    PlateNumber = x.AssignedTruckFk != null ? x.AssignedTruckFk.PlateNumber : "",
                    IsMultipDrops = x.ShippingRequestFk.NumberOfDrops > 1 ? true : false,
                    TotalDrops = x.ShippingRequestFk.NumberOfDrops,
                    StartTripDate = (x.StartTripDate != null && x.StartTripDate.Year > 1)
                       ? x.StartTripDate.ToShortDateString()
                       : "",
                    CarrierName = x.ShippingRequestFk.CarrierTenantFk != null ? x.ShippingRequestFk.CarrierTenantFk.TenancyName : "",
                    PackingTypeDisplayName = x.ShippingRequestFk.PackingTypeFk.DisplayName,
                    NumberOfPacking = x.ShippingRequestFk.NumberOfPacking,
                    TotalWeight = x.ShippingRequestFk.TotalWeight
                });

                var pickup = GetPickupOrDropPointFacilityForTrip(shippingRequestTripId, PickingType.Pickup);


                var finalOutput = query.ToList().Select(x
                    => new GetMasterWaybillOutput()
                    {
                        MasterWaybillNo = x.MasterWaybillNo,
                        Date = Clock.Now.ToShortDateString(),
                        ShippingRequestStatus = "Draft",
                        CompanyName = x.SenderCompanyName,
                        DriverName = x.DriverName,
                        DriverIqamaNo = "",
                        TruckTypeDisplayName = x.TruckTypeDisplayName,
                        PlateNumber = x.PlateNumber,
                        IsMultipDrops = x.IsMultipDrops,
                        TotalDrops = x.TotalDrops,
                        PackingTypeDisplayName = x.PackingTypeDisplayName,
                        NumberOfPacking = x.NumberOfPacking,
                        FacilityName = pickup?.Name,
                        CountryName = pickup?.CityFk.CountyFk.DisplayName,
                        CityName = pickup?.CityFk.DisplayName,
                        Area = pickup?.Address,
                        StartTripDate = x.StartTripDate,
                        CarrierName = x.CarrierName,
                        TotalWeight = x.TotalWeight

                    });

                return finalOutput;
            }
        }


        //Single Drop Waybill
        public IEnumerable<GetSingleDropWaybillOutput> GetSingleDropWaybill(int shippingRequestTripId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var info = _shippingRequestTripRepository.GetAll()
                    .Where(e => e.Id == shippingRequestTripId);

                var query = info.Select(x => new
                {
                    MasterWaybillNo = x.Id,
                    ShippingRequestStatus = "Draft",
                    SenderCompanyName = x.ShippingRequestFk.Tenant.companyName,
                    ClientName = x.ShippingRequestFk.Tenant.Name,
                    ReceiverCompanyName = x.ShippingRequestFk.CarrierTenantFk != null ? x.ShippingRequestFk.CarrierTenantFk.companyName : "",
                    CarrierName = x.ShippingRequestFk.CarrierTenantFk.Name,
                    DriverName = x.AssignedDriverUserFk != null ? x.AssignedDriverUserFk.FullName : "",
                    DriverIqamaNo = "",
                    TruckTypeTranslationList = x.AssignedTruckFk.TrucksTypeFk.Translations,
                    TruckTypeDisplayName = (x.AssignedTruckFk.TransportTypeFk == null ? "" : ObjectMapper.Map<TransportTypeDto>(x.AssignedTruckFk.TransportTypeFk).TranslatedDisplayName) + "-" + //o.TransportTypeFk.DisplayName) + " - " +
                             (x.AssignedTruckFk.TrucksTypeFk == null ? "" : ObjectMapper.Map<TrucksTypeDto>(x.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName) + " - " +
                             (x.AssignedTruckFk.CapacityFk == null ? "" : ObjectMapper.Map<CapacityDto>(x.AssignedTruckFk.CapacityFk).DisplayName),
                    PlateNumber = x.AssignedTruckFk != null ? x.AssignedTruckFk.PlateNumber : "",
                    PackingTypeDisplayName = x.ShippingRequestFk.PackingTypeFk.DisplayName,
                    NumberOfPacking = x.ShippingRequestFk.NumberOfPacking,
                    StartTripDate = x.StartTripDate,
                    //( x.StartTripDate.Year > 1)
                    //? x.StartTripDate
                    //: null,
                    DeliveryDate = x.EndTripDate,
                    TotalWeight = x.ShippingRequestFk.TotalWeight,
                    GoodCategoryTranslation = x.ShippingRequestFk.GoodCategoryFk.Translations,
                    GoodsCategoryDisplayName = x.ShippingRequestFk.GoodCategoryFk //x.ShippingRequestFk.GoodCategoryFk.DisplayName
                });

                var pickup = GetPickupOrDropPointFacilityForTrip(shippingRequestTripId, PickingType.Pickup);

                var delivery = GetPickupOrDropPointFacilityForTrip(shippingRequestTripId, PickingType.Dropoff);


                var finalOutput = query.ToList().Select(x
                    => new GetSingleDropWaybillOutput
                    {
                        MasterWaybillNo = x.MasterWaybillNo,
                        Date = Clock.Now.ToShortDateString(),
                        ShippingRequestStatus = "Draft",
                        SenderCompanyName = x.SenderCompanyName,
                        ReceiverCompanyName = x.ReceiverCompanyName,
                        DriverName = x.DriverName,
                        DriverIqamaNo = "",
                        TruckTypeDisplayName = x.TruckTypeDisplayName,
                        PlateNumber = x.PlateNumber,
                        PackingTypeDisplayName = x.PackingTypeDisplayName,
                        NumberOfPacking = x.NumberOfPacking,
                        FacilityName = pickup != null ? pickup.Name : "",
                        CountryName = pickup?.CityFk.CountyFk.DisplayName,
                        CityName = pickup?.CityFk.DisplayName,
                        Area = pickup?.Address,
                        StartTripDate = x.StartTripDate,
                        DroppFacilityName = delivery?.Name,
                        DroppCountryName = delivery?.CityFk.CountyFk.DisplayName,
                        DroppCityName = delivery?.CityFk.DisplayName,
                        DroppArea = delivery?.Address,
                        DeliveryDate = x.DeliveryDate,
                        TotalWeight = x.TotalWeight,
                        ClientName = x.ClientName,
                        CarrierName = x.CarrierName,
                        GoodsCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(x.GoodsCategoryDisplayName).DisplayName,

                    });

                return finalOutput;
            }
        }

        public IEnumerable<GetAllShippingRequestVasesOutput> GetShippingRequestVasesForSingleDropWaybill(int shippingRequestTripId)
        {
            var vases = _shippingRequestTripVasRepository.GetAll()
                .Include(x => x.ShippingRequestVasFk)
                .Include(x => x.ShippingRequestVasFk.VasFk)
                .Where(x => x.ShippingRequestTripId == shippingRequestTripId)
                .ToList();

            var output = vases.Select(x => new GetAllShippingRequestVasesOutput
            {
                VasName = x.ShippingRequestVasFk.VasFk.Name,
                Amount = x.ShippingRequestVasFk.RequestMaxAmount,
                Count = x.ShippingRequestVasFk.RequestMaxCount
            });

            return output;
        }
        //End Single Drop

        //Multiple Drops 
        public IEnumerable<GetMultipleDropWaybillOutput> GetMultipleDropWaybill(long routPointId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                //get shipping request id by step id
                var routPoint = _routPointRepository.GetAll()
                    .Include(e => e.FacilityFk)
                    .ThenInclude(e => e.CityFk)
                    .ThenInclude(e => e.CountyFk)
                    .FirstOrDefault(x => x.Id == routPointId);

                var info = _shippingRequestTripRepository.GetAll()
                    .Include(e => e.ShippingRequestFk)
                    .Where(e => e.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .Where(e => e.Id == routPoint.ShippingRequestTripId);

                var query = info.Select(x => new
                {
                    MasterWaybillNo = x.Id,
                    SubWaybillNo = routPoint.Id,
                    ShippingRequestStatus = "Draft",
                    SenderCompanyName = x.ShippingRequestFk.Tenant.companyName,
                    ReceiverCompanyName = x.ShippingRequestFk.CarrierTenantFk != null ? x.ShippingRequestFk.CarrierTenantFk.companyName : "",
                    DriverName = x.AssignedDriverUserFk != null ? x.AssignedDriverUserFk.Name : "",
                    DriverIqamaNo = "",
                    TruckTypeTranslationList = x.AssignedTruckFk.TrucksTypeFk.Translations,
                    TruckTypeDisplayName = (x.AssignedTruckFk.TransportTypeFk == null ? "" : ObjectMapper.Map<TransportTypeDto>(x.AssignedTruckFk.TransportTypeFk).TranslatedDisplayName) + "-" + //o.TransportTypeFk.DisplayName) + " - " +
                             (x.AssignedTruckFk.TrucksTypeFk == null ? "" : ObjectMapper.Map<TrucksTypeDto>(x.AssignedTruckFk.TrucksTypeFk).TranslatedDisplayName) + " - " +
                             (x.AssignedTruckFk.CapacityFk == null ? "" : ObjectMapper.Map<CapacityDto>(x.AssignedTruckFk.CapacityFk).DisplayName),
                    PlateNumber = x.AssignedTruckFk != null ? x.AssignedTruckFk.PlateNumber : "",
                    PackingTypeDisplayName = x.ShippingRequestFk.PackingTypeFk.DisplayName,
                    NumberOfPacking = x.ShippingRequestFk.NumberOfPacking,
                    StartTripDate = (x.StartTripDate != null && x.StartTripDate.Year > 1)
                        ? x.StartTripDate.ToShortDateString()
                        : "",
                    DroppFacilityName = routPoint.FacilityFk.Name,
                    DroppCountryName = routPoint.FacilityFk.CityFk.CountyFk.DisplayName,
                    DroppCityName = routPoint.FacilityFk.CityFk.DisplayName,
                    DroppArea = routPoint.FacilityFk.Address,
                    CarrierName = x.ShippingRequestFk.CarrierTenantFk != null ? x.ShippingRequestFk.CarrierTenantFk.Name : "",
                    TotalWeight = x.ShippingRequestFk.TotalWeight,
                    GoodsCategoryTranslation = x.ShippingRequestFk.GoodCategoryFk.Translations,
                    GoodsCategoryDisplayName = x.ShippingRequestFk.GoodCategoryFk,
                    DeliveryDate = x.EndTripDate
                });

                var finalOutput = query.ToList().Select(x
                    => new GetMultipleDropWaybillOutput
                    {
                        MasterWaybillNo = x.MasterWaybillNo,
                        SubWaybillNo = x.SubWaybillNo,
                        Date = Clock.Now.ToShortDateString(),
                        ShippingRequestStatus = "Draft",
                        SenderCompanyName = x.SenderCompanyName,
                        ReceiverCompanyName = x.ReceiverCompanyName,
                        DriverName = x.DriverName,
                        DriverIqamaNo = "",
                        TruckTypeDisplayName = x.TruckTypeDisplayName,
                        PlateNumber = x.PlateNumber,
                        PackingTypeDisplayName = x.PackingTypeDisplayName,
                        NumberOfPacking = x.NumberOfPacking,
                        StartTripDate = x.StartTripDate,
                        DroppFacilityName = x.DroppFacilityName,
                        DroppCountryName = x.DroppCountryName,
                        DroppCityName = x.DroppCityName,
                        DroppArea = x.DroppArea,
                        CarrierName = x.CarrierName,
                        ClientName = "Shipper",
                        TotalWeight = x.TotalWeight,
                        GoodsCategoryDisplayName = ObjectMapper.Map<GoodCategoryDto>(x.GoodsCategoryDisplayName).DisplayName,// x.GoodsCategoryDisplayName,
                        DeliveryDate = x.DeliveryDate
                    });

                return finalOutput;
            }
        }

        public IEnumerable<GetAllShippingRequestVasesOutput> GetShippingRequestVasesForMultipleDropWaybill(long RoutPointId)
        {
            //get shipping request id by step id
            var shippingRequestTripId = _routPointRepository.FirstOrDefault(x => x.Id == RoutPointId).ShippingRequestTripId;

            //get vases by shipping request id
            var vases = _shippingRequestTripVasRepository.GetAll()
                .Include(x => x.ShippingRequestVasFk.VasFk)
                .Include(x => x.ShippingRequestVasFk)
                .Where(x => x.ShippingRequestTripId == shippingRequestTripId)
                .ToList();

            var output = vases.Select(x => new GetAllShippingRequestVasesOutput
            {
                VasName = x.ShippingRequestVasFk.VasFk.DisplayName,
                Amount = x.ShippingRequestVasFk.RequestMaxAmount,
                Count = x.ShippingRequestVasFk.RequestMaxCount
            });

            return output;
        }


        #endregion

        #region dropDowns


        public async Task<List<SelectItemDto>> GetAllUnitOfMeasuresForDropdown()
        {
            return await _unitOfMeasureRepository.GetAll()
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllShippingTypesForDropdown()
        {
            return await _shippingTypeRepository.GetAll()
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }
        public async Task<List<SelectItemDto>> GetAllPackingTypesForDropdown()
        {
            return await _packingTypeRepository.GetAll()
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }
        //end Multiple Drops



        #endregion


        private async Task ShippingRequestVasListValidate(CreateOrEditShippingRequestDto input)
        {
            if (input.ShippingRequestVasList.Count <= 0) return;

            var vasesItems = await _lookup_vasRepository.GetAllListAsync();

            foreach (var item in input.ShippingRequestVasList)
            {
                var vasItem = vasesItems.FirstOrDefault(x => x.Id == item.VasId);
                if (vasItem != null && vasItem.HasAmount)
                {
                    if (item.RequestMaxAmount < 1)
                    {
                        throw new ValidationException(L("Vas Amount must have value"));
                    }
                }

                if (vasItem != null && vasItem.HasCount)
                {
                    if (item.RequestMaxCount < 1)
                    {
                        throw new ValidationException(L("Vas Count must have value"));
                    }
                }
            }
        }

        private Facility GetPickupOrDropPointFacilityForTrip(int id, PickingType type)
        {
            var pickupFacility = _routPointRepository
                .GetAll()
                .Where(x => x.ShippingRequestTripId == id)
                .Where(x => x.PickingType == type)
                .Include(x => x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .ThenInclude(x => x.CountyFk)
                .Select(x => x.FacilityFk)
                .FirstOrDefault();
            return pickupFacility;
        }


    }



}