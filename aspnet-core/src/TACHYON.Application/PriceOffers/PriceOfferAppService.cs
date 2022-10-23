using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Actors;
using TACHYON.Cities;
using TACHYON.Cities.Dtos;
using TACHYON.Configuration;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.MultiTenancy.Dto;
using TACHYON.Notifications;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages;
using TACHYON.Rating;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.SrPostPriceUpdates;
using TACHYON.Shipping.ShippingRequestUpdates;
using TACHYON.Shipping.Trips;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;
using TACHYON.Vases;
using TACHYON.Shipping.ShippingRequestAndTripNotes;
using Abp.Collections.Extensions;
using TACHYON.Shipping.Notes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Goods.GoodCategories;
using TACHYON.Packing.PackingTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Packing.PackingTypes.Dtos;

namespace TACHYON.PriceOffers
{
    [AbpAuthorize()]
    public class PriceOfferAppService : TACHYONAppServiceBase, IPriceOfferAppService
    {
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly NormalPricePackageManager _normalPricePackageManager;
        private IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<TrucksTypesTranslation> _truckTypeTranslationRepository;
        private readonly IRepository<SrPostPriceUpdate, long> _srPostPriceUpdateRepository;
        private readonly IRepository<Facility, long> _facilityRepository;
        private readonly ShippingRequestUpdateManager _srUpdateManager;
        private readonly IRepository<ShippingRequestAndTripNote> _ShippingRequestAndTripNoteRepository;
        private readonly IRepository<Actor> _actorsRepository;
        private readonly IRepository<TransportType> _transportsTypeRepository;
        private readonly IRepository<Capacity> _capacitiesRepository;
        private readonly IRepository<GoodCategory> _goodsCategoriesRepository;
        private readonly IRepository<PackingType> _packingTypesRepository;


        private IRepository<VasPrice> _vasPriceRepository;

        public PriceOfferAppService(IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository,
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            PriceOfferManager priceOfferManager,
            IRepository<PriceOffer, long> priceOfferRepository,
            IRepository<VasPrice> vasPriceRepository,
            IRepository<City> cityRepository,
            IRepository<TrucksType, long> trucksTypeRepository,
            IAppNotifier appNotifier,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<TrucksTypesTranslation> truckTypeTranslationRepository,
            NormalPricePackageManager normalPricePackageManager,
            IRepository<Facility, long> facilityRepository,
            IRepository<SrPostPriceUpdate, long> srPostPriceUpdateRepository,
            ShippingRequestUpdateManager srUpdateManager,
            IRepository<ShippingRequestAndTripNote> ShippingRequestAndTripNoteRepository,
            IRepository<Actor> actorsRepository,
            IRepository<TransportType> transportsTypeRepository,
            IRepository<Capacity> capacitiesRepository,
            IRepository<GoodCategory> goodsCategoriesRepository,
            IRepository<PackingType> packingTypesRepository)
        {
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
            _priceOfferManager = priceOfferManager;
            _priceOfferRepository = priceOfferRepository;
            _vasPriceRepository = vasPriceRepository;
            _cityRepository = cityRepository;
            _trucksTypeRepository = trucksTypeRepository;
            _appNotifier = appNotifier;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _truckTypeTranslationRepository = truckTypeTranslationRepository;
            _normalPricePackageManager = normalPricePackageManager;
            _facilityRepository = facilityRepository;
            _srPostPriceUpdateRepository = srPostPriceUpdateRepository;
            _facilityRepository = facilityRepository;
            _srUpdateManager = srUpdateManager;
            _ShippingRequestAndTripNoteRepository = ShippingRequestAndTripNoteRepository;
            _actorsRepository = actorsRepository;
            _transportsTypeRepository = transportsTypeRepository;
            _capacitiesRepository = capacitiesRepository;
            _goodsCategoriesRepository = goodsCategoriesRepository;
            _packingTypesRepository = packingTypesRepository;
        }
        #region Services


        /// <summary>
        /// List all offers
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<PriceOfferListDto>> GetAll(PriceOfferGetAllInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            DisableTenancyFilters();
            var query = _priceOfferRepository
                .GetAll()
                .Include(x => x.Tenant)
                .Where(x => x.ShippingRequestId == input.id)
                .WhereIf(input.Channel.HasValue, x => x.Channel == input.Channel.Value /*|| x.Channel== PriceOfferChannel.TachyonManageService*/)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && (!x.ShippingRequestFk.IsTachyonDeal || x.Channel == PriceOfferChannel.TachyonManageService))
                //.WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.CarrierClients), x => x.Channel != PriceOfferChannel.CarrierAsSaas)
                .OrderBy(input.Sorting ?? "id desc");

            var offers = query.PageBy(input);

            List<PriceOfferListDto> PriceOfferList = new List<PriceOfferListDto>();
            foreach (var offer in await offers.ToListAsync())
            {
                var price = ObjectMapper.Map<PriceOfferListDto>(offer);
                if (AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper))
                {
                    price.TotalAmount = offer.TotalAmountWithCommission;
                    if (offer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                        price.StatusTitle = PriceOfferStatus.New.GetEnumDescription();
                    else if (offer.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier)
                        price.StatusTitle = PriceOfferStatus.Accepted.GetEnumDescription();
                    price.CarrierRate = offer.Tenant.Rate;
                    price.CarrierRateNumber = offer.Tenant.RateNumber;
                }
                PriceOfferList.Add(price);
            }

