using Abp;
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
using TACHYON.Cities;
using TACHYON.Cities.Dtos;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Notifications;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Vases;

namespace TACHYON.PriceOffers
{
    [AbpAuthorize()]
    public class PriceOfferAppService : TACHYONAppServiceBase, IPriceOfferAppService
    {
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly PriceOfferManager _priceOfferManager;
        private IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;
        private readonly IAppNotifier _appNotifier;


        private IRepository<VasPrice> _vasPriceRepository;
        public PriceOfferAppService(IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository,
            IRepository<ShippingRequest, long> shippingRequestsRepository, PriceOfferManager priceOfferManager,
            IRepository<PriceOffer, long> priceOfferRepository, IRepository<VasPrice> vasPriceRepository,
            IRepository<City> cityRepository, IRepository<TrucksType, long> trucksTypeRepository, IAppNotifier appNotifier)
        {
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
            _priceOfferManager = priceOfferManager;
            _priceOfferRepository = priceOfferRepository;
            _vasPriceRepository = vasPriceRepository;
            _cityRepository = cityRepository;
            _trucksTypeRepository = trucksTypeRepository;
            _appNotifier = appNotifier;
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
                .WhereIf(input.Channel.HasValue,x=>x.Channel== input.Channel.Value /*|| x.Channel== PriceOfferChannel.TachyonManageService*/)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId && (!x.ShippingRequestFK.IsTachyonDeal || x.Channel == PriceOfferChannel.TachyonManageService))
                //.WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId)
                .OrderBy(input.Sorting ?? "id desc")
               ;
            var offers = query.PageBy(input);

            List<PriceOfferListDto> PriceOfferList = new List<PriceOfferListDto>();
            foreach (var offer in offers)
            {
               var price= ObjectMapper.Map<PriceOfferListDto>(offer);
                if (AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper)) {
                    price.TotalAmount = offer.TotalAmountWithCommission;
                    if (offer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper) 
                        price.StatusTitle = PriceOfferStatus.New.GetEnumDescription();
                    else if(offer.Status == PriceOfferStatus.AcceptedAndWaitingForCarrier)
                        price.StatusTitle = PriceOfferStatus.Accepted.GetEnumDescription();
                }
                PriceOfferList.Add(price);
            }

