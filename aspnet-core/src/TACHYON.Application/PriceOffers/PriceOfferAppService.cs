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
using TACHYON.Authorization;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Vases;

namespace TACHYON.PriceOffers
{
    public class PriceOfferAppService : TACHYONAppServiceBase, IPriceOfferAppService
    {
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly PriceOfferManager _priceOfferManager;
        private IRepository<PriceOffer, long> _priceOfferRepository;

        private IRepository<VasPrice> _vasPriceRepository;
        public PriceOfferAppService(IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository, IRepository<ShippingRequest, long> shippingRequestsRepository, PriceOfferManager priceOfferManager, IRepository<PriceOffer, long> priceOfferRepository, IRepository<VasPrice> vasPriceRepository)
        {
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
            _priceOfferManager = priceOfferManager;
            _priceOfferRepository = priceOfferRepository;
            _vasPriceRepository = vasPriceRepository;
        }
        #region Services
        /// <summary>
        /// Get the price offer when the user need to create offer or edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier)]
        public async Task<PriceOfferDto> GetPriceOfferForCreateOrEdit(long id)
        {
            DisableTenancyFilters();
            var shippingrequest = await _shippingRequestsRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                  .ThenInclude(v => v.VasFk)
                .FirstOrDefaultAsync(x => x.Id == id && (x.Status == ShippingRequestStatus.PrePrice || x.Status == ShippingRequestStatus.NeedsAction));
            if (shippingrequest == null) throw new UserFriendlyException(L("TheRecordIsNotFound"));
            var offer = await _priceOfferRepository
                .GetAll()
                .Include(i => i.PriceOfferDetails)
                .FirstOrDefaultAsync(
                x => x.ShippingRequestId == shippingrequest.Id &&
                (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.Rejected) &&
                x.TenantId == AbpSession.TenantId.Value
                );
            PriceOfferDto priceOfferDto;
            if (offer != null)
            {
                priceOfferDto = ObjectMapper.Map<PriceOfferDto>(offer);
                foreach (var item in priceOfferDto.Items)
                {
                    item.ItemName = shippingrequest.ShippingRequestVases.FirstOrDefault(x => x.Id == item.SourceId).VasFk.Name;
                }

            }
            else
            {
                priceOfferDto = new PriceOfferDto()
                {// Set Default data
                    PriceType = PriceOfferType.Trip,
                    Quantity = shippingrequest.NumberOfTrips,
                    Items = GetItems(shippingrequest),
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
        [AbpAuthorize(AppPermissions.Pages_MarketPlace)]
        public async Task<PriceOfferViewDto> GetPriceOfferForView(long OfferId)
        {
            DisableTenancyFilters();
            var offer = await _priceOfferRepository
                .GetAll()
                .Include(i => i.Tenant)
                .Include(i => i.PriceOfferDetails)
                .Include(i => i.ShippingRequestFK)
                 .ThenInclude(v => v.ShippingRequestVases)
                  .ThenInclude(v => v.VasFk)
                  .Where(x => x.Id == OfferId)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId.Value && !x.ShippingRequestFK.IsTachyonDeal)
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
            }

            return priceOfferDto;
        }


        [RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer)]
        public async Task<long> CreateOrEdit(CreateOrEditPriceOfferInput Input)
        {
            //Input.Channel = PriceOfferChannel.MarketPlace;
            return await _priceOfferManager.CreateOrEdit(Input);

        }

        [AbpAuthorize(AppPermissions.Pages_MarketPlace)]
        public async Task<ListResultDto<GetShippingRequestForPriceOfferListDto>> GetAllShippingRequest(ShippingRequestForPriceOfferGetAllInput input)
        {
            DisableTenancyFilters();
            List<GetShippingRequestForPriceOfferListDto> query = new List<GetShippingRequestForPriceOfferListDto>();
            if (input.Channel == PriceOfferChannel.DirectRequest)
            {
                query = await GetFromDirectRequest(input);
            }
            else if (input.Channel == PriceOfferChannel.MarketPlace)
            {
                query = await GetFromMarketPlace(input);
            }

            return new ListResultDto<GetShippingRequestForPriceOfferListDto>(query);
        }

        [RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer)]
        public async Task Delete(EntityDto Input)
        {
            await _priceOfferManager.Delete(Input);
        }
        #endregion

        #region Pricing
        /// <summary>
        /// Get shipping request details for pricing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        [AbpAuthorize(AppPermissions.Pages_MarketPlace)]

        public async Task<GetShippingRequestForPricingOutput> GetShippingRequestForPricing(GetShippingRequestForPricingInput input)
        {
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
                            .FirstOrDefaultAsync(r => r.Id == input.Id && (r.Status == ShippingRequestStatus.NeedsAction || r.Status == ShippingRequestStatus.PrePrice));

            if (shippingRequest == null) throw new UserFriendlyException(L("TheRecordIsNotFound"));

            if (AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier))// Applay permission for carrier if can see the shipping request details
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



