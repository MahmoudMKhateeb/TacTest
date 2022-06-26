using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityHistory;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Abp.Threading;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Castle.Core.Internal;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Ports;
using TACHYON.Authorization;
using TACHYON.Cities;
using TACHYON.Common;
using TACHYON.Documents;
using TACHYON.Dto;
using TACHYON.EntityLogs.Transactions;
using TACHYON.Extension;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Invoices;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Offers;
using TACHYON.Packing.PackingTypes;
using TACHYON.Packing.PackingTypes.Dtos;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.PricePackages;
using TACHYON.Receivers;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutSteps;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.TachyonDealer;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.Shipping.ShippingTypes.Dtos;
using TACHYON.Shipping.SrPostPriceUpdates;
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
using TACHYON.UnitOfMeasures.Dtos;
using TACHYON.Vases;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
    public class ShippingRequestsAppService : TACHYONAppServiceBase, IShippingRequestsAppService
    {
        public ShippingRequestsAppService(IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<ShippingRequestTripVas, long> shippingRequestTripVasRepository,
            IRepository<ShippingType, int> shippingTypeRepository,
            IRepository<PackingType, int> packingTypeRepository,
            IRepository<UnitOfMeasure, int> unitOdMeasureRepository,
            IAppNotifier appNotifier,
            IRepository<Tenant> tenantRepository,
            IRepository<TrucksType, long> lookupTrucksTypeRepository,
            IRepository<TrailerType, int> lookupTrailerTypeRepository,
            IRepository<GoodCategory, int> lookupGoodCategoryRepository,
            IRepository<Vas, int> lookup_vasRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository,
            IRepository<VasPrice> vasPriceRepository,
            IRepository<Port, long> lookupPortRepository,
            BidDomainService bidDomainService,
            IRepository<Capacity, int> capacityRepository,
            IRepository<TransportType, int> transportTypeRepository,
            IRepository<RoutPoint, long> routPointRepository,
            IRepository<ShippingRequestsCarrierDirectPricing> carrierDirectPricingRepository,
            PriceOfferManager priceOfferManager,
            IRepository<InvoiceTrip, long> invoiveTripRepository,
            ShippingRequestDirectRequestAppService shippingRequestDirectRequestAppService,
            ShippingRequestDirectRequestManager shippingRequestDirectRequestManager,
            DocumentFilesManager documentFilesManager,
            IRepository<PriceOffer, long> priceOfferRepository,
            IEntityChangeSetReasonProvider reasonProvider,
            NormalPricePackageManager normalPricePackageManager,
            SrPostPriceUpdateManager postPriceUpdateManager)
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
            _bidDomainService = bidDomainService;
            _lookup_vasRepository = lookup_vasRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
            _capacityRepository = capacityRepository;
            _transportTypeRepository = transportTypeRepository;
            _routPointRepository = routPointRepository;
            _carrierDirectPricingRepository = carrierDirectPricingRepository;
            _priceOfferManager = priceOfferManager;
            _InvoiveTripRepository = invoiveTripRepository;
            _shippingRequestDirectRequestAppService = shippingRequestDirectRequestAppService;
            _shippingRequestDirectRequestManager = shippingRequestDirectRequestManager;
            _documentFilesManager = documentFilesManager;
            _priceOfferRepository = priceOfferRepository;
            _reasonProvider = reasonProvider;
            _normalPricePackageManager = normalPricePackageManager;
            _postPriceUpdateManager = postPriceUpdateManager;
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
        private readonly SrPostPriceUpdateManager _postPriceUpdateManager;
        private readonly IRepository<RoutPoint, long> _routPointRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<TrailerType, int> _lookup_trailerTypeRepository;
        private readonly IRepository<GoodCategory, int> _lookup_goodCategoryRepository;
        private readonly IRepository<Port, long> _lookup_PortRepository;
        private readonly BidDomainService _bidDomainService;
        private readonly IRepository<Capacity, int> _capacityRepository;
        private readonly IRepository<TransportType, int> _transportTypeRepository;
        private readonly IRepository<ShippingRequestTripVas, long> _shippingRequestTripVasRepository;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IRepository<InvoiceTrip, long> _InvoiveTripRepository;
        private readonly ShippingRequestDirectRequestAppService _shippingRequestDirectRequestAppService;
        private readonly ShippingRequestDirectRequestManager _shippingRequestDirectRequestManager;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IEntityChangeSetReasonProvider _reasonProvider;
        private readonly NormalPricePackageManager _normalPricePackageManager;

        public async Task<GetAllShippingRequestsOutputDto> GetAll(GetAllShippingRequestsInput Input)
        {
            DisableTenancyFilters();
            //using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            //{
            IQueryable<ShippingRequest> query = _shippingRequestRepository
                .GetAll()
                .AsNoTracking()
                .Include(t => t.Tenant)
                .Include(x => x.OriginCityFk)
                .Include(x => x.DestinationCityFk)
                .WhereIf(Input.IsBid.HasValue, e => e.IsBid == Input.IsBid.Value)
                .WhereIf(Input.Status.HasValue, e => e.Status == Input.Status.Value)
                .WhereIf(Input.IsPricedWihtoutTrips.HasValue,
                    e => e.Status == ShippingRequestStatus.PostPrice && e.TotalsTripsAddByShippier == 0)
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer), e => e.IsTachyonDeal) //if the user is TachyonDealer
                .WhereIf(IsEnabled(AppFeatures.Carrier),
                    e => e.CarrierTenantId == AbpSession.TenantId) //if the user is carrier
                .WhereIf(IsEnabled(AppFeatures.Shipper),
                    e => e.TenantId == AbpSession.TenantId) //if the user is shipper
                .OrderBy(Input.Sorting ?? "id desc");

            //var myDraftsOnly = query.Where(x => x.TenantId == AbpSession.TenantId)
            //             .Where(x => x.IsDrafted);

            //var withoutDrafts = query.Where(x => !x.IsDrafted);
            ////concat all requests without draft with my draft requests
            //var allWithMyDraftsOnly = myDraftsOnly.Concat(withoutDrafts);

            //var ResultPage = allWithMyDraftsOnly.PageBy(Input)
            //var totalCount = await allWithMyDraftsOnly.CountAsync();
            var totalCount = await query.CountAsync();

            //var output = ObjectMapper.Map<List<ShippingRequestListDto>>(await ResultPage.ToListAsync());
            var output = ObjectMapper.Map<List<ShippingRequestListDto>>(await query.ToListAsync());

            foreach (var item in output.Where(x => IsEnabled(AppFeatures.Shipper) && x.IsTachyonDeal))
            {
                item.TotalBids = 0;
            }

            return new GetAllShippingRequestsOutputDto()
            {
                Data = new PagedResultDto<ShippingRequestListDto>(
                    totalCount, output
                ),
                NoOfPostPriceWithoutTrips = IsEnabled(AppFeatures.Shipper)
                    ? _shippingRequestRepository.GetAll().Where(r =>
                        r.Status == ShippingRequestStatus.PostPrice && r.TotalsTripsAddByShippier == 0 &&
                        r.TenantId == AbpSession.TenantId).Count()
                    : 0
            };
            // }
        }

        public async Task<LoadResult> GetAllShippingRequstHistory(LoadOptionsInput input)
        {
            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();
            var query = _shippingRequestRepository
                .GetAll().AsNoTracking()
                .Include(t => t.Tenant)
                .Include(x => x.CarrierTenantFk)
                .Where(x => x.Status == ShippingRequestStatus.Completed || x.Status == ShippingRequestStatus.Cancled)
                .WhereIf(IsEnabled(AppFeatures.Carrier),
                    e => e.CarrierTenantId == AbpSession.TenantId) //if the user is carrier
                .WhereIf(IsEnabled(AppFeatures.Shipper),
                    e => e.TenantId == AbpSession.TenantId) //if the user is shipper
                .ProjectTo<ShipmentHistoryDto>(AutoMapperConfigurationProvider);
            return await LoadResultAsync(query, input.LoadOptions);
        }

        public async Task<GetShippingRequestForViewOutput> GetShippingRequestForView(long id)
        {
            return await _GetShippingRequestForView(id);
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.CarrierAsASaas)]
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
        public async Task<long> CreateOrEditStep1(CreateOrEditShippingRequestStep1Dto input)
        {
            if (input.IsTachyonDeal)
            {
                if (!await IsEnabledAsync(AppFeatures.SendTachyonDealShippingRequest))
                {
                    // throw new UserFriendlyException(L("feature SendTachyonDealShippingRequest not enabled"));
                }
            }
            else if (input.IsDirectRequest)
            {
                if (!await IsEnabledAsync(AppFeatures.SendDirectRequest) && !await IsEnabledAsync(AppFeatures.CarrierAsASaas))
                {
                    throw new UserFriendlyException(L("feature SendDirectRequest not enabled"));
                }
            }

            if (input.Id == null)
            {
                return await CreateStep1(input);
            }
            else
            {
                return await UpdateStep1(input);
            }
        }

        public async Task<CreateOrEditShippingRequestStep1Dto> GetStep1ForEdit(EntityDto<long> entity)
        {
            var shippingRequest = await GetDraftedShippingRequest(entity.Id);
            return ObjectMapper.Map<CreateOrEditShippingRequestStep1Dto>(shippingRequest);
        }

        /// <summary>
        /// Route Details - Shipping Request Wizard
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task EditStep2(EditShippingRequestStep2Dto input)
        {
            if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            var shippingRequest = await GetDraftedShippingRequest(input.Id);
            if (shippingRequest.DraftStep < 2)
            {
                shippingRequest.DraftStep = 2;
            }

            ObjectMapper.Map(input, shippingRequest);
        }

        public async Task<EditShippingRequestStep2Dto> GetStep2ForEdit(EntityDto<long> entity)
        {
            var shippingRequest = await GetDraftedShippingRequest(entity.Id);
            return ObjectMapper.Map<EditShippingRequestStep2Dto>(shippingRequest);
        }

        /// <summary>
        /// Goods Details - Shipping Request Wizard
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task EditStep3(EditShippingRequestStep3Dto input)
        {
            if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            await OthersNameValidation(input);

            var shippingRequest = await GetDraftedShippingRequest(input.Id);
            if (shippingRequest.DraftStep < 3)
            {
                shippingRequest.DraftStep = 3;
            }

            ObjectMapper.Map(input, shippingRequest);
        }


        public async Task<EditShippingRequestStep3Dto> GetStep3ForEdit(EntityDto<long> entity)
        {
            var shippingRequest = await GetDraftedShippingRequest(entity.Id);
            return ObjectMapper.Map<EditShippingRequestStep3Dto>(shippingRequest);
        }

        /// <summary>
        /// Services - Shipping Request Wizard
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task EditStep4(EditShippingRequestStep4Dto input)
        {
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
                {
                    DisableTenancyFilters();
                }

                ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                    .Include(x => x.ShippingRequestVases)
                    .Where(x => x.Id == input.Id && x.IsDrafted == true)
                    .FirstOrDefaultAsync();

                await ShippingRequestVasListValidate(input, shippingRequest.NumberOfTrips);
                //delete vases
                foreach (var vas in shippingRequest.ShippingRequestVases)
                {
                    if (!input.ShippingRequestVasList.Any(x => x.Id == vas.Id))
                    {
                        await _shippingRequestVasRepository.DeleteAsync(vas);
                    }
                }

                if (shippingRequest.DraftStep < 4)
                {
                    shippingRequest.DraftStep = 4;
                }

                ObjectMapper.Map(input, shippingRequest);
            }
        }


        public async Task<EditShippingRequestStep4Dto> GetStep4ForEdit(EntityDto<long> entity)
        {
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                    .Include(x => x.ShippingRequestVases)
                    .Where(x => x.Id == entity.Id && x.IsDrafted == true)
                    .FirstOrDefaultAsync();
                return ObjectMapper.Map<EditShippingRequestStep4Dto>(shippingRequest);
            }
            //await ValidateShippingRequestBeforePublish(shippingRequest);
            //shippingRequest.IsDrafted = false;
        }

        public async Task PublishShippingRequest(long id)
        {
            if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                DisableTenancyFilters();
            }

            ShippingRequest shippingRequest;
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                shippingRequest = await _shippingRequestRepository.GetAll()
                    .Include(x => x.ShippingRequestVases)
                 .Where(x => x.Id == id && x.IsDrafted == true)
                 .FirstOrDefaultAsync();
            }

            if (shippingRequest.DraftStep < 4)
            {
                throw new UserFriendlyException(L("YouMustCompleteWizardStepsFirst"));
            }

            await ValidateShippingRequestBeforePublish(shippingRequest);
            // _commissionManager.AddShippingRequestCommissionSettingInfo(shippingRequest);
            shippingRequest.IsDrafted = false;
            //to make SR to non drafted .. 
            //CurrentUnitOfWork.SaveChanges();
            if (!shippingRequest.IsSaas())
            {
                await SendtoCarrierIfShippingRequestIsDirectRequest(shippingRequest);
            }

            // handle carrier as saas SR pricing
            if (shippingRequest.IsSaas())
            {
                decimal carrierAsSaasCommissionValue = Convert.ToDecimal(await FeatureChecker.GetValueAsync(shippingRequest.TenantId, AppFeatures.CarrierAsSaasCommissionValue));


                var itemDetails = new List<PriceOfferDetailDto>();
                foreach (ShippingRequestVas vas in shippingRequest.ShippingRequestVases)
                {

                    itemDetails.Add(new PriceOfferDetailDto
                    {
                        ItemId = vas.Id,
                        Price = 0,
                        CommissionPercentageOrAddValue = 0,
                        PriceType = PriceOfferType.Vas,
                        CommissionType = PriceOfferCommissionType.CommissionValue
                    }
                    );
                }


                var input = new CreateOrEditPriceOfferInput
                {
                    ShippingRequestId = shippingRequest.Id,
                    ItemPrice = 0,
                    Channel = PriceOfferChannel.CarrierAsSaas,
                    ParentId = null,
                    PriceType = PriceOfferType.Trip,
                    //ignor vas's
                    ItemDetails = itemDetails,
                    CommissionPercentageOrAddValue = carrierAsSaasCommissionValue,
                    CommissionType = PriceOfferCommissionType.CommissionValue,
                    VasCommissionPercentageOrAddValue = 0,
                    VasCommissionType = PriceOfferCommissionType.CommissionValue,
                    SourceId = null
                };


                var offer = await _priceOfferManager.InitPriceOffer(input);
                _priceOfferManager.SetShippingRequestPricing(offer);

                offer.Status = PriceOfferStatus.Accepted;

                shippingRequest.Status = ShippingRequestStatus.PostPrice;

                await _priceOfferRepository.InsertAsync(offer);

            }
        }


        private async Task<ShippingRequest> GetDraftedShippingRequest(long id)
        {
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            {
                DisableTenancyFilters();
                ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                    .WhereIf(await IsTachyonDealer(), x => x.IsTachyonDeal == true)
                  .Where(x => x.Id == id && x.IsDrafted == true)
                  .FirstOrDefaultAsync();
                return shippingRequest;
            }
        }

        private async Task<long> CreateStep1(CreateOrEditShippingRequestStep1Dto input)
        {
            ShippingRequest shippingRequest = ObjectMapper.Map<ShippingRequest>(input);
            if (input.ShipperId.HasValue)
            {
                shippingRequest.TenantId = input.ShipperId.Value;
            }

            if (await IsCarrier() && await IsEnabledAsync(AppFeatures.CarrierAsASaas))
            {
                shippingRequest.TenantId = AbpSession.TenantId.Value;
                shippingRequest.CarrierTenantId = AbpSession.TenantId.Value;
            }

            //ValidateStep1(shippingRequest);
            shippingRequest.CreatedByTachyonDealer = await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer);
            shippingRequest.IsDrafted = true;
            shippingRequest.DraftStep = 1;
            await _shippingRequestRepository.InsertAndGetIdAsync(shippingRequest);
            await CurrentUnitOfWork.SaveChangesAsync();
            return shippingRequest.Id;
        }

        private async Task<long> UpdateStep1(CreateOrEditShippingRequestStep1Dto input)
        {
            var shippingRequest = await GetDraftedShippingRequest(input.Id.Value);

            ObjectMapper.Map(input, shippingRequest);
            return shippingRequest.Id;
            // ValidateStep1(shippingRequest);
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

                shippingRequest.BidStatus = shippingRequest.BidStartDate.Value.Date == Clock.Now.Date
                    ? ShippingRequestBidStatus.OnGoing
                    : ShippingRequestBidStatus.StandBy;
                await SendNotificationToCarriersWithTheSameTrucks(shippingRequest);
            }
        }

        private async Task SendtoCarrierIfShippingRequestIsDirectRequest(ShippingRequest shippingRequest)
        {
            if (shippingRequest.IsDirectRequest && shippingRequest.CarrierTenantIdForDirectRequest.HasValue)
            {
                var directRequestInput = new CreateShippingRequestDirectRequestInput();
                directRequestInput.CarrierTenantId = shippingRequest.CarrierTenantIdForDirectRequest.Value;
                directRequestInput.ShippingRequestId = shippingRequest.Id;
                await _shippingRequestDirectRequestAppService.Create(directRequestInput);
            }
        }

        #endregion

        [RequiresFeature(AppFeatures.ShippingRequest, AppFeatures.CarrierAsASaas)]
        public async Task CreateOrEdit(CreateOrEditShippingRequestDto input)
        {
            await OthersNameValidation(input);

            if (input.IsTachyonDeal)
            {
                if (!await IsEnabledAsync(AppFeatures.SendTachyonDealShippingRequest))
                {
                    throw new UserFriendlyException(L("feature SendTachyonDealShippingRequest not enabled"));
                }
            }
            else if (input.IsDirectRequest)
            {
                if (!await IsEnabledAsync(AppFeatures.SendDirectRequest) && !await IsEnabledAsync(AppFeatures.CarrierAsASaas))
                {
                    throw new UserFriendlyException(L("feature SendDirectRequest not enabled"));
                }
            }

            // Vas validation
            await ShippingRequestVasListValidate(input, input.NumberOfTrips);

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
                    var vas = await _shippingRequestVasRepository.FirstOrDefaultAsync(x =>
                        x.Id == item.ShippingRequestVasId);
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

                await _appNotifier.UpdateShippingRequestPrice(
                    new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value), input.Id,
                    input.Price);
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

                await _appNotifier.RejectShippingRequest(
                    new UserIdentifier(shippingRequest.TenantId, shippingRequest.CreatorUserId.Value), id);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Delete)]
        [RequiresFeature(AppFeatures.ShippingRequest)]
        public async Task Delete(EntityDto<long> input)
        {
            //Disable Tenancy filter to allow to Tachyon Dealer to delete the Drafted Requests | TAC-2331
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant,
                       nameof(IHasIsDrafted)))
            {
                var tenantId = AbpSession.TenantId;
                var shippingRequest = await _shippingRequestRepository.GetAll()
                    // allow to shipper to delete his drafted requests  and to carrier if he is a Shipping request Creator
                    .WhereIf(tenantId.HasValue && !IsEnabled(AppFeatures.TachyonDealer), x => x.TenantId == tenantId)
                    .WhereIf(!tenantId.HasValue || IsEnabled(AppFeatures.TachyonDealer), x => true)
                    .Where(x => x.Id == input.Id && x.IsDrafted == true).FirstOrDefaultAsync();

                if (shippingRequest == null) throw new UserFriendlyException(L("TheShippingRequestDoesNotExits"));

                await _shippingRequestRepository.DeleteAsync(shippingRequest);
            }
        }

        public async Task<List<CarriersForDropDownDto>> GetAllCarriersForDropDownAsync()
        {
            return await _shippingRequestDirectRequestManager.GetCarriersForDropDownByPermissionAsync();
        }

        public async Task<List<ShippersForDropDownDto>> GetAllShippersForDropDownAsync()
        {
            return await _tenantRepository.GetAll()
                .Where(x => x.Edition.Id == ShipperEditionId)
                .Select(x => new ShippersForDropDownDto { Id = x.Id, DisplayName = x.TenancyName }).ToListAsync();
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
                .Select(trailerType => new SelectItemDto
                {
                    Id = trailerType.Id.ToString(),
                    DisplayName = trailerType == null || trailerType.DisplayName == null
                        ? ""
                        : trailerType.DisplayName.ToString()
                }).ToListAsync();
        }

        public async Task<List<GetAllGoodsCategoriesForDropDownOutput>> GetAllGoodCategoriesForTableDropdown()
        {
            var list = await _lookup_goodCategoryRepository.GetAll()
                .Where(x => x.IsActive)
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
            using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted")) // for wizard last step 
            {
                DisableTenancyFilters();


                ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                    .Where(e => e.Id == id)
                    .Include(e => e.Tenant)
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
                    .ThenInclude(v => v.Translations)
                    .Include(e => e.CarrierTenantFk)
                    .FirstOrDefaultAsync();

                bool isShipper = await IsEnabledAsync(AppFeatures.Shipper);
                bool isCarrier = await IsEnabledAsync(AppFeatures.Carrier);
                int? abpSessionTenantId = AbpSession.TenantId;

                async Task<bool> IsCarrierSaasAndSrOwner()
                {
                    return await FeatureChecker.IsEnabledAsync(AppFeatures.CarrierAsASaas) &&
                           shippingRequest.TenantId == AbpSession.TenantId;
                }

                // shippers access
                if (abpSessionTenantId!=null && shippingRequest.TenantId != abpSessionTenantId && isShipper)
                {
                    throw new UserFriendlyException("You cant view this shipping request msg");
                }

                //carrier access if he is not assigned to the SR
                if (abpSessionTenantId != null && !await IsCarrierSaasAndSrOwner() && isCarrier && shippingRequest.CarrierTenantId != abpSessionTenantId)
                {
                    //if PrePrice or NeedsAction
                    if (shippingRequest.Status == ShippingRequestStatus.PrePrice ||
                        shippingRequest.Status == ShippingRequestStatus.NeedsAction)
                    {
                        var carrierHasOffers = _carrierDirectPricingRepository.GetAll().Any
                        (
                            e =>
                                e.RequestId == id && e.CarrirerTenantId == abpSessionTenantId
                        );
                        // if carrier has no offers 
                        if (!carrierHasOffers)
                        {
                            throw new UserFriendlyException("You cant view this shipping request msg");
                        }
                    }
                    else
                    {
                        throw new UserFriendlyException("You cant view this shipping request msg");
                    }
                }

                //VAS
                var shippingRequestVasList = await _shippingRequestVasRepository.GetAll()
                    .Where(x => x.ShippingRequestId == id)
                    .Select(e =>
                        new GetShippingRequestVasForViewDto
                        {
                            ShippingRequestVas = ObjectMapper.Map<ShippingRequestVasDto>(e), VasName = e.VasFk.Key
                        }).ToListAsync();

                //Bids
                List<ShippingRequestBidDto> shippingRequestBidDtoList = new List<ShippingRequestBidDto>();
                //hid bids for shipper if SR IsTachyonDeal 
                if (isShipper && shippingRequest.IsTachyonDeal)
                {
                    // don't fill bids list
                }
                else
                {
                    shippingRequestBidDtoList =
                        ObjectMapper.Map<List<ShippingRequestBidDto>>(shippingRequest.ShippingRequestBids);
                }


                GetShippingRequestForViewOutput output =
                    ObjectMapper.Map<GetShippingRequestForViewOutput>(shippingRequest);

                //output.ShippingRequest.AddTripsByTmsEnabled =
                //    await FeatureChecker.IsEnabledAsync(shippingRequest.TenantId, AppFeatures.AddTripsByTachyonDeal);


                output.ShippingRequest.CanAddTrip = await CanCurrentUserAddTrip(shippingRequest);
                output.ShippingRequestBidDtoList = shippingRequestBidDtoList;
                output.ShippingRequestVasDtoList = shippingRequestVasList;
                output.ShipperRating = shippingRequest.Tenant.Rate;
                output.ShipperRatingNumber = shippingRequest.Tenant.RateNumber;
                //return translated good category name by default language
                output.GoodsCategoryName =
                    ObjectMapper.Map<GoodCategoryDto>(shippingRequest.GoodCategoryFk).DisplayName;



                //return translated Packing Type name by current language
                if (shippingRequest.PackingTypeFk != null)
                    output.packingTypeDisplayName =
                        ObjectMapper.Map<PackingTypeDto>(shippingRequest.PackingTypeFk).DisplayName;



                //return translated truck type by default language
                output.TruckTypeDisplayName =
                    ObjectMapper.Map<TrucksTypeDto>(shippingRequest.TrucksTypeFk).TranslatedDisplayName;
                output.TruckTypeFullName = ObjectMapper.Map<TransportTypeDto>(shippingRequest.TransportTypeFk)
                                               .TranslatedDisplayName
                                           + "-" + output.TruckTypeDisplayName
                                           + "-" + ObjectMapper.Map<CapacityDto>(shippingRequest.CapacityFk)
                                               .TranslatedDisplayName;

                return output;
            }
        }

        public async Task<bool> CanAddTripForShippingRequest(long shippingRequestId)
        {
            DisableTenancyFilters();
            var request = await _shippingRequestRepository.GetAsync(shippingRequestId);

            return await CanCurrentUserAddTrip(request);
        }
        private async Task<bool> CanCurrentUserAddTrip(ShippingRequest request)
        {


            //CarrierSaas
            if (request.IsSaas() && AbpSession.TenantId == request.TenantId && await FeatureChecker.IsEnabledAsync(AppFeatures.CarrierAsASaas))
            {
                return true;
            }


            // TripsByTMS
            if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer)) // false 
            {

                return true;
            }

            //Shipper
            if (request.TenantId == AbpSession.TenantId && await IsEnabledAsync(AppFeatures.Shipper))
            {
                return true;
            }

            return false;

        }



        [AbpAuthorize(AppPermissions.Pages_ShippingRequestTrips_Create)]

        protected virtual GetShippingRequestForEditOutput _GetShippingRequestForEdit(EntityDto<long> input)
        {
            //using (CurrentUnitOfWork.DisableFilter("IHasIsDrafted"))
            //{
            ShippingRequest shippingRequest = _shippingRequestRepository
                .GetAll()
                .Include(x => x.ShippingRequestVases)
                .Single(x => x.Id == input.Id);
            var Request = ObjectMapper.Map<CreateOrEditShippingRequestDto>(shippingRequest);
            Request.ShippingRequestVasList =
                ObjectMapper.Map<List<CreateOrEditShippingRequestVasListDto>>(shippingRequest.ShippingRequestVases);

            if (shippingRequest.TenantId != AbpSession.TenantId && shippingRequest.IsDrafted)
            {
                return null;
            }

            GetShippingRequestForEditOutput output = new GetShippingRequestForEditOutput { ShippingRequest = Request };
            return output;
            // }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Create)]
        protected virtual async Task Create(CreateOrEditShippingRequestDto input)
        {
            var vasList = input.ShippingRequestVasList;

            await ValidateGoodsCategory(input);

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

                    shippingRequest.BidStatus = shippingRequest.BidStartDate.Value.Date == Clock.Now.Date
                        ? ShippingRequestBidStatus.OnGoing
                        : ShippingRequestBidStatus.StandBy;
                }

                // _commissionManager.AddShippingRequestCommissionSettingInfo(shippingRequest);
            }

            // todo Add this Validation in Update Shipping Request


            await _shippingRequestRepository.InsertAndGetIdAsync(shippingRequest);


            await CurrentUnitOfWork.SaveChangesAsync();
            if (shippingRequest.IsBid)
            {
                //Notify Carrier with the same Truck type
                await SendNotificationToCarriersWithTheSameTrucks(shippingRequest);
            }
        }

        /// <summary>
        /// Used to check if the updated shipping request has any offer
        /// and if it has any offer this method will notify all offers owners.
        /// please use this method for pre-priced shipping requests only
        /// </summary>
        /// <param name="request"></param>
        private async Task CheckHasOffersToNotifyCarriers(ShippingRequest request)
        {
            DisableTenancyFilters();
            var carriers = await _priceOfferRepository.GetAll()
                .Where(x => x.ShippingRequestId == request.Id)
                .Select(x => x.TenantId).ToArrayAsync();

            if (request.RequestType == ShippingRequestType.DirectRequest)
            {
                await _appNotifier.NotifyOfferOwnerWhenDirectRequestSrUpdated(request.Id, request.ReferenceNumber,
                    carriers);
                return;
            }
                
            await _appNotifier.NotifyOfferOwnerWhenMarketplaceSrUpdated(request.Id, request.ReferenceNumber, carriers);
        }
        private async Task ValidateGoodsCategory(CreateOrEditShippingRequestDto input)
        {
            if (input.GoodCategoryId != null)
            {
                var goodCategory = await _lookup_goodCategoryRepository.GetAsync(input.GoodCategoryId.Value);
                if (goodCategory.FatherId != null)
                    throw new UserFriendlyException(L("GoodsCategoryMustBeMainNotSub"));
            }
        }

        private async Task SendNotificationToCarriersWithTheSameTrucks(ShippingRequest shippingRequest)
        {
            if (shippingRequest.BidStatus == ShippingRequestBidStatus.OnGoing)
            {
                UserIdentifier[] users =
                    await _bidDomainService.GetCarriersByTruckTypeArrayAsync(shippingRequest.TrucksTypeId.Value);
                await _appNotifier.ShippingRequestAsBidWithSameTruckAsync(users, shippingRequest.Id);

                var carriers = await _normalPricePackageManager.GetCarriersMatchingPricePackages(shippingRequest.TrucksTypeId, shippingRequest.OriginCityId, shippingRequest.DestinationCityId);
                await _appNotifier.ShippingRequestAsBidWithMatchingPricePackage(carriers, shippingRequest.ReferenceNumber, shippingRequest.Id);
            }
        }


        [AbpAuthorize(AppPermissions.Pages_ShippingRequests_Edit)]
        protected virtual async Task Update(CreateOrEditShippingRequestDto input)
        {
            _reasonProvider.Use(nameof(UpdateShippingRequestTransaction));

            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                .Where(x => x.Id == (long)input.Id)
                .FirstOrDefaultAsync();

            if (shippingRequest.Status == ShippingRequestStatus.PostPrice || shippingRequest.CarrierTenantId.HasValue)
            {
                await _postPriceUpdateManager.Create(shippingRequest, input,AbpSession.UserId);
                return;
            }
            
            
            input.IsBid = shippingRequest.IsBid;
            input.IsTachyonDeal = shippingRequest.IsTachyonDeal;


            foreach (var vas in shippingRequest.ShippingRequestVases)
            {
                if (!input.ShippingRequestVasList.Any(x => x.Id == vas.Id))
                {
                    await _shippingRequestVasRepository.DeleteAsync(vas);
                }
            }

            await ValidateGoodsCategory(input);
            
            if (shippingRequest.Status == ShippingRequestStatus.NeedsAction) 
                await CheckHasOffersToNotifyCarriers(shippingRequest);

            ObjectMapper.Map(input, shippingRequest);

            await CurrentUnitOfWork.SaveChangesAsync();
        }


        [AbpAuthorize(AppPermissions.Pages_VasPrices)]
        public async Task<List<ShippingRequestVasListOutput>> GetAllShippingRequestVasesForTableDropdown()
        {
            return await _lookup_vasRepository.GetAll()
                .Select(vas => new ShippingRequestVasListOutput
                {
                    VasName = vas.Translations.FirstOrDefault(t => t.Language.Contains(CurrentLanguage)) != null
                        ? vas.Translations.FirstOrDefault(t => t.Language.Contains(CurrentLanguage)).DisplayName
                        : vas.Key,
                    HasAmount = vas.HasAmount,
                    HasCount = vas.HasCount,
                    MaxAmount = 0,
                    MaxCount = 0,
                    Id = vas.Id,
                    IsOther = vas.ContainsOther()
                }).ToListAsync();
        }


        public async Task<List<ShippingRequestVasPriceDto>> GetAllShippingRequestVasForPricing(long shippingRequestId)
        {
            var shippingRequestVases = _shippingRequestVasRepository.GetAll().Include(x => x.VasFk)
                .Where(z => z.ShippingRequestId == shippingRequestId);
            var result = from o in shippingRequestVases
                join o1 in _vasPriceRepository.GetAll() on o.VasId equals o1.VasId into j1
                from s1 in j1.DefaultIfEmpty()
                select new ShippingRequestVasPriceDto()
                {
                    ShippingRequestVas = new ShippingRequestVasListOutput
                    {
                        VasName = o.VasFk.Key == null || o.VasFk.Key == null ? "" : o.VasFk.Key,
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

        public async Task<ShippingRequestPricingOutputforView> GetAllShippingRequestPricingForView(
            long shippingRequestId)
        {
            var pricedShippingRequest = new ShippingRequestPricingOutputforView();
            var carrierId = AbpSession.TenantId;

            pricedShippingRequest.PricedVasesList = await _shippingRequestVasRepository.GetAll().Include(x => x.VasFk)
                .Include(s => s.ShippingRequestFk).Where(z => z.ShippingRequestId == shippingRequestId)
                .Select(x => new ShippingRequestVasPriceDto
                    {
                        ActualPrice = x.ActualPrice,
                        ShippingRequestVasId = x.Id,
                        ShippingRequestVas = new ShippingRequestVasListOutput
                        {
                            VasName = x.VasFk.Key, MaxAmount = x.RequestMaxAmount, MaxCount = x.RequestMaxCount,
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
                .AsNoTracking().ToListAsync();

            List<TransportTypeSelectItemDto> transportTypeDtos =
                ObjectMapper.Map<List<TransportTypeSelectItemDto>>(transportTypes);

            return transportTypeDtos;
        }

        public async Task<List<TrucksTypeSelectItemDto>> GetAllTruckTypesByTransportTypeIdForDropdown(
            int transportTypeId)
        {
            List<TrucksType> list = await _lookup_trucksTypeRepository.GetAll()
                .Include(x => x.Translations)
                .Where(x => x.TransportTypeId == transportTypeId || x.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()))
                .Where(x => x.IsActive)
                .ToListAsync();
            return ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(list);
        }

        public async Task<List<SelectItemDto>> GetAllTuckCapacitiesByTuckTypeIdForDropdown(int truckTypeId)
        {
            return await _capacityRepository.GetAll()
                .Where(x => x.TrucksTypeId == truckTypeId || x.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()))
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }

        public async Task<IEnumerable<ISelectItemDto>> GetAllCapacitiesForDropdown()
        {
            List<Capacity> capacity = await _capacityRepository
                .GetAllIncluding(x => x.Translations)
                .ToListAsync();

            List<Capacity> filteredCapacity = new List<Capacity>();
            foreach (var c in capacity)
            {
                if (filteredCapacity.Find
                    (
                        x => x.DisplayName.ToLower().TrimEnd().TrimStart() ==
                             c.DisplayName.ToLower().TrimEnd().TrimStart()
                    ) == null)
                    filteredCapacity.Add(c);
            }

            return ObjectMapper.Map<List<CapacitySelectItemDto>>(filteredCapacity);
        }

        #endregion

        #region Waybills

        //Master Waybill
        public IEnumerable<GetMasterWaybillOutput> GetMasterWaybill(int shippingRequestTripId)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant,
                       nameof(IHasIsDrafted)))
            {
                var info = _shippingRequestTripRepository.GetAll()
                    .Include(e => e.ShippingRequestFk)
                    .Where(e => e.Id == shippingRequestTripId);


                string pickupFacility = GetFacilityPoint(shippingRequestTripId, null, PickingType.Pickup);

                var SenderpickupPoint = GetSenderInfo(shippingRequestTripId);
                var contactName = SenderpickupPoint != null ? SenderpickupPoint.FullName : "";
                var mobileNo = SenderpickupPoint != null ? SenderpickupPoint.PhoneNumber : "";

                var query = info.Select(x => new
                {
                    MasterWaybillNo = x.WaybillNumber.Value,
                    ShippingRequestStatus = x.Status == Trips.ShippingRequestTripStatus.Delivered ? "Final" : "Draft",
                    //(x.AssignedDriverUserId != null && x.AssignedTruckId != null) ? "Final" : "Draft",
                    SenderCompanyName = pickupFacility, //.ShippingRequestFk.Tenant.companyName,
                    DriverName = x.AssignedDriverUserFk != null ? x.AssignedDriverUserFk.FullName : "",
                    driverUserId = x.AssignedDriverUserId,
                    TruckTypeTranslationList = x.AssignedTruckFk.TrucksTypeFk.Translations,
                    TruckTypeDisplayName = x.AssignedTruckFk == null
                        ? ""
                        : (
                            (x.AssignedTruckFk.TransportTypeFk == null
                                ? ""
                                : ObjectMapper.Map<TransportTypeDto>(x.AssignedTruckFk.TransportTypeFk)
                                    .TranslatedDisplayName) + "-" + //o.TransportTypeFk.DisplayName) + " - " +
                            (x.AssignedTruckFk.TrucksTypeFk == null
                                ? ""
                                : ObjectMapper.Map<TrucksTypeDto>(x.AssignedTruckFk.TrucksTypeFk)
                                    .TranslatedDisplayName) + " - " +
                            (x.AssignedTruckFk.CapacityFk == null
                                ? ""
                                : ObjectMapper.Map<CapacityDto>(x.AssignedTruckFk.CapacityFk).DisplayName)
                        ),
                    PlateNumber = x.AssignedTruckFk != null ? x.AssignedTruckFk.PlateNumber : "",
                    IsMultipDrops = x.ShippingRequestFk.NumberOfDrops > 1 ? true : false,
                    TotalDrops = x.ShippingRequestFk.NumberOfDrops,
                    StartTripDate = x.ActualPickupDate,
                    CarrierName =
                        x.ShippingRequestFk.CarrierTenantFk != null
                            ? x.ShippingRequestFk.CarrierTenantFk.TenancyName
                            : "",
                    PackingTypeDisplayName = x.ShippingRequestFk.PackingTypeFk.DisplayName,
                    NumberOfPacking = x.ShippingRequestFk.NumberOfPacking,
                    TotalWeight = x.ShippingRequestFk.TotalWeight,
                    ShipperReference = x.ShippingRequestFk.ShipperReference,
                    ShipperInvoiceNo = x.ShippingRequestFk.ShipperInvoiceNo,
                    ClientName = x.ShippingRequestFk.Tenant.TenancyName,
                    ShipperNotes = x.Note
                });

                var pickup = GetPickupOrDropPointFacilityForTrip(shippingRequestTripId, PickingType.Pickup);


                var finalOutput = query.ToList().Select(x
                    => new GetMasterWaybillOutput()
                    {
                        MasterWaybillNo = x.MasterWaybillNo,
                        Date = NormalizeDateTimeToClientTime(Clock.Now),
                        ShippingRequestStatus = x.ShippingRequestStatus,
                        CompanyName = x.SenderCompanyName,
                        ContactName = contactName,
                        Mobile = mobileNo,
                        DriverName = x.DriverName,
                        DriverIqamaNo = GetDriverIqamaNo(x.driverUserId),
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
                        StartTripDate = NormalizeDateTimeToClientTime(x.StartTripDate),
                        CarrierName = x.CarrierName,
                        TotalWeight = x.TotalWeight,
                        ShipperReference = x.ShipperReference,
                        ShipperInvoiceNo = x.ShipperInvoiceNo,
                        InvoiceNumber = GetInvoiceNumberByTripId(shippingRequestTripId).ToString(),//GetInvoiceNumberByTripId(shippingRequestTripId),
                        ClientName = x.ClientName,
                        ShipperNotes = x.ShipperNotes
                    });

                return finalOutput;
            }
        }

        //Single Drop Waybill
        public IEnumerable<GetDropWaybillOutput> GetDropWaybill(int shippingRequestTripId, long? dropOffId = null)
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                var info = _shippingRequestTripRepository.GetAll()
                    .Where(e => e.Id == shippingRequestTripId);

                var query = info.Select(x => new
                {
                    Id = x.Id,
                    MasterWaybillNo = x.WaybillNumber.Value,
                    ShippingRequestStatus = x.Status == Trips.ShippingRequestTripStatus.Delivered ? "Final" : "Draft",
                    ClientName = x.ShippingRequestFk.Tenant.TenancyName,
                    CarrierName = x.ShippingRequestFk.CarrierTenantFk.TenancyName,
                    DriverName = x.AssignedDriverUserFk != null ? x.AssignedDriverUserFk.FullName : "",
                    driverUserId = x.AssignedDriverUserId,
                    TruckTypeTranslationList = x.AssignedTruckFk.TrucksTypeFk.Translations,
                    TruckTypeDisplayName = x.AssignedTruckFk == null
                        ? ""
                        : (
                            (x.AssignedTruckFk.TransportTypeFk == null
                                ? ""
                                : ObjectMapper.Map<TransportTypeDto>(x.AssignedTruckFk.TransportTypeFk)
                                    .TranslatedDisplayName) + "-" + //o.TransportTypeFk.DisplayName) + " - " +
                            (x.AssignedTruckFk.TrucksTypeFk == null
                                ? ""
                                : ObjectMapper.Map<TrucksTypeDto>(x.AssignedTruckFk.TrucksTypeFk)
                                    .TranslatedDisplayName) + " - " +
                            (x.AssignedTruckFk.CapacityFk == null
                                ? ""
                                : ObjectMapper.Map<CapacityDto>(x.AssignedTruckFk.CapacityFk).DisplayName)
                        ),
                    PlateNumber = x.AssignedTruckFk != null ? x.AssignedTruckFk.PlateNumber : "",
                    PackingTypeDisplayName = x.ShippingRequestFk.PackingTypeFk.DisplayName,
                    NumberOfPacking = x.ShippingRequestFk.NumberOfPacking,
                    StartTripDate = x.StartTripDate,
                    ActualPickupDate = x.ActualPickupDate,
                    DeliveryDate = x.ActualDeliveryDate,
                    TotalWeight = x.ShippingRequestFk.TotalWeight,
                    GoodCategoryTranslation = x.ShippingRequestFk.GoodCategoryFk.Translations,
                    GoodsCategoryDisplayName =
                        x.ShippingRequestFk.GoodCategoryFk, //x.ShippingRequestFk.GoodCategoryFk.DisplayName,
                    HasAttachment = x.HasAttachment,
                    NeedDeliveryNote = x.NeedsDeliveryNote,
                    ShipperReference = x.ShippingRequestFk.ShipperReference,
                    ShipperInvoiceNo = x.ShippingRequestFk.ShipperInvoiceNo,
                    ShipperNotes = x.Note
                });

                var pickup = GetPickupOrDropPointFacilityForTrip(shippingRequestTripId, PickingType.Pickup);

                var delivery = GetPickupOrDropPointFacilityForTrip(shippingRequestTripId, PickingType.Dropoff, dropOffId);

                var routPointWaybillNumber = _routPointRepository.GetAll()
                    .Where(x => x.Id == dropOffId && x.PickingType == PickingType.Dropoff)
                    .Select(x => x.WaybillNumber).FirstOrDefault();

                var SenderpickupPoint = GetSenderInfo(shippingRequestTripId);
                var contactName = SenderpickupPoint != null ? SenderpickupPoint.FullName : "";
                var mobileNo = SenderpickupPoint != null ? SenderpickupPoint.PhoneNumber : "";

                var finalOutput = query.ToList().Select(x
                    => new GetDropWaybillOutput
                    {
                        MasterWaybillNo = x.MasterWaybillNo,
                        WaybillNumber = routPointWaybillNumber,
                        Date = NormalizeDateTimeToClientTime(Clock.Now),
                        ShippingRequestStatus = x.ShippingRequestStatus,
                        SenderCompanyName = GetFacilityPoint(x.Id, null, PickingType.Pickup), // x.SenderCompanyName,
                        SenderContactName = contactName,
                        SenderMobile = mobileNo,
                        ReceiverCompanyName = GetFacilityPoint(x.Id, null, PickingType.Dropoff),
                        ReceiverContactName = GetReceiverName(null, x.Id),
                        ReceiverMobile = GetReceiverPhone(null, x.Id),
                        DriverName = x.DriverName,
                        DriverIqamaNo = GetDriverIqamaNo(x.driverUserId),
                        TruckTypeDisplayName = x.TruckTypeDisplayName,
                        PlateNumber = x.PlateNumber,
                        PackingTypeDisplayName = x.PackingTypeDisplayName,
                        NumberOfPacking = x.NumberOfPacking,
                        FacilityName = pickup != null ? pickup.Name : "",
                        CountryName = pickup?.CityFk.CountyFk.DisplayName,
                        CityName = pickup?.CityFk.DisplayName,
                        Area = pickup?.Address,
                        StartTripDate = NormalizeDateTimeToClientTime(x.StartTripDate),
                        ActualPickupDate = NormalizeDateTimeToClientTime(x.ActualPickupDate),
                        DroppFacilityName = delivery?.Name,
                        DroppCountryName = delivery?.CityFk.CountyFk.DisplayName,
                        DroppCityName = delivery?.CityFk.DisplayName,
                        DroppArea = delivery?.Address,
                        DeliveryDate = NormalizeDateTimeToClientTime(x.DeliveryDate),
                        TotalWeight = x.TotalWeight,
                        ClientName = x.ClientName,
                        CarrierName = x.CarrierName,
                        GoodsCategoryDisplayName =
                            ObjectMapper.Map<GoodCategoryDto>(x.GoodsCategoryDisplayName).DisplayName,
                        HasAttachment = x.HasAttachment,
                        NeedsDeliveryNote = x.NeedDeliveryNote,
                        ShipperReference = x.ShipperReference, /*TAC-2181 || 22/12/2021 || need to display it as an empty on production*/
                        ShipperInvoiceNo = x.ShipperInvoiceNo, /*TAC-2181 || 22/12/2021 || need to display it as an empty on production*/
                        InvoiceNumber = GetInvoiceNumberByTripId(shippingRequestTripId).ToString(),
                        IsSingleDrop = !dropOffId.HasValue,
                        ShipperNotes = x.ShipperNotes
                    });

                return finalOutput;
            }
        }

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
                    //.Where(e => e.ShippingRequestFk.TenantId == AbpSession.TenantId)
                    .Where(e => e.Id == routPoint.ShippingRequestTripId);

                var query = info.Select(x => new
                {
                    Id = x.Id,
                    MasterWaybillNo = x.WaybillNumber.Value,
                    SubWaybillNo = routPoint.WaybillNumber,
                    ShippingRequestStatus = x.Status == Trips.ShippingRequestTripStatus.Delivered ? "Final" : "Draft",
                    //(x.AssignedDriverUserId != null && x.AssignedTruckId != null) ? "Final" : "Draft",
                    ClientName = x.ShippingRequestFk.Tenant.TenancyName,
                    DriverName = x.AssignedDriverUserFk != null ? x.AssignedDriverUserFk.Name : "",
                    driverUserId = x.AssignedDriverUserId,
                    TruckTypeTranslationList = x.AssignedTruckFk.TrucksTypeFk.Translations,
                    TruckTypeDisplayName = x.AssignedTruckFk == null
                        ? ""
                        : (
                            (x.AssignedTruckFk.TransportTypeFk == null
                                ? ""
                                : ObjectMapper.Map<TransportTypeDto>(x.AssignedTruckFk.TransportTypeFk)
                                    .TranslatedDisplayName) + "-" + //o.TransportTypeFk.DisplayName) + " - " +
                            (x.AssignedTruckFk.TrucksTypeFk == null
                                ? ""
                                : ObjectMapper.Map<TrucksTypeDto>(x.AssignedTruckFk.TrucksTypeFk)
                                    .TranslatedDisplayName) + " - " +
                            (x.AssignedTruckFk.CapacityFk == null
                                ? ""
                                : ObjectMapper.Map<CapacityDto>(x.AssignedTruckFk.CapacityFk).DisplayName)
                        ),
                    PlateNumber = x.AssignedTruckFk != null ? x.AssignedTruckFk.PlateNumber : "",
                    PackingTypeDisplayName = x.ShippingRequestFk.PackingTypeFk.DisplayName,
                    NumberOfPacking = x.ShippingRequestFk.NumberOfPacking,
                    StartTripDate = x.StartTripDate,
                    //(x.StartTripDate != null && x.StartTripDate.Year > 1)
                    //    ? x.StartTripDate.ToShortDateString()
                    //    : "",
                    x.ActualPickupDate,
                    DroppFacilityName = routPoint.FacilityFk.Name,
                    DroppCountryName = routPoint.FacilityFk.CityFk.CountyFk.DisplayName,
                    DroppCityName = routPoint.FacilityFk.CityFk.DisplayName,
                    DroppArea = routPoint.FacilityFk.Address,
                    CarrierName =
                        x.ShippingRequestFk.CarrierTenantFk != null
                            ? x.ShippingRequestFk.CarrierTenantFk.TenancyName
                            : "",
                    TotalWeight = x.ShippingRequestFk.TotalWeight,
                    GoodsCategoryTranslation = x.ShippingRequestFk.GoodCategoryFk.Translations,
                    GoodsCategoryDisplayName = x.ShippingRequestFk.GoodCategoryFk,
                    DeliveryDate = x.ActualDeliveryDate,
                    HasAttachment = x.HasAttachment,
                    NeedsDeliveryNote = x.NeedsDeliveryNote,
                    ShipperReference = x.ShippingRequestFk.ShipperReference,
                    ShipperInvoiceNo = x.ShippingRequestFk.ShipperInvoiceNo,
                    ShipperNotes = x.Note
                });
                ;

                var SenderpickupPoint = GetSenderInfo(routPoint.ShippingRequestTripId);
                var contactName = SenderpickupPoint != null ? SenderpickupPoint.FullName : "";
                var mobileNo = SenderpickupPoint != null ? SenderpickupPoint.PhoneNumber : "";

                var finalOutput = query.ToList().Select(x
                    => new GetMultipleDropWaybillOutput
                    {
                        MasterWaybillNo = x.MasterWaybillNo,
                        SubWaybillNo = x.SubWaybillNo != null ? x.SubWaybillNo.Value : 0,
                        Date = NormalizeDateTimeToClientTime(Clock.Now),
                        ShippingRequestStatus = x.ShippingRequestStatus,
                        SenderCompanyName = GetFacilityPoint(x.Id, null, PickingType.Pickup),
                        SenderContactName = contactName,
                        SenderMobile = mobileNo,
                        ReceiverCompanyName =
                            GetFacilityPoint(null, routPointId, PickingType.Dropoff), //x.ReceiverCompanyName,
                        ReceiverContactName = GetReceiverName(routPointId, null),
                        ReceiverMobile = GetReceiverPhone(routPointId, null),
                        DriverName = x.DriverName,
                        DriverIqamaNo = GetDriverIqamaNo(x.driverUserId),
                        TruckTypeDisplayName = x.TruckTypeDisplayName,
                        PlateNumber = x.PlateNumber,
                        PackingTypeDisplayName = x.PackingTypeDisplayName,
                        NumberOfPacking = x.NumberOfPacking,
                        StartTripDate = NormalizeDateTimeToClientTime(x.StartTripDate),
                        DroppFacilityName = x.DroppFacilityName,
                        DroppCountryName = x.DroppCountryName,
                        DroppCityName = x.DroppCityName,
                        DroppArea = x.DroppArea,
                        CarrierName = x.CarrierName,
                        ClientName = x.ClientName,
                        TotalWeight = x.TotalWeight,
                        GoodsCategoryDisplayName =
                            ObjectMapper.Map<GoodCategoryDto>(x.GoodsCategoryDisplayName)
                                .DisplayName, // x.GoodsCategoryDisplayName,
                        DeliveryDate = NormalizeDateTimeToClientTime(x.DeliveryDate),
                        HasAttachment = x.HasAttachment,
                        NeedsDeliveryNote = x.NeedsDeliveryNote,
                        ShipperReference = x.ShipperReference,/*TAC-2181 || 22/12/2021 || need to display it as an empty on production*/
                        ShipperInvoiceNo = x.ShipperInvoiceNo,
                        InvoiceNumber = GetInvoiceNumberByTripId(x.Id).ToString()
                    });

                return finalOutput;
            }
        }

        public IEnumerable<GetAllShippingRequestVasesOutput> GetShippingRequestVasesForSingleDropWaybill(
            int shippingRequestTripId)
        {
            var vases = _shippingRequestTripVasRepository.GetAll()
                .Include(x => x.ShippingRequestVasFk)
                .Include(x => x.ShippingRequestVasFk.VasFk)
                .Where(x => x.ShippingRequestTripId == shippingRequestTripId)
                .ToList();

            var output = vases.Select
            (
                x => new GetAllShippingRequestVasesOutput
                {
                    VasName = x.ShippingRequestVasFk.VasFk.Key,
                    Amount = x.ShippingRequestVasFk.RequestMaxAmount,
                    Count = x.ShippingRequestVasFk.RequestMaxCount
                }
            );

            return output;
        }

        public IEnumerable<GetAllShippingRequestVasesOutput> GetShippingRequestVasesForMultipleDropWaybill(
            long RoutPointId)
        {
            //get shipping request id by step id
            var shippingRequestTripId =
                _routPointRepository.FirstOrDefault(x => x.Id == RoutPointId).ShippingRequestTripId;

            //get vases by shipping request id
            var vases = _shippingRequestTripVasRepository.GetAll()
                .Include(x => x.ShippingRequestVasFk.VasFk)
                .Include(x => x.ShippingRequestVasFk)
                .Where(x => x.ShippingRequestTripId == shippingRequestTripId)
                .ToList();

            var output = vases.Select
            (
                x => new GetAllShippingRequestVasesOutput
                {
                    VasName = x.ShippingRequestVasFk.VasFk.Key,
                    Amount = x.ShippingRequestVasFk.RequestMaxAmount,
                    Count = x.ShippingRequestVasFk.RequestMaxCount
                }
            );

            return output;
        }

        private string GetFacilityPoint(int? shippingRequestTripId,
            long? PointId,
            PickingType pickingType)
        {
            var point = _routPointRepository.GetAll()
                .Include(x => x.FacilityFk)
                .WhereIf(shippingRequestTripId != null,
                    x => x.ShippingRequestTripId == shippingRequestTripId && x.PickingType == pickingType)
                .WhereIf(PointId != null, x => x.Id == PointId && x.PickingType == pickingType)
                .FirstOrDefault();
            if (point != null)
                return point.FacilityFk.Name;
            return "";
        }

        // convert date time from UTC to client date time
        private string NormalizeDateTimeToClientTime(DateTime? serverDateTime)
        {
            return serverDateTime.HasValue ? ClockProviders.Local.Normalize(serverDateTime.Value).ToString() : "";
        }

        private string GetReceiverName(long? PointId, int? tripId)
        {
            var point = _routPointRepository.GetAll()
                .Include(x => x.ReceiverFk)
                .WhereIf(PointId != null, x => x.Id == PointId && x.PickingType == PickingType.Dropoff)
                .WhereIf(tripId != null, x => x.ShippingRequestTripId == tripId && x.PickingType == PickingType.Dropoff)
                .FirstOrDefault();
            if (point != null)
            {
                return point.ReceiverId != null ? point.ReceiverFk.FullName : point.ReceiverFullName;
            }

            return "";
        }

        private string GetReceiverPhone(long? PointId, int? tripId)
        {
            var point = _routPointRepository.GetAll()
                .Include(x => x.ReceiverFk)
                .WhereIf(PointId != null, x => x.Id == PointId && x.PickingType == PickingType.Dropoff)
                .WhereIf(tripId != null, x => x.ShippingRequestTripId == tripId && x.PickingType == PickingType.Dropoff)
                .FirstOrDefault();
            if (point != null)
            {
                return point.ReceiverId != null ? point.ReceiverFk.PhoneNumber : point.ReceiverPhoneNumber;
            }

            return "";
        }

        private Receiver GetSenderInfo(int tripId)
        {
            return _routPointRepository.GetAll()
                .Include(x => x.ReceiverFk)
                .Where(x => x.ShippingRequestTripId == tripId && x.PickingType == PickingType.Pickup)
                .FirstOrDefault().ReceiverFk;
        }

        private long? GetInvoiceNumberByTripId(int tripId)
        {
            var invoiceNumber = _InvoiveTripRepository.GetAll()
                .Where(x => x.TripId == tripId)
                .WhereIf(AbpSession.TenantId != null, x => x.InvoiceFK.TenantId == AbpSession.TenantId)
                .Select(x => x.InvoiceFK.InvoiceNumber)
                .FirstOrDefault();
            return invoiceNumber;
        }

        private string GetDriverIqamaNo(long? UserId)
        {
            if (UserId == null)
                return "";
            return (AsyncHelper.RunSync(() => _documentFilesManager.GetDriverIqamaActiveDocumentAsync(UserId.Value)))
                ?.Number;
        }

        #endregion

        #region dropDowns


        public async Task<List<GetAllUnitOfMeasureForDropDownOutput>> GetAllUnitOfMeasuresForDropdown()
        {
            var unitOfMeasures = await _unitOfMeasureRepository.GetAll()
                .Include(x => x.Translations)
                .ToListAsync();
            return ObjectMapper.Map<List<GetAllUnitOfMeasureForDropDownOutput>>(unitOfMeasures);
        }

        public async Task<List<SelectItemDto>> GetAllShippingTypesForDropdown()
        {
            return (await _shippingTypeRepository.GetAll()
                    .ProjectTo<ShippingTypeDto>(AutoMapperConfigurationProvider)
                    .ToArrayAsync())
                .Select(x => new SelectItemDto() { Id = x.Id.ToString(), DisplayName = x.DisplayName }).ToList();
        }

        public async Task<List<SelectItemDto>> GetAllPackingTypesForDropdown()
        {
            return (await _packingTypeRepository.GetAll()
                    .ProjectTo<PackingTypeDto>(AutoMapperConfigurationProvider)
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName,
                    IsOther = x.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower())
                }).ToListAsync());

        }
        //end Multiple Drops

        #endregion


        private async Task ShippingRequestVasListValidate(IHasVasListDto input, int numberOfTrips)
        {
            if (input.ShippingRequestVasList.Count <= 0) return;

            var vasesItems = await _lookup_vasRepository.GetAllListAsync();

            foreach (var item in input.ShippingRequestVasList)
            {
                var vasItem = vasesItems.FirstOrDefault(x => x.Id == item.VasId);

                if (item.NumberOfTrips > numberOfTrips)
                {
                    throw new AbpValidationException
                    (
                        L("NumberOfTripsForVasCanNotBeGreaterThanShippingRequestNumberOfTrips")
                    );
                }

                if (vasItem == null) continue;

                if (vasItem.HasAmount && item.RequestMaxAmount < 1)
                {
                    throw new ValidationException(L("Vas Amount must have value"));
                }

                if (vasItem.HasCount && item.RequestMaxCount < 1)
                {
                    throw new ValidationException(L("Vas Count must have value"));
                }
            }
        }

        /// <summary>
        /// 
        /// <list type="bullet|number|table">
        /// <listheader>This Method Used For Validate</listheader>
        ///    <item>1-OtherGoodsCategoryName</item>
        ///    <item>2-OtherTransportTypeName</item>
        ///    <item>3-OtherTrucksTypeName</item>
        /// </list>
        /// See <see cref="CreateOrEditShippingRequestDto"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private async Task OthersNameValidation(IShippingRequestDtoHaveOthersName input)
        {
            #region Validate GoodCategory

            if (input.GoodCategoryId != null)
            {
                var goodCategory = await _lookup_goodCategoryRepository
                    .FirstOrDefaultAsync(input.GoodCategoryId.Value);

                if (goodCategory.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                    input.OtherGoodsCategoryName.Trim().IsNullOrEmpty())
                    throw new UserFriendlyException(L("GoodCategoryCanNotBeOtherAndEmptyAtSameTime"));
            }

            #endregion

            #region Validate TransportType

            if (input.TransportTypeId != null)
            {
                var transportType = await _transportTypeRepository
                    .FirstOrDefaultAsync(input.TransportTypeId.Value);

                if (transportType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                    input.OtherTransportTypeName.Trim().IsNullOrEmpty())
                    throw new UserFriendlyException(L("TransportTypeCanNotBeOtherAndEmptyAtSameTime"));
            }

            #endregion

            #region Validate TrucksType

            //? FYI TrucksTypeId Not Nullable 
            var trucksType = await _lookup_trucksTypeRepository
                .FirstOrDefaultAsync(input.TrucksTypeId);

            if (trucksType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                input.OtherTrucksTypeName.Trim().IsNullOrEmpty())
                throw new UserFriendlyException(L("TrucksTypeCanNotBeOtherAndEmptyAtSameTime"));

            #endregion

            #region Validate PackingType

            var packingType = await _packingTypeRepository
                .FirstOrDefaultAsync(input.PackingTypeId);

            if (packingType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName.ToLower()) &&
                input.OtherPackingTypeName.Trim().IsNullOrEmpty())
                throw new UserFriendlyException(L("PackingTypeCanNotBeOtherAndEmptyAtSameTime"));

            #endregion

        }

        private Facility GetPickupOrDropPointFacilityForTrip(int id, PickingType type, long? dropOffId = null)
        {
            var pickupFacility = _routPointRepository
                .GetAll()
                .Where(x => x.ShippingRequestTripId == id && x.PickingType == type)
                .WhereIf(dropOffId.HasValue, x => x.Id == dropOffId.Value)
                .Include(x => x.FacilityFk)
                .ThenInclude(x => x.CityFk)
                .ThenInclude(x => x.CountyFk)
                .Select(x => x.FacilityFk)
                .FirstOrDefault();
            return pickupFacility;
        }
    }
}