            return new PagedResultDto<PriceOfferListDto>(
                await query.CountAsync(),
                PriceOfferList
            );
        }
        public GetShippingRequestSearchListDto GetAllListForSearch()
        {
            var searchList = new GetShippingRequestSearchListDto() 
            { 
                Cities=ObjectMapper.Map<List<CityDto>>(_cityRepository.GetAllIncluding(x => x.Translations)),
                TrucksTypes = ObjectMapper.Map<List<TrucksTypeDto>>(_trucksTypeRepository.GetAllIncluding(x => x.Translations))
            };

            return searchList;
        }

        /// <summary>
        /// Get the price offer when the user need to create offer or edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier)]
        public async Task<PriceOfferDto> GetPriceOfferForCreateOrEdit(long id,long? OfferId)
        {
            DisableTenancyFilters();
            var shippingrequest = await _shippingRequestsRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                  .ThenInclude(v => v.VasFk)
                .FirstOrDefaultAsync(x => x.Id == id && (x.Status == ShippingRequestStatus.PrePrice || x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier));

            if (shippingrequest == null) throw new UserFriendlyException(L("TheRecordIsNotFound"));

            var offer = await _priceOfferRepository
                .GetAll()
                .Include(i => i.PriceOfferDetails)
                .Where(x => x.ShippingRequestId == shippingrequest.Id )
                .WhereIf(OfferId.HasValue,x=>x.Id== OfferId.Value)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId.Value && (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.Rejected))
                .WhereIf(await IsEnabledAsync(AppFeatures.TachyonDealer),
                x=> ((x.TenantId == AbpSession.TenantId.Value || x.ShippingRequestFK.IsTachyonDeal ) && (x.Status == PriceOfferStatus.New || (x.Status == PriceOfferStatus.Rejected && x.Tenant.EditionId== AppConsts.TachyonEditionId) || x.Status == PriceOfferStatus.Pending)))              
                .OrderBy(x=>x.Status)
                //.OrderByDescending(x=>x.Id)
                .FirstOrDefaultAsync();
            PriceOfferDto priceOfferDto;
            if (offer != null)
            {
                priceOfferDto = ObjectMapper.Map<PriceOfferDto>(offer);
                foreach (var item in priceOfferDto.Items)
                {
                    item.ItemName = shippingrequest.ShippingRequestVases.FirstOrDefault(x => x.Id == item.SourceId).VasFk.Name;
                }
                if (IsEnabled(AppFeatures.TachyonDealer))
                {
                    if (priceOfferDto.Items !=null && priceOfferDto.Items.Count>0)
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
                    Quantity = shippingrequest.NumberOfTrips,
                    Items = GetVases(shippingrequest),
                    TaxVat = (decimal)Convert.ChangeType(SettingManager.GetSettingValue(AppSettings.HostManagement.TaxVat), typeof(decimal))
                };
                SetCommssionSettingsForTachyonDealer(priceOfferDto, shippingrequest);

            }
            if (IsEnabled(AppFeatures.TachyonDealer))
            {
                priceOfferDto.CommssionSettings = SetTenantCommssionSettingsForTachyonDealer(shippingrequest.TenantId);
            }
            return priceOfferDto;
        }

        /// <summary>
        /// Get offer details for preview
        /// </summary>
        /// <param name="OfferId"></param>
        /// <returns></returns>
        public async Task<PriceOfferViewDto> GetPriceOfferForView(long OfferId)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier, AppFeatures.Shipper);
            DisableTenancyFilters();
            var offer = await _priceOfferRepository
                .GetAll()
                .Include(i => i.Tenant)
                .Include(i => i.PriceOfferDetails)
                .Include(i => i.ShippingRequestFK)
                 .ThenInclude(v => v.ShippingRequestVases)
                  .ThenInclude(v => v.VasFk)
                  .Where(x => x.Id == OfferId)
                //.WhereIf(!AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal || (!x.ShippingRequestFK.IsTachyonDeal && x.TenantId== AbpSession.TenantId.Value))
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId.Value && (!x.ShippingRequestFK.IsTachyonDeal || x.Channel== PriceOfferChannel.TachyonManageService))
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId.Value)
                .FirstOrDefaultAsync();


            if (offer == null) throw new UserFriendlyException(L("ThereNoOffer"));
            var priceOfferDto = ObjectMapper.Map<PriceOfferViewDto>(offer);
            foreach (var item in priceOfferDto.Items)
            {
                item.ItemName = offer.ShippingRequestFK.ShippingRequestVases.FirstOrDefault(x => x.Id == item.SourceId).VasFk.Name;
                if (AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper))
                {
                    item.ItemPrice = item.ItemSubTotalAmountWithCommission;
                    item.ItemTotalAmount = item.ItemTotalAmountWithCommission;
                }

            }
            if (AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper))
            {
                priceOfferDto.ItemPrice = offer.ItemSubTotalAmountWithCommission;
                priceOfferDto.ItemTotalAmount = offer.ItemTotalAmountWithCommission;
                priceOfferDto.TotalAmount = offer.TotalAmountWithCommission;
                priceOfferDto.SubTotalAmount = offer.SubTotalAmountWithCommission;
                priceOfferDto.VatAmount = offer.VatAmountWithCommission;
                offer.IsView = true;
            }

            return priceOfferDto;
        }


        [RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer)]
       // [AbpAuthorize(AppPermissions.Pages_Offers_Create)]

        public async Task<long> CreateOrEdit(CreateOrEditPriceOfferInput Input)
        {
            if (IsEnabled(AppFeatures.Carrier))
            {
                Input.CommissionPercentageOrAddValue = default;
                Input.CommissionType = default;
                Input.VasCommissionPercentageOrAddValue = default;
                Input.VasCommissionType = default;
                Input.ParentId = default;
            }
            //Input.Channel = PriceOfferChannel.MarketPlace;
            return await _priceOfferManager.CreateOrEdit(Input);

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
            return new ListResultDto<GetShippingRequestForPriceOfferListDto>(query);
        }


        public async Task Delete(EntityDto Input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Carrier);
            await _priceOfferManager.Delete(Input);
        }


        public async Task<PriceOfferStatus> Accept(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer,  AppFeatures.Shipper);
            return await  _priceOfferManager.AcceptOffer(id);
        }

   
        public async Task Reject(RejectPriceOfferInput input)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            if (AbpSession.TenantId.HasValue) input.RejectBy =  GetCurrentTenant().Name;
            await _priceOfferManager.RejectOffer(input);
        }


        public async Task Cancel(long id)
        {
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer);

            DisableTenancyFilters();

            var offer = await _priceOfferRepository.FirstOrDefaultAsync(x => x.Id == id && x.Status == PriceOfferStatus.Pending);
            if (offer !=null)
            {
                offer.Status = PriceOfferStatus.New;
            if (offer.Channel== PriceOfferChannel.DirectRequest)  await  _priceOfferManager.ChangeDirectRequestStatus(offer.SourceId.Value, ShippingRequestDirectRequestStatus.Response);
                var childOffer = await  _priceOfferManager.GetOfferParentId(offer.Id);
                if (childOffer != null) {
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
                            .Include(dc => dc.DestinationCityFk)
                            .Include(v => v.ShippingRequestVases)
                             .ThenInclude(v => v.VasFk)
                            .Include(c => c.GoodCategoryFk)
                             .ThenInclude(x => x.Translations)
                            .Include(t => t.TrucksTypeFk)
                             .ThenInclude(x => x.Translations)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                            .FirstOrDefaultAsync(r => r.Id == input.Id/* && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice || r.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier)*/);

            if (shippingRequest == null) throw new UserFriendlyException(L("TheRecordIsNotFound"));

            if (AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier))// Applay permission for carrier if can see the shipping request details
            {
                if (!_priceOfferManager.CheckCarrierIsPricing(input.Id))
                {
                    if (shippingRequest.IsBid && input.Channel == PriceOfferChannel.MarketPlace)
                    {
                        if (shippingRequest.BidStatus != ShippingRequestBidStatus.OnGoing) throw new UserFriendlyException(L("The Bid must be Ongoing"));
                    }
                    else
                    {
                        var _directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.CarrierTenantId == AbpSession.TenantId.Value && x.ShippingRequestId == input.Id && x.Status != ShippingRequestDirectRequestStatus.Declined);
                        if (_directRequest == null) throw new UserFriendlyException(L("YouDoNotHaveDirectRequest"));
                    }
                }


            }



            var getShippingRequestForPricingOutput = ObjectMapper.Map<GetShippingRequestForPricingOutput>(shippingRequest);
            getShippingRequestForPricingOutput.Items = ObjectMapper.Map<List<PriceOfferItemDto>>(shippingRequest.ShippingRequestVases);
            getShippingRequestForPricingOutput.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(shippingRequest.GoodCategoryFk).DisplayName;
            getShippingRequestForPricingOutput.TrukType = ObjectMapper.Map<TrucksTypeDto>(shippingRequest.TrucksTypeFk).TranslatedDisplayName;

            return getShippingRequestForPricingOutput;

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
                        Quantity = vas.RequestMaxCount<=0  ?   1: vas.RequestMaxCount,
                        ItemName = vas.VasFk.Name
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
        private PriceOfferTenantCommssionSettings SetTenantCommssionSettingsForTachyonDealer(int TenantId)
        {
            PriceOfferTenantCommssionSettings commssionSettings=
                new PriceOfferTenantCommssionSettings() 
                { 
                     ItemCommissionType= (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionType)),
                     ItemCommissionPercentage= Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionPercentage)),
                     ItemCommissionValue= Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionValue)),
                     ItemMinValueCommission= Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripMinValueCommission)),
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
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(dc => dc.DestinationCityFk)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(c => c.GoodCategoryFk)
                                    .ThenInclude(x => x.Translations)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(t => t.TrucksTypeFk)
                                    .ThenInclude(x => x.Translations)
                            .Where(r => /*r.Status != ShippingRequestDirectRequestStatus.Accepted &&*/ (r.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction || r.ShippingRequestFK.Status == ShippingRequestStatus.PrePrice || r.ShippingRequestFK.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier))
                            .WhereIf(input.ShippingRequestId.HasValue, x => x.ShippingRequestId == input.ShippingRequestId)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId && !x.ShippingRequestFK.IsTachyonDeal /*&& x.ShippingRequestFK.RequestType == ShippingRequestType.DirectRequest*/)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId && x.Status != ShippingRequestDirectRequestStatus.Declined && (x.ShippingRequestFK.RequestType == ShippingRequestType.DirectRequest || x.ShippingRequestFK.IsTachyonDeal))
                            .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.ShippingRequestFK.StartTripDate >= input.PickupFromDate.Value && x.ShippingRequestFK.StartTripDate <= input.PickupToDate.Value)
                            .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.CreationTime >= input.FromDate.Value && x.CreationTime <= input.ToDate.Value)
                            .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFK.OriginCityId==input.OriginId)
                            .WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestFK.DestinationCityId == input.DestinationId)
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
                dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.ShippingRequestFK.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.ShippingRequestFK.GoodCategoryFk).DisplayName;
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
            var query = _shippingRequestsRepository
                .GetAll()
                .AsNoTracking()
                    .Include(t => t.Tenant)
                    .Include(oc => oc.OriginCityFk)
                    .Include(dc => dc.DestinationCityFk)
                    .Include(c => c.GoodCategoryFk)
                     .ThenInclude(x => x.Translations)
                    .Include(t => t.TrucksTypeFk)
                     .ThenInclude(x => x.Translations)
                .Where(x => x.IsBid && (x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice || x.Status == ShippingRequestStatus.AcceptedAndWaitingCarrier))
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal || (x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice))
                .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.StartTripDate >= input.PickupFromDate.Value && x.StartTripDate <= input.PickupToDate.Value)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.BidStartDate >= input.FromDate.Value && x.BidStartDate <= input.ToDate.Value)
                .WhereIf(input.OriginId.HasValue, x => x.OriginCityId == input.OriginId)
                .WhereIf(input.DestinationId.HasValue, x => x.DestinationCityId == input.DestinationId)
                .WhereIf(input.RouteTypeId.HasValue, x => x.RouteTypeId == input.RouteTypeId)
                .WhereIf(input.TruckTypeId.HasValue, x => x.TrucksTypeId == input.TruckTypeId)
                .WhereIf(input.Status.HasValue, x => x.BidStatus ==(ShippingRequestBidStatus) input.Status)
                .WhereIf(input.IsTachyonDeal, x => x.IsTachyonDeal == input.IsTachyonDeal)
                .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.Tenant.Name.ToLower().Contains(input.Filter) || x.Tenant.companyName.ToLower().Contains(input.Filter) || x.Tenant.TenancyName.ToLower().Contains(input.Filter))
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);


            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

            foreach (var request in await query.ToListAsync())
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request);

                if (AbpSession.TenantId.HasValue && ( IsEnabled(AppFeatures.Carrier) || (IsEnabled(AppFeatures.TachyonDealer) && !request.IsTachyonDeal)))
                {
                    var offer = _priceOfferManager.GetCarrierPricingOrNull(request.Id);
                    if (offer != null)
                    {
                        dto.OfferId = offer.Id;
                        dto.isPriced = true;
                        if (offer.Status== PriceOfferStatus.Accepted || offer.Status == PriceOfferStatus.AcceptedAndWaitingForShipper)
                        {
                            dto.BidStatusTitle = "Confirmed";
                        }
                        else
                        {
                            dto.BidStatusTitle = "PriceSubmitted";

                        }
                    }
                    else
                    {
                        dto.BidStatusTitle = "New";
                    }
                    dto.StatusTitle = "";
                }
                dto.CreationTime = request.BidStartDate;
                dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.GoodCategoryFk).DisplayName;
                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;
        }


        private async Task<List<GetShippingRequestForPriceOfferListDto>> GetFromShippingRequest(ShippingRequestForPriceOfferGetAllInput input)
        {
            //using (CurrentUnitOfWork.DisableFilter(nameof(IHasIsDrafted)))
            //{
                var query = _shippingRequestsRepository
                .GetAll()
                .AsNoTracking()
                    .Include(t => t.Tenant)
                    .Include(c => c.CarrierTenantFk)
                    .Include(oc => oc.OriginCityFk)
                    .Include(dc => dc.DestinationCityFk)
                    .Include(c => c.GoodCategoryFk)
                     .ThenInclude(x => x.Translations)
                    .Include(t => t.TrucksTypeFk)
                     .ThenInclude(x => x.Translations)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => input.isTMSRequest || (!input.isTMSRequest && (x.Status == ShippingRequestStatus.Cancled || x.Status == ShippingRequestStatus.Completed)))
                .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.StartTripDate >= input.PickupFromDate.Value && x.StartTripDate <= input.PickupToDate.Value)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.CreationTime >= input.FromDate.Value && x.CreationTime <= input.ToDate.Value)
                .WhereIf(input.OriginId.HasValue, x => x.OriginCityId == input.OriginId)
                .WhereIf(input.DestinationId.HasValue, x => x.DestinationCityId == input.DestinationId)
                .WhereIf(input.RouteTypeId.HasValue, x => x.RouteTypeId == input.RouteTypeId)
                .WhereIf(input.TruckTypeId.HasValue, x => x.TrucksTypeId == input.TruckTypeId)
                .WhereIf(input.Status.HasValue, x => x.Status == (ShippingRequestStatus)input.Status)
                .WhereIf(input.RequestType.HasValue, x => x.RequestType == input.RequestType)
                .WhereIf(input.IsTachyonDeal, x => x.IsTachyonDeal == input.IsTachyonDeal)
                .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.Tenant.Name.ToLower().Contains(input.Filter) || x.Tenant.companyName.ToLower().Contains(input.Filter) || x.Tenant.TenancyName.ToLower().Contains(input.Filter))
                .WhereIf(!string.IsNullOrEmpty(input.Carrier), x => x.CarrierTenantFk.Name.ToLower().Contains(input.Carrier) || x.CarrierTenantFk.companyName.ToLower().Contains(input.Carrier) || x.CarrierTenantFk.TenancyName.ToLower().Contains(input.Carrier))
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);

                //var myDraftsOnly = query.Where(x => x.TenantId == AbpSession.TenantId)
                //             .Where(x => x.IsDrafted);

                //var withoutDrafts = query.Where(x => !x.IsDrafted);
                //concat all requests without draft with my draft requests
                //query = myDraftsOnly.Concat(withoutDrafts);

                List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

                foreach (var request in await query.ToListAsync())
                {
                    var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request);
                    dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.TrucksTypeFk)?.TranslatedDisplayName;
                    dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.GoodCategoryFk)?.DisplayName;

                    if (AbpSession.TenantId.HasValue && (IsEnabled(AppFeatures.Carrier)))
                    {

                        dto.Price = request.CarrierPrice;
                    }
                    else if (AbpSession.TenantId.HasValue && (IsEnabled(AppFeatures.Shipper)))
                    {
                        if (request.IsTachyonDeal)
                        {
                            dto.TotalOffers = _priceOfferManager.GetTotalOffersByTMS(request.Id);
                        }
                    }

                    ShippingRequestForPriceOfferList.Add(dto);

                }

                return ShippingRequestForPriceOfferList;
          //  }
        }

        private async Task<List<GetShippingRequestForPriceOfferListDto>> GetFromOffers(ShippingRequestForPriceOfferGetAllInput input)
        {
            var offers = _priceOfferRepository
                            .GetAll()
                            .AsNoTracking()
                            .Include(carrier => carrier.Tenant)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(shipper => shipper.Tenant)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(oc => oc.OriginCityFk)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(dc => dc.DestinationCityFk)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(c => c.GoodCategoryFk)
                                    .ThenInclude(x => x.Translations)
                            .Include(r => r.ShippingRequestFK)
                                .ThenInclude(t => t.TrucksTypeFk)
                                    .ThenInclude(x => x.Translations)
                             .Where(r =>(r.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction || r.ShippingRequestFK.Status == ShippingRequestStatus.PrePrice || r.ShippingRequestFK.Status == ShippingRequestStatus.PostPrice))
                            .WhereIf(input.ShippingRequestId.HasValue, x => x.ShippingRequestId == input.ShippingRequestId)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId && !x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.TenantId == AbpSession.TenantId)
                            .WhereIf(input.PickupFromDate.HasValue && input.PickupToDate.HasValue, x => x.ShippingRequestFK.StartTripDate >= input.FromDate.Value && x.ShippingRequestFK.StartTripDate <= input.ToDate.Value)
                            .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue, x => x.CreationTime >= input.FromDate.Value && x.CreationTime <= input.ToDate.Value)
                            .WhereIf(input.OriginId.HasValue, x => x.ShippingRequestFK.OriginCityId == input.OriginId)
                            .WhereIf(input.DestinationId.HasValue, x => x.ShippingRequestFK.DestinationCityId == input.DestinationId)
                            .WhereIf(input.RouteTypeId.HasValue, x => x.ShippingRequestFK.RouteTypeId == input.RouteTypeId)
                            .WhereIf(input.TruckTypeId.HasValue, x => x.ShippingRequestFK.TrucksTypeId == input.TruckTypeId)
                            .WhereIf(input.Status.HasValue, x => x.Status == (PriceOfferStatus)input.Status)
                            .WhereIf(input.IsTachyonDeal, x => x.ShippingRequestFK.IsTachyonDeal == input.IsTachyonDeal)
                            .WhereIf(!string.IsNullOrEmpty(input.Filter), x => x.ShippingRequestFK.Tenant.Name.ToLower().Contains(input.Filter) || x.ShippingRequestFK.Tenant.companyName.ToLower().Contains(input.Filter) || x.ShippingRequestFK.Tenant.TenancyName.ToLower().Contains(input.Filter))
                            .WhereIf(!string.IsNullOrEmpty(input.Carrier), x => x.Tenant.Name.ToLower().Contains(input.Carrier) || x.Tenant.companyName.ToLower().Contains(input.Carrier) || x.Tenant.TenancyName.ToLower().Contains(input.Carrier))

                            .OrderBy(input.Sorting ?? "id desc")
                            .PageBy(input);

            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

            foreach (var request in await offers.ToListAsync())
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request.ShippingRequestFK);
                dto.DirectRequestStatusTitle = request.Status.GetEnumDescription();
                dto.OfferStatus = request.Status;

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
                else if (AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier) )
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
                dto.BidStatusTitle = string.Empty;
                dto.TruckType = ObjectMapper.Map<TrucksTypeDto>(request.ShippingRequestFK.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.ShippingRequestFK.GoodCategoryFk).DisplayName;
                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;

        }
        public async Task CancelShipment(CancelShippingRequestInput input)
        {
            DisableTenancyFilters();
            CheckIfCanAccessService(true, AppFeatures.TachyonDealer, AppFeatures.Shipper);
            var request = await _shippingRequestsRepository
                .GetAll()
                .Where(x=>x.Status  == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper),x=> x.TenantId== AbpSession.TenantId.Value)
                .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.IsTachyonDeal)
                .FirstOrDefaultAsync(x => x.Id == input.Id);
            if (request == null) throw new UserFriendlyException(L("YouCanNotCancelThisShipment"));
                request.CancelReason = input.CancelReason;
                request.Status = ShippingRequestStatus.Cancled;
            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
              var user = await UserManager.GetAdminTachyonDealerAsync();
                await _appNotifier.CancelShipment(request.Id, request.CancelReason, L(AppConsts.TMS), new UserIdentifier(request.TenantId, request.CreatorUserId.Value));
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