            var getShippingRequestForPricingOutput = ObjectMapper.Map<GetShippingRequestForPricingOutput>(shippingRequest);
            getShippingRequestForPricingOutput.Items = ObjectMapper.Map<List<PriceOfferItemDto>>(shippingRequest.ShippingRequestVases);
            getShippingRequestForPricingOutput.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(shippingRequest.GoodCategoryFk).DisplayName;
            getShippingRequestForPricingOutput.TrukType = ObjectMapper.Map<TrucksTypeDto>(shippingRequest.TrucksTypeFk).TranslatedDisplayName;

            return getShippingRequestForPricingOutput;

        }


        #endregion
        #region Helper
        private List<PriceOfferItem> GetItems(ShippingRequest shippingRequest)
        {
            List<PriceOfferItem> items = new List<PriceOfferItem>();
            if (shippingRequest.ShippingRequestVases != null && shippingRequest.ShippingRequestVases.Count > 0)
            {
                var vasIds = shippingRequest.ShippingRequestVases.Select(id => id.VasId).ToList();
                var Tenantvases = _vasPriceRepository
                    .GetAll()
                    .Include(v => v.VasFk)
                    .Where(x => x.TenantId == AbpSession.TenantId.Value && !vasIds.Contains(x.VasId)).ToList();

                foreach (var vas in shippingRequest.ShippingRequestVases)
                {
                    var item = new PriceOfferItem()
                    {
                        SourceId = vas.Id,
                        PriceType = PriceOfferType.Vas,
                        Quantity = vas.RequestMaxAmount + vas.RequestMaxCount,
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
        /// Get TachyonDealer commission settings
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
                     TripCommissionType= (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionType)),
                     TripCommissionPercentage= Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionPercentage)),
                     TripCommissionValue= Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripCommissionValue)),
                     TripMinValueCommission= Convert.ToDecimal(FeatureChecker.GetValue(TenantId, AppFeatures.TachyonDealerTripMinValueCommission)),
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
                            .Where(r => /*r.Status != ShippingRequestDirectRequestStatus.Accepted &&*/ (r.ShippingRequestFK.Status == ShippingRequestStatus.NeedsAction || r.ShippingRequestFK.Status == ShippingRequestStatus.PrePrice))
                            .WhereIf(input.ShippingRequestId.HasValue, x => x.ShippingRequestId == input.ShippingRequestId)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId && !x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId && x.Status != ShippingRequestDirectRequestStatus.Declined)
                            .OrderBy(input.Sorting ?? "id desc")
                            .PageBy(input);

            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

            foreach (var request in directRequests)
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request.ShippingRequestFK);
                if (AbpSession.TenantId.HasValue && !IsEnabled(AppFeatures.Carrier))
                {
                    dto.Name = request.Carrier.Name;
                    dto.RemainingDays = string.Empty;
                    // dto.isPriced = request.Status == ShippingRequestDirectRequestStatus.Response;

                }
                //else if (AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier))
                //{
                //    var ShippingRequestId = request.ShippingRequestId;
                //    dto.isPriced = _priceOfferManager.CheckCarrierIsPricing(ShippingRequestId);
                //}
                var offer = await _priceOfferManager.GetOfferBySource(request.Id, PriceOfferChannel.DirectRequest);
                if (offer != null)
                {
                    dto.isPriced = true;
                    dto.OfferId = offer.Id;
                }
                dto.DirectRequestId = request.Id;
                dto.DirectRequestStatus = request.Status;
                dto.DirectRequestStatusTitle = request.Status.GetEnumDescription();
                dto.BidStatusTitle = string.Empty;
                dto.TrukType = ObjectMapper.Map<TrucksTypeDto>(request.ShippingRequestFK.TrucksTypeFk).TranslatedDisplayName;
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
                .Where(x => x.IsBid && (x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice))
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                .WhereIf(AbpSession.TenantId.HasValue && await IsEnabledAsync(AppFeatures.Carrier), x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);


            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

            foreach (var request in query)
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request);

                if (AbpSession.TenantId.HasValue && IsEnabled(AppFeatures.Carrier))
                {
                    var offer = _priceOfferManager.GetCarrierPricingOrNull(request.Id);
                    if (offer != null)
                    {
                        dto.OfferId = offer.Id;
                        dto.isPriced = true;

                    }
                    dto.StatusTitle = "";
                }

                dto.TrukType = ObjectMapper.Map<TrucksTypeDto>(request.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.GoodCategoryFk).DisplayName;
                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;
        }
        #endregion




    }
}