            return new PagedResultDto<PriceOfferListDto>(
                await query.CountAsync(),
                PriceOfferList
            );
        }
        public async Task<GetShippingRequestSearchListDto> GetAllListForSearch()
        {
            var searchList = new GetShippingRequestSearchListDto()
            {
                Cities = ObjectMapper.Map<List<CityDto>>(await _cityRepository.GetAllIncluding(x => x.Translations).ToListAsync()),
                TrucksTypes = ObjectMapper.Map<List<TrucksTypeDto>>(await _trucksTypeRepository.GetAllIncluding(x => x.Translations).ToListAsync()),
                TransportTypes = ObjectMapper.Map<List<TransportTypeDto>>(await _transportsTypeRepository.GetAllIncluding(x => x.Translations).ToListAsync()),
                Capacities = ObjectMapper.Map<List<CapacityDto>>(await _capacitiesRepository.GetAllIncluding(x => x.Translations).ToListAsync()),
                GoodsCategories= ObjectMapper.Map<List<GoodCategoryDto>>(await _goodsCategoriesRepository.GetAllIncluding(x => x.Translations).ToListAsync()),
                PackingTypes= ObjectMapper.Map<List<PackingTypeDto>>(await _packingTypesRepository.GetAllIncluding(x => x.Translations).ToListAsync())
            };

            return searchList;
        }
        

        /// <summary>
        /// Get the price offer when the user need to create offer or edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier,AppFeatures.CarrierClients)]
        public async Task<PriceOfferDto> GetPriceOfferForCreateOrEdit(long id, long? OfferId)
        {
            DisableTenancyFilters();
            var shippingRequest = await _shippingRequestsRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                  .ThenInclude(v => v.VasFk)
                .FirstOrDefaultAsync(x => x.Id == id && (x.Status == ShippingRequestStatus.PrePrice || x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier));

            if (shippingRequest == null) throw new UserFriendlyException(L("TheRecordIsNotFound"));

            var offer = await _priceOfferRepository
                .GetAll()
                .Include(i => i.PriceOfferDetails)
                .Where(x => x.ShippingRequestId == shippingRequest.Id)
                .WhereIf(OfferId.HasValue, x => x.Id == OfferId.Value)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId.Value && (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.Rejected))
                .WhereIf(await IsEnabledAsync(AppFeatures.TachyonDealer),
                x => ((x.TenantId == AbpSession.TenantId.Value || x.ShippingRequestFk.IsTachyonDeal) && (x.Status == PriceOfferStatus.New || (x.Status == PriceOfferStatus.Rejected && x.Tenant.EditionId == TachyonEditionId) || x.Status == PriceOfferStatus.Pending)))
                .OrderBy(x => x.Status)
                //.OrderByDescending(x=>x.Id)
                .FirstOrDefaultAsync();
            PriceOfferDto priceOfferDto;
            if (offer != null)
            {
                priceOfferDto = ObjectMapper.Map<PriceOfferDto>(offer);
                foreach (var item in priceOfferDto.Items)
                {
                    item.ItemName = shippingRequest.ShippingRequestVases.FirstOrDefault(x => x.Id == item.SourceId)?.VasFk.Key;
                }
                if (await IsEnabledAsync(AppFeatures.TachyonDealer))
                {
                    if (priceOfferDto.Items != null && priceOfferDto.Items.Count > 0)
                    {
                        var item = priceOfferDto.Items.FirstOrDefault();
                        priceOfferDto.VasCommissionPercentageOrAddValue = item.CommissionPercentageOrAddValue.Value;
                        priceOfferDto.VasCommissionType = item.CommissionType;

                    }
                    if (offer.TenantId != AbpSession.TenantId.Value)
                    {
                        priceOfferDto.ParentId = offer.Id;
                    }
                }
            }
            else
            {
                priceOfferDto = new PriceOfferDto()
                {// Set Default data
                    PriceType = PriceOfferType.Trip,
                    Quantity = shippingRequest.NumberOfTrips,
                    Items = GetVases(shippingRequest),
                    TaxVat = (decimal)Convert.ChangeType(SettingManager.GetSettingValue(AppSettings.HostManagement.TaxVat), typeof(decimal))
                };
                SetCommssionSettingsForTachyonDealer(priceOfferDto, shippingRequest);

            }
            if (IsEnabled(AppFeatures.TachyonDealer))
            {
                priceOfferDto.CommissionSettings = SetTenantCommssionSettingsForTachyonDealer(shippingRequest.TenantId);
            }
            return priceOfferDto;
        }

        /// <summary>
        /// Get offer details for preview
        /// </summary>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper)]
        public async Task<GetOfferForViewOutput> GetPriceOfferForView(long offerId)
        {
            DisableTenancyFilters();
            var offer = await _priceOfferRepository
                .GetAll()
                .Include(i => i.Tenant)
                .Include(i => i.PriceOfferDetails)
                .Include(i => i.ShippingRequestFk)
                 .ThenInclude(v => v.ShippingRequestVases)
                  .ThenInclude(v => v.VasFk)
                  .Where(x => x.Id == offerId)
                .WhereIf(await IsShipper(), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId.Value && (!x.ShippingRequestFk.IsTachyonDeal || x.Channel == PriceOfferChannel.TachyonManageService))
                .WhereIf(await IsCarrier(), x => x.TenantId == AbpSession.TenantId.Value)
                .SingleAsync();


            var priceOfferDto = ObjectMapper.Map<PriceOfferViewDto>(offer);



            foreach (var item in priceOfferDto.Items)
            {
                item.ItemName = offer.ShippingRequestFk?.ShippingRequestVases?.FirstOrDefault(x => x.Id == item.SourceId)?
                    .VasFk?.Name ?? L("VasRemoved");
                if (AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper))
                {
                    item.ItemPrice = item.ItemSubTotalAmountWithCommission;
                    item.ItemTotalAmount = item.ItemTotalAmountWithCommission;
                }

            }
            if (await IsShipper())
            {
                priceOfferDto.ItemPrice = offer.ItemSubTotalAmountWithCommission;
                priceOfferDto.ItemTotalAmount = offer.ItemTotalAmountWithCommission;
                priceOfferDto.TotalAmount = offer.TotalAmountWithCommission;
                priceOfferDto.SubTotalAmount = offer.SubTotalAmountWithCommission;
                priceOfferDto.VatAmount = offer.VatAmountWithCommission;
                offer.IsView = true;
            }

            return new GetOfferForViewOutput()
            {
                PriceOfferViewDto = priceOfferDto,
                CanIAcceptOffer = await _priceOfferManager.CanAcceptOrRejectOffer(offer),
                CanIAcceptOrRejectOfferOnBehalf = await _priceOfferManager.canAcceptOrRejectOfferOnBehalf(offer),
                CanIEditOffer = _priceOfferManager.CanEditOffer(offer)
            };
        }


        [RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer,AppFeatures.CarrierClients)]
        // [AbpAuthorize(AppPermissions.Pages_Offers_Create)]

        public async Task<long> CreateOrEdit(CreateOrEditPriceOfferInput Input)
        {
            if (await IsEnabledAsync(AppFeatures.Carrier) || await IsEnabledAsync(AppFeatures.CarrierClients))
            {
                Input.CommissionPercentageOrAddValue = default;
                Input.CommissionType = default;
                Input.VasCommissionPercentageOrAddValue = default;
                Input.VasCommissionType = default;
                Input.ParentId = default;
            }
            return await _priceOfferManager.CreateOrEdit(Input);

        }


        //[UnitOfWork(IsDisabled = true)]
        public async Task<PriceOfferDto> InitPriceOffer(CreateOrEditPriceOfferInput input)
        {
            DisableTenancyFilters();
            var shippingRequest = await _shippingRequestsRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                .ThenInclude(v => v.VasFk)
                .FirstOrDefaultAsync(x =>
                    x.Id == input.ShippingRequestId);

            var offer = await _priceOfferManager.InitPriceOffer(input);
            var priceOfferDto = ObjectMapper.Map<PriceOfferDto>(offer);

            if (input.VasCommissionType != null)
            {
                priceOfferDto.VasCommissionType = input.VasCommissionType.Value;
            }

            if (input.VasCommissionPercentageOrAddValue != null)
            {
                priceOfferDto.VasCommissionPercentageOrAddValue = input.VasCommissionPercentageOrAddValue.Value;
            }

            foreach (var item in priceOfferDto.Items)
            {
                item.ItemName = shippingRequest.ShippingRequestVases.FirstOrDefault(x => x.Id == item.SourceId)?.VasFk
                    .Key;
            }

            return priceOfferDto;
        }
        public async Task<ListResultDto<GetShippingRequestForPriceOfferListDto>> GetAllShippingRequest(ShippingRequestForPriceOfferGetAllInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            DisableTenancyFilters();
            List<GetShippingRequestForPriceOfferListDto> query = new List<GetShippingRequestForPriceOfferListDto>();
            if (!input.Channel.HasValue)
            {
                query = await GetFromShippingRequest(input);
            }
            else if (input.Channel == PriceOfferChannel.DirectRequest)
            {
                query = await GetFromDirectRequest(input);
            }
            else if (input.Channel == PriceOfferChannel.MarketPlace)
            {
                query = await GetFromMarketPlace(input);
            }
            else if (input.Channel == PriceOfferChannel.Offers)
            {
                query = await GetFromOffers(input);
            }
            foreach(var request in query)
            {
                var index = 1;
                foreach(var destCity in request.destinationCities)
                {
                    if (index == 1)
                        request.DestinationCity = destCity.CityName;
                    else
                        request.DestinationCity = request.DestinationCity + ", " + destCity.CityName;
                    index++;
                }
            }
            return new ListResultDto<GetShippingRequestForPriceOfferListDto>(query);
        }



        public async Task Delete(EntityDto<long> Input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _priceOfferManager.Delete(Input);
        }


        public async Task<PriceOfferStatus> Accept(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Shipper,AppFeatures.ShipperClients);
            await CheckSrHasBendingUpdates(id);
            return await _priceOfferManager.AcceptOffer(id);
        }

        private async Task CheckSrHasBendingUpdates(long priceOfferId)
        {
            var hasBendingUpdates = await _srUpdateManager.IsRequestHasBendingUpdates(priceOfferId);
            if (hasBendingUpdates)
                throw new AbpValidationException(L("CanNotAcceptOfferTheSRHasBendingUpdates"));
        }


        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<PriceOfferStatus> AcceptOfferOnBehalfShipper(long id)
        {
            await CheckSrHasBendingUpdates(id);
            return await _priceOfferManager.AcceptOfferOnBehalfShipper(id);
        }


        public async Task Reject(RejectPriceOfferInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            if (AbpSession.TenantId.HasValue) input.RejectBy = GetCurrentTenant().Name;
            await _priceOfferManager.RejectOffer(input);
        }


        public async Task Cancel(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            DisableTenancyFilters();

            var offer = await _priceOfferRepository.FirstOrDefaultAsync(x => x.Id == id && x.Status == PriceOfferStatus.Pending);
            if (offer != null)
            {
                offer.Status = PriceOfferStatus.New;
                if (offer.Channel == PriceOfferChannel.DirectRequest) await _priceOfferManager.ChangeDirectRequestStatus(offer.SourceId.Value, ShippingRequestDirectRequestStatus.Response);
                var childOffer = await _priceOfferManager.GetOfferParentId(offer.Id);
                if (childOffer != null)
                {
                    childOffer.ParentId = default;
                    childOffer.Status = PriceOfferStatus.New;
                }
            }
            else
            {
                throw new UserFriendlyException(L("YouCanNotCancel"));
            }
        }

        #endregion


        public async Task SendTestNotification()
        {
            var offer = await _priceOfferRepository.FirstOrDefaultAsync(10823);

            await _appNotifier.RejectedPostPriceOffer(offer, "Plus Company");
        }
        
        #region Pricing
        /// <summary>
        /// Get shipping request details for pricing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>




        public async Task<GetShippingRequestForPricingOutput> GetShippingRequestForPricing(GetShippingRequestForPricingInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);

            DisableTenancyFilters();
            var shippingRequest = await _shippingRequestsRepository
                            .GetAll()
                            .AsNoTracking()
                            .Include(t => t.Tenant)
                            .Include(oc => oc.OriginCityFk)
                             .ThenInclude(x => x.Translations)
                            .Include(dc => dc.ShippingRequestDestinationCities)
                            .ThenInclude(x=>x.CityFk)
                            .Include(v => v.ShippingRequestVases)
                             .ThenInclude(v => v.VasFk)
                            .Include(c => c.GoodCategoryFk)
                             .ThenInclude(x => x.Translations)
                            .Include(t => t.TrucksTypeFk)
                             .ThenInclude(x => x.Translations)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                            .FirstOrDefaultAsync(r => r.Id == input.Id/* && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice || r.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier)*/);
            long? pricePackageOfferId = default, matchingPricePackageId = default;
            if (shippingRequest == null) throw new UserFriendlyException(L("TheRecordIsNotFound"));

            var isCarrier = await IsEnabledAsync(AppFeatures.Carrier);
            if (AbpSession.TenantId.HasValue && isCarrier)// Applay permission for carrier if can see the shipping request details
            {
                if (!_priceOfferManager.CheckCarrierIsPricing(input.Id))
                {
                    if (shippingRequest.IsBid && input.Channel == PriceOfferChannel.MarketPlace)
                    {
                        if (shippingRequest.BidStatus != ShippingRequestBidStatus.OnGoing) throw new UserFriendlyException(L("The Bid must be Ongoing"));
                        //matchingPricePackageId = await _normalPricePackageManager.GetMatchingPricePackageId(shippingRequest.TrucksTypeId, shippingRequest.OriginCityId, shippingRequest.DestinationCityId, AbpSession.TenantId);
                    }
                    else
                    {
                        var _directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.CarrierTenantId == AbpSession.TenantId.Value && x.ShippingRequestId == input.Id && x.Status != ShippingRequestDirectRequestStatus.Declined);
                        if (_directRequest == null) throw new UserFriendlyException(L("YouDoNotHaveDirectRequest"));
                        pricePackageOfferId = _directRequest.PricePackageOfferId;
                    }
                }


            }



            var getShippingRequestForPricingOutput = ObjectMapper.Map<GetShippingRequestForPricingOutput>(shippingRequest);
            getShippingRequestForPricingOutput.Items = ObjectMapper.Map<List<PriceOfferItemDto>>(shippingRequest.ShippingRequestVases);
            getShippingRequestForPricingOutput.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(shippingRequest.GoodCategoryFk).DisplayName;
            getShippingRequestForPricingOutput.TrukType = ObjectMapper.Map<TrucksTypeDto>(shippingRequest.TrucksTypeFk).TranslatedDisplayName;
            getShippingRequestForPricingOutput.ShipperRating = shippingRequest.Tenant.Rate;
            getShippingRequestForPricingOutput.ShipperRatingNumber = shippingRequest.Tenant.RateNumber;
            getShippingRequestForPricingOutput.OriginCity = ObjectMapper.Map<TenantCityLookupTableDto>(shippingRequest.OriginCityFk).DisplayName;
            //getShippingRequestForPricingOutput.DestinationCity = ObjectMapper.Map<TenantCityLookupTableDto>(shippingRequest.DestinationCityFk).DisplayName;
            getShippingRequestForPricingOutput.PricePackageOfferId = pricePackageOfferId;
            getShippingRequestForPricingOutput.MatchingPricePackageId = matchingPricePackageId;
            if (isCarrier)
            {
                getShippingRequestForPricingOutput.OfferId = await _priceOfferRepository.GetAll()
                    .Where(x=> x.ShippingRequestId == shippingRequest.Id && x.TenantId == AbpSession.TenantId)
                    .Select(x=> x.Id).FirstOrDefaultAsync();
            }

            return getShippingRequestForPricingOutput;

        }

        public async Task<List<SelectItemDto>> GetAllCarrierActorsForDropDown()
        {
            return await _actorsRepository.GetAll()
                .Where(x=> x.ActorType == ActorTypesEnum.Carrier)
                   .Select(x => new SelectItemDto()
                   {
                       Id = x.Id.ToString(),
                       DisplayName = x.CompanyName
                   }).ToListAsync();
        }


        #endregion
        #region Helper
        /// <summary>
        /// Get all vas item related with shipment
        /// </summary>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private List<PriceOfferItem> GetVases(ShippingRequest shippingRequest)
        {
            List<PriceOfferItem> items = new List<PriceOfferItem>();
            if (shippingRequest.ShippingRequestVases != null && shippingRequest.ShippingRequestVases.Count > 0)
            {
                var vasIds = shippingRequest.ShippingRequestVases.Select(id => id.VasId).ToList();
                var Tenantvases = _vasPriceRepository
                    .GetAll()
                    .Include(v => v.VasFk)
                    .Where(x => x.TenantId == AbpSession.TenantId.Value && vasIds.Contains(x.VasId)).ToList();

                foreach (var vas in shippingRequest.ShippingRequestVases)
                {
                    var item = new PriceOfferItem()
                    {
                        SourceId = vas.Id,
                        PriceType = PriceOfferType.Vas,
                        Quantity = vas.RequestMaxCount <= 0 ? 1 : vas.RequestMaxCount,
                        ItemName = vas.VasFk.Key
                    };
                    var vasDefine = Tenantvases.FirstOrDefault(x => x.VasId == vas.VasId);
                    if (vasDefine != null)
                    {
                        item.ItemPrice = (decimal)vasDefine.Price;
                        item.ItemTotalAmount = item.ItemPrice * item.Quantity;
                    }

                    items.Add(item);
                }
            }
            return items;
        }
        /// <summary>
        /// Get TachyonDealer default commission settings for tenent
        /// </summary>
        /// <param name="offer"></param>
        /// <param name="shippingRequest"></param>
        private void SetCommssionSettingsForTachyonDealer(PriceOfferDto offer, ShippingRequest shippingRequest)
        {
            if (IsEnabled(AppFeatures.TachyonDealer))
            {
                offer.CommissionType = (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionType));
                if (offer.CommissionType == PriceOfferCommissionType.CommissionPercentage)
                {
                    offer.CommissionPercentageOrAddValue = Convert.ToDecimal(FeatureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionPercentage));
                }
                else
                {
                    offer.CommissionPercentageOrAddValue = Convert.ToDecimal(FeatureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerTripCommissionValue));

                }
                offer.VasCommissionType = (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionType));

                if (offer.VasCommissionType == PriceOfferCommissionType.CommissionPercentage)
                {
                    offer.VasCommissionPercentageOrAddValue = Convert.ToDecimal(FeatureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionPercentage));
                }
                else
                {
                    offer.VasCommissionPercentageOrAddValue = Convert.ToDecimal(FeatureChecker.GetValue(shippingRequest.TenantId, AppFeatures.TachyonDealerVasCommissionValue));

                }
            }

        }
        private PriceOfferTenantCommissionSettings SetTenantCommssionSettingsForTachyonDealer(int TenantId)
        {
            PriceOfferTenantCommissionSettings commssionSettings =
                new PriceOfferTenantCommissionSettings()
                {
                    ItemCommissionType = (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionType)),
                    ItemCommissionPercentage = Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionPercentage)),
                    ItemCommissionValue = Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionValue)),
                    ItemMinValueCommission = Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripMinValueCommission)),
                    VasCommissionType = (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerVasCommissionType)),
                    VasCommissionPercentage = Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerVasCommissionPercentage)),
                    VasCommissionValue = Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerVasCommissionValue)),
                    VasMinValueCommission = Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerVasMinValueCommission))
                };

            return commssionSettings;
        }
        /// <summary>
        /// When the request come from direct request page
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<List<GetShippingRequestForPriceOfferListDto>> GetFromDirectRequest(ShippingRequestForPriceOfferGetAllInput input)
        {
            var directRequests = _shippingRequestDirectRequestRepository
                            .GetAll()
                            .AsNoTracking()
                            .Include(carrier => carrier.Carrier)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(shipper => shipper.Tenant)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(oc => oc.OriginCityFk)
                                  .ThenInclude(x => x.Translations)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(dc => dc.ShippingRequestDestinationCities)
                                .ThenInclude(x=>x.CityFk)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(c => c.GoodCategoryFk)
                                    .ThenInclude(x => x.Translations)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(t => t.TrucksTypeFk)
                                    .ThenInclude(x => x.Translations)
                            .Where(r => /*r.Status != ShippingRequestDirectRequestStatus.Accepted &&*/ (r.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction || r.ShippingRequestFK.Status == ShippingRequestStatus.PrePrice || r.ShippingRequestFK.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier))
                            .WhereIf(input.ShippingRequestId.HasValue, x => x.ShippingRequestId == input.ShippingRequestId)
                            .WhereIf(input.DirectRequestId.HasValue, x => x.Id == input.DirectRequestId)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId && !x.ShippingRequestFK.IsTachyonDeal /*&& x.ShippingRequestFk.RequestType == ShippingRequestType.DirectRequest*/)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId && x.Status != ShippingRequestDirectRequestStatus.Declined && (x.ShippingRequestFK.RequestType == ShippingRequestType.DirectRequest || x.ShippingRequestFK.IsTachyonDeal))
                            .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.ShippingRequestFK.StartTripDate >= input.PickupFromDate.Value && x.ShippingRequestFK.StartTripDate <= input.PickupToDate.Value)
                            .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.CreationTime >= input.FromDate.Value && x.CreationTime <= input.ToDate.Value)
                            .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFK.OriginCityId == input.OriginId)
                            .WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestFK.ShippingRequestDestinationCities.Any(x => x.CityId == input.DestinationId))
                            .WhereIf(input.RouteTypeId.HasValue, x => x.ShippingRequestFK.RouteTypeId == input.RouteTypeId)
                            .WhereIf(input.TruckTypeId.HasValue, x => x.ShippingRequestFK.TrucksTypeId == input.TruckTypeId)
                            .WhereIf(input.Status.HasValue, x => x.Status == (ShippingRequestDirectRequestStatus)input.Status)
                            .WhereIf(input.IsTachyonDeal, x => x.ShippingRequestFK.IsTachyonDeal == input.IsTachyonDeal)
                            .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.ShippingRequestFK.Tenant.Name.ToLower().Contains(input.Filter) || x.ShippingRequestFK.Tenant.companyName.ToLower().Contains(input.Filter) || x.ShippingRequestFK.Tenant.TenancyName.ToLower().Contains(input.Filter))
                            .WhereIf(!string.IsNullOrEmpty(input.Carrier), x => x.Carrier.Name.ToLower().Contains(input.Carrier) || x.Carrier.companyName.ToLower().Contains(input.Carrier) || x.Carrier.TenancyName.ToLower().Contains(input.Carrier))

                            .OrderBy(input.Sorting ?? "id desc")
                            .PageBy(input);

            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();
            foreach (var request in await directRequests.ToListAsync())
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request.ShippingRequestFK);
                dto.DirectRequestStatusTitle = request.Status.GetEnumDescription();

                if (AbpSession.TenantId.HasValue && !IsEnabled(AppFeatures.Carrier))
                {
                    dto.Name = request.Carrier.Name;
                    dto.RemainingDays = string.Empty;

                }
                else if (AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) && request.Status == ShippingRequestDirectRequestStatus.Response)
                {
                    dto.DirectRequestStatusTitle = "WaitingForResponse";
                    
                    if (await _srUpdateManager.IsRequestHasBendingUpdates(request.ShippingRequestId,request.CarrierTenantId))
                    {
                        dto.DirectRequestStatusTitle = "PendingUpdate";
                    }
                }

                var offer = await _priceOfferManager.GetOfferBySource(request.Id, PriceOfferChannel.DirectRequest);
                if (offer != null)
                {
                    dto.isPriced = true;
                    dto.OfferId = offer.Id;
                }
                dto.DirectRequestId = request.Id;
                dto.CreationTime = request.CreationTime;
                dto.DirectRequestStatus = request.Status;
                dto.BidStatusTitle = string.Empty;
                dto.BidNormalPricePackageId = request.PricePackageOfferId;
                dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.ShippingRequestFK.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.ShippingRequestFK.GoodCategoryFk).DisplayName;
                dto.NumberOfCompletedTrips = await getCompletedRequestTripsCount(request.ShippingRequestFK);
                //dto.Longitude = (request.ShippingRequestFK.DestinationCityFk.Location != null ?
                //  request.ShippingRequestFK.DestinationCityFk.Location.X : 0);
                //dto.Latitude = (request.ShippingRequestFK.DestinationCityFk.Location != null ?
                //    request.ShippingRequestFK.DestinationCityFk.Location.Y : 0);
                dto.GoodCategoryId = request.ShippingRequestFK.GoodCategoryId;
                dto.OriginCity = ObjectMapper.Map<TenantCityLookupTableDto>(request.ShippingRequestFK.OriginCityFk).DisplayName;
                //dto.DestinationCity = ObjectMapper.Map<TenantCityLookupTableDto>(request.ShippingRequestFK.DestinationCityFk).DisplayName;
                dto.NotesCount = await GetRequestNotesCount(request.Id);

                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;

        }
        /// <summary>
        /// When the request come from market place page
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task<List<GetShippingRequestForPriceOfferListDto>> GetFromMarketPlace(ShippingRequestForPriceOfferGetAllInput input)
        {
            // ## This Method Need a lot of Code And Performance Improvements ##
            var query = _shippingRequestsRepository
                .GetAll()
                .AsNoTracking()
                    .Include(t => t.Tenant)
                    .Include(oc => oc.OriginCityFk)
                     .ThenInclude(x => x.Translations)
                    .Include(dc => dc.ShippingRequestDestinationCities)
                    .ThenInclude(x=>x.CityFk)
                    .Include(c => c.GoodCategoryFk)
                     .ThenInclude(x => x.Translations)
                    .Include(t => t.TrucksTypeFk)
                     .ThenInclude(x => x.Translations)
                .Where(x => x.IsBid && (x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice || x.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier))
                .Where(x=> !x.BidStartDate.HasValue || Clock.Now > x.BidStartDate)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal || (x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice))
                .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.StartTripDate >= input.PickupFromDate.Value && x.StartTripDate <= input.PickupToDate.Value)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.BidStartDate >= input.FromDate.Value && x.BidStartDate <= input.ToDate.Value)
                .WhereIf(input.OriginId.HasValue, x => x.OriginCityId == input.OriginId)
                //.WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestDestinationCities.Any(x => x.CityId == input.DestinationId))
                .WhereIf(input.RouteTypeId.HasValue, x => x.RouteTypeId == input.RouteTypeId)
                .WhereIf(input.TruckTypeId.HasValue, x => x.TrucksTypeId == input.TruckTypeId)
                .WhereIf(input.Status.HasValue, x => x.BidStatus == (ShippingRequestBidStatus)input.Status)
                .WhereIf(input.IsTachyonDeal, x => x.IsTachyonDeal == input.IsTachyonDeal)
                .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.Tenant.Name.ToLower().Contains(input.Filter) || x.Tenant.companyName.ToLower().Contains(input.Filter) || x.Tenant.TenancyName.ToLower().Contains(input.Filter))
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);


            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();
            foreach (var request in await query.ToListAsync())
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request);

                if (AbpSession.TenantId.HasValue && (await IsEnabledAsync(AppFeatures.Carrier) ||
                                                     (await IsEnabledAsync(AppFeatures.TachyonDealer) && !request.IsTachyonDeal)))
                {
                    dto.BidStatusTitle = "New";
                    var offer = _priceOfferManager.GetCarrierPricingOrNull(request.Id);
                    if (offer != null)
                    {
                        dto.OfferId = offer.Id;
                        dto.isPriced = true;
                        if (offer.Status == PriceOfferStatus.Accepted || offer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                            dto.BidStatusTitle = "Confirmed";
                        else if (await _srUpdateManager.IsRequestHasBendingUpdates(offer.Id))
                            dto.BidStatusTitle = "PendingUpdate";
                        else
                            dto.BidStatusTitle = "PriceSubmitted";
                    }

                    dto.StatusTitle = "";
                }
                dto.CreationTime = request.BidStartDate;
                dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.GoodCategoryFk).DisplayName;
                dto.NumberOfCompletedTrips = await getCompletedRequestTripsCount(request);
               // dto.Longitude = (request.DestinationCityFk != null ? (request.DestinationCityFk.Location != null ? request.DestinationCityFk.Location.X : 0) : 0);
               // dto.Latitude = (request.DestinationCityFk != null ? (request.DestinationCityFk.Location != null ? request.DestinationCityFk.Location.Y : 0) : 0);
                dto.OriginCity = ObjectMapper.Map<TenantCityLookupTableDto>(request.OriginCityFk).DisplayName;
               // dto.DestinationCity = ObjectMapper.Map<TenantCityLookupTableDto>(request.DestinationCityFk).DisplayName;
                dto.NotesCount = await GetRequestNotesCount(request.Id);
                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;
        }


        private async Task<List<GetShippingRequestForPriceOfferListDto>> GetFromShippingRequest(ShippingRequestForPriceOfferGetAllInput input)
        {

            var query = _shippingRequestsRepository
            .GetAll()
            .AsNoTracking()
                .Include(t => t.Tenant)
                .Include(c => c.CarrierTenantFk)
                .Include(oc => oc.OriginCityFk)
                .Include(dc => dc.ShippingRequestDestinationCities)
                .ThenInclude(x=>x.CityFk)
                .Include(c => c.GoodCategoryFk)
                 .ThenInclude(x => x.Translations)
                .Include(t => t.TrucksTypeFk)
                .Include(t => t.ShipperActorFk)
                .Include(t => t.CarrierActorFk)
            //.ThenInclude(x => x.Translations)
            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
            .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.StartTripDate >= input.PickupFromDate.Value && x.StartTripDate <= input.PickupToDate.Value)
            .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.CreationTime >= input.FromDate.Value && x.CreationTime <= input.ToDate.Value)
            .WhereIf(input.OriginId.HasValue, x => x.OriginCityId == input.OriginId)
            .WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestDestinationCities.Any(x => x.CityId == input.DestinationId))
            .WhereIf(input.RouteTypeId.HasValue, x => x.RouteTypeId == input.RouteTypeId)
            .WhereIf(input.TruckTypeId.HasValue, x => x.TrucksTypeId == input.TruckTypeId)
            .WhereIf(input.Status.HasValue, x => x.Status == (ShippingRequestStatus)input.Status)
            .WhereIf(input.RequestType.HasValue, x => x.RequestType == input.RequestType)
            .WhereIf(input.IsTachyonDeal, x => x.IsTachyonDeal == input.IsTachyonDeal)
            .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.Tenant.Name.ToLower().Contains(input.Filter) || x.Tenant.companyName.ToLower().Contains(input.Filter) || x.Tenant.TenancyName.ToLower().Contains(input.Filter))
            .WhereIf(!string.IsNullOrEmpty(input.Carrier), x => x.CarrierTenantFk.Name.ToLower().Contains(input.Carrier) || x.CarrierTenantFk.companyName.ToLower().Contains(input.Carrier) || x.CarrierTenantFk.TenancyName.ToLower().Contains(input.Carrier))
            .OrderBy(input.Sorting ?? "id desc")
            .PageBy(input);

            //get truck type tranlation list
            var goodscategoryIds = query.Select(x => x.TrucksTypeId).ToList();
            var truckTypeTranslationList = _truckTypeTranslationRepository.GetAll().Where(x => goodscategoryIds.Contains(x.CoreId)).ToList();


            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

            var isCarrier = await IsEnabledAsync(AppFeatures.Carrier);
            var isShipper = await IsEnabledAsync(AppFeatures.Shipper);


            foreach (var request in await query.ToListAsync())
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request);
                //dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.TrucksTypeFk)?.TranslatedDisplayName;
                dto.TruckType = truckTypeTranslationList.Where(x => x.CoreId == request.TrucksTypeId && x.Language == CultureInfo.CurrentCulture.Name).FirstOrDefault()?.TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.GoodCategoryFk)?.DisplayName;
                dto.NumberOfCompletedTrips = await getCompletedRequestTripsCount(request);
                if (AbpSession.TenantId.HasValue && (isCarrier))
                {

                    dto.Price = request.CarrierPrice;
                }
                else if (AbpSession.TenantId.HasValue && (isShipper))
                {
                    if (request.IsTachyonDeal)
                    {
                        dto.TotalOffers = _priceOfferManager.GetTotalOffersByTms(request.Id);
                    }
                }

                //dto.Longitude = (request.DestinationCityFk.Location != null ? request.DestinationCityFk.Location.X : 0);
                //dto.Latitude = (request.DestinationCityFk.Location != null ? request.DestinationCityFk.Location.Y : 0);
                dto.NotesCount = await GetRequestNotesCount(request.Id);
                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;
        }

        private async Task<List<GetShippingRequestForPriceOfferListDto>> GetFromOffers(ShippingRequestForPriceOfferGetAllInput input)
        {
            var offers = _priceOfferRepository
                            .GetAll()
                            .AsNoTracking()
                            .Include(carrier => carrier.Tenant)
                            .Include(r => r.ShippingRequestFk)
                                .ThenInclude(shipper => shipper.Tenant)
                            .Include(r => r.ShippingRequestFk)
                                .ThenInclude(oc => oc.OriginCityFk)
                            .Include(r => r.ShippingRequestFk)
                                .ThenInclude(dc => dc.ShippingRequestDestinationCities)
                                .ThenInclude(x=>x.CityFk)
                            .Include(r => r.ShippingRequestFk)
                                .ThenInclude(c => c.GoodCategoryFk)
                                    .ThenInclude(x => x.Translations)
                            .Include(r => r.ShippingRequestFk)
                                .ThenInclude(t => t.TrucksTypeFk)
                                    .ThenInclude(x => x.Translations)
                             .Where(r => (r.ShippingRequestFk.Status == ShippingRequestStatus.NeedsAction || r.ShippingRequestFk.Status == ShippingRequestStatus.PrePrice || r.ShippingRequestFk.Status == ShippingRequestStatus.PostPrice))
                            .WhereIf(input.ShippingRequestId.HasValue, x => x.ShippingRequestId == input.ShippingRequestId)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFk.TenantId == AbpSession.TenantId && !x.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFk.IsTachyonDeal)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId)
                            .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.ShippingRequestFk.StartTripDate >= input.FromDate.Value && x.ShippingRequestFk.StartTripDate <= input.ToDate.Value)
                            .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.CreationTime >= input.FromDate.Value && x.CreationTime <= input.ToDate.Value)
                            .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFk.OriginCityId == input.OriginId)
                            .WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestFk.ShippingRequestDestinationCities.Any(x => x.CityId == input.DestinationId))
                            .WhereIf(input.RouteTypeId.HasValue, x => x.ShippingRequestFk.RouteTypeId == input.RouteTypeId)
                            .WhereIf(input.TruckTypeId.HasValue, x => x.ShippingRequestFk.TrucksTypeId == input.TruckTypeId)
                            .WhereIf(input.Status.HasValue, x => x.Status == (PriceOfferStatus)input.Status)
                            .WhereIf(input.IsTachyonDeal, x => x.ShippingRequestFk.IsTachyonDeal == input.IsTachyonDeal)
                            .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.ShippingRequestFk.Tenant.Name.ToLower().Contains(input.Filter) || x.ShippingRequestFk.Tenant.companyName.ToLower().Contains(input.Filter) || x.ShippingRequestFk.Tenant.TenancyName.ToLower().Contains(input.Filter))
                            .WhereIf(!string.IsNullOrEmpty(input.Carrier), x => x.Tenant.Name.ToLower().Contains(input.Carrier) || x.Tenant.companyName.ToLower().Contains(input.Carrier) || x.Tenant.TenancyName.ToLower().Contains(input.Carrier))

                            .OrderBy(input.Sorting ?? "id desc")
                            .PageBy(input);

            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

            foreach (var request in await offers.ToListAsync())
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request.ShippingRequestFk);
                dto.DirectRequestStatusTitle = request.Status.GetEnumDescription();
                dto.OfferStatus = request.Status;
                dto.NumberOfCompletedTrips = await getCompletedRequestTripsCount(request.ShippingRequestFk);

                if (!AbpSession.TenantId.HasValue || IsEnabled(AppFeatures.TachyonDealer) || IsEnabled(AppFeatures.Shipper))
                {
                    dto.Name = request.Tenant?.Name;
                    dto.RemainingDays = string.Empty;
                    if (AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Shipper) && request.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier)
                    {
                        dto.DirectRequestStatusTitle = PriceOfferStatus.Accepted.GetEnumDescription();

                    }
                    dto.Price = request.TotalAmountWithCommission;

                }
                else if (AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier))
                {
                    dto.Price = request.TotalAmount;
                    if (request.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                        dto.DirectRequestStatusTitle = PriceOfferStatus.Accepted.GetEnumDescription();
                }
                dto.isPriced = true;
                dto.OfferId = request.Id;
                dto.DirectRequestId = request.Id;
                dto.CreationTime = request.CreationTime;
                dto.OfferStatus = request.Status;
                //dto.Longitude = (request.ShippingRequestFk.DestinationCityFk.Location != null ?
                //    request.ShippingRequestFk.DestinationCityFk.Location.X : 0);
                //dto.Latitude = (request.ShippingRequestFk.DestinationCityFk.Location != null ?
                //    request.ShippingRequestFk.DestinationCityFk.Location.Y : 0);
                dto.BidStatusTitle = string.Empty;
                dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.ShippingRequestFk.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.ShippingRequestFk.GoodCategoryFk).DisplayName;
                dto.GoodCategoryId = request.ShippingRequestFk.GoodCategoryId;
                dto.NotesCount = await GetRequestNotesCount(request.Id);
                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;

        }

        private async Task<int> getCompletedRequestTripsCount(ShippingRequest request)
        {
            return await _shippingRequestTripRepository.CountAsync(x => x.ShippingRequestId == request.Id && x.Status == ShippingRequestTripStatus.Delivered);
        }

        private async Task<int> GetRequestNotesCount(long SRId)
        {
            DisableTenancyFilters();
            return await _ShippingRequestAndTripNoteRepository.GetAll()
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),
                    x => x.TenantId == AbpSession.TenantId ||
                    (x.TenantId != AbpSession.TenantId && (x.Visibility == VisibilityNotes.ShipperOnly || x.Visibility == VisibilityNotes.Internal)))
                .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier),
                    x => ((x.ShippingRequestFK.CarrierTenantId == AbpSession.TenantId || x.ShippingRequestFK.CarrierTenantIdForDirectRequest == AbpSession.TenantId) && 
                    (x.Visibility == VisibilityNotes.Internal ||
                    x.Visibility == VisibilityNotes.CarrierOnly ||
                    x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                    (x.TenantId == AbpSession.TenantId)
                    )
              .WhereIf(AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.TachyonDealer),
                    x => (x.ShippingRequestFK.IsTachyonDeal &&
                   (x.Visibility == VisibilityNotes.TMSOnly
                   || x.Visibility == VisibilityNotes.TMSAndCarrier)) ||
                   (x.TenantId == AbpSession.TenantId)
                   )
                .CountAsync(x => x.ShippingRequetId == SRId);
        }

        private async Task CancelTrips(long Id)
        {
            var trips = await _shippingRequestTripRepository.GetAll().Where(x => x.ShippingRequestId == Id && x.Status == ShippingRequestTripStatus.New).ToListAsync();
            trips.ForEach(t =>
            {
                t.Status = ShippingRequestTripStatus.Canceled;
            });
        }

        public async Task CancelShipment(CancelShippingRequestInput input)
        {
            DisableTenancyFilters();
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            var request = await _shippingRequestsRepository
                .GetAll()
                .Where(x => x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .FirstOrDefaultAsync(x => x.Id == input.Id);
            if (request == null) throw new UserFriendlyException(L("YouCanNotCancelThisShipment"));
            request.CancelReason = input.CancelReason;
            request.Status = ShippingRequestStatus.Cancled;
            await CancelTrips(request.Id);
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                var userAdmin = await UserManager.GetAdminByTenantIdAsync(request.TenantId);
                if (request.CreatorUserId == null)
                {
                    await _appNotifier.CancelShipment(request.Id, request.CancelReason, L(AppConsts.TMS), new UserIdentifier(request.TenantId, userAdmin.Id));
                }
                else
                {
                    await _appNotifier.CancelShipment(request.Id, request.CancelReason, L(AppConsts.TMS), new UserIdentifier(request.TenantId, request.CreatorUserId.Value));
                }
            }
            else if (await IsEnabledAsync(AppFeatures.Shipper) && request.IsTachyonDeal)
            {
                var user = await UserManager.GetAdminTachyonDealerAsync();
                await _appNotifier.CancelShipment(request.Id, request.CancelReason, L(AppConsts.TMS), new UserIdentifier(user.TenantId, user.Id));
            }
        }
        #endregion
    }
}