using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Cities;
using TACHYON.Cities.Dtos;
using TACHYON.Common;
using TACHYON.Configuration;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Notifications;
using TACHYON.PriceOffers.Dto;
using TACHYON.PriceOffers.Dto._new;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Vases;

namespace TACHYON.PriceOffers
{
    public class New_PriceOfferAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<TrucksType, long> _trucksTypeRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<VasPrice> _vasPriceRepository;

        public New_PriceOfferAppService(
            IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository,
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            PriceOfferManager priceOfferManager,
            IRepository<PriceOffer, long> priceOfferRepository,
            IRepository<VasPrice> vasPriceRepository,
            IRepository<City> cityRepository,
            IRepository<TrucksType, long> trucksTypeRepository,
            IAppNotifier appNotifier,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository)
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
        }


        /// <summary>
        /// Get the price offer when the user need to create offer or edit
        /// </summary>
        /// <param name="shippingRequestId"></param>
        /// <param name="offerId"></param>
        /// <returns></returns>
        /// 
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier)]
        public async Task<CreateOrEditPriceOffer> GetPriceOfferForCreateOrEdit(long shippingRequestId, long? offerId)
        {
            DisableTenancyFilters();
            var shippingRequest = await _shippingRequestsRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                .ThenInclude(v => v.VasFk)
                .FirstOrDefaultAsync(x => x.Id == shippingRequestId);

            if (!shippingRequest.Status.IsIn(ShippingRequestStatus.PrePrice, ShippingRequestStatus.NeedsAction,
                    ShippingRequestStatus.AcceptedAndWaitingCarrier))
                throw new UserFriendlyException(L("YouCantCreateOrAddPriceForThisShipment"));

            if (shippingRequest == null)
                throw new UserFriendlyException(L("TheRecordIsNotFound"));

            var offer = await _priceOfferRepository
                .GetAll()
                .Include(i => i.PriceOfferDetails)
                .Where(x => x.ShippingRequestId == shippingRequest.Id)
                .WhereIf(offerId.HasValue, x => x.Id == offerId.Value)
                .WhereIf(await IsCarrier(),
                    x => x.TenantId == AbpSession.TenantId.Value &&
                         (x.Status == PriceOfferStatus.New || x.Status == PriceOfferStatus.Rejected))
                .WhereIf(await IsTachyonDealer(),
                    x => ((x.TenantId == AbpSession.TenantId.Value || x.ShippingRequestFk.IsTachyonDeal) &&
                          (x.Status == PriceOfferStatus.New ||
                           (x.Status == PriceOfferStatus.Rejected &&
                            x.Tenant.EditionId == TachyonEditionId) || x.Status == PriceOfferStatus.Pending)))
                .OrderBy(x => x.Status)
                .FirstOrDefaultAsync();
            CreateOrEditPriceOffer createOrEditPriceOffer = default;
            if (offer != null)
            {
                createOrEditPriceOffer.PriceOfferDto = ObjectMapper.Map<PriceOfferDto>(offer);
                foreach (var item in createOrEditPriceOffer.PriceOfferDto.Items)
                {
                    item.ItemName = shippingRequest.ShippingRequestVases.FirstOrDefault(x => x.Id == item.SourceId)
                        ?.VasFk?.Key;
                }

                if (await IsTachyonDealer())
                {
                    if (createOrEditPriceOffer.PriceOfferDto.Items != null &&
                        createOrEditPriceOffer.PriceOfferDto.Items.Count > 0)
                    {
                        var item = createOrEditPriceOffer.PriceOfferDto.Items.FirstOrDefault();
                        createOrEditPriceOffer.PriceOfferDto.VasCommissionPercentageOrAddValue =
                            item.CommissionPercentageOrAddValue.Value;
                        createOrEditPriceOffer.PriceOfferDto.VasCommissionType = item.CommissionType;
                    }

                    if (offer.TenantId != AbpSession.TenantId.Value)
                    {
                        createOrEditPriceOffer.PriceOfferDto.ParentId = offer.Id;
                    }
                }
            }
            else
            {
                createOrEditPriceOffer.PriceOfferDto = new PriceOfferDto()
                {
                    // Set Default data
                    PriceType = PriceOfferType.Trip,
                    Quantity = shippingRequest.NumberOfTrips,
                    Items = GetVases(shippingRequest),
                    TaxVat = (decimal)Convert.ChangeType(
                        await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.TaxVat), typeof(decimal))
                };
                await FillCommissionSettingsIfTms(createOrEditPriceOffer.PriceOfferDto, shippingRequest.TenantId);
            }

            if (await IsTachyonDealer())
            {
                createOrEditPriceOffer.PriceOfferDto.CommissionSettings =
                    GetPriceOfferTenantCommissionSettings(shippingRequest.TenantId);
            }

            return createOrEditPriceOffer;
        }


        /// <summary>
        /// Get all VAS item related with shipment
        /// </summary>
        /// <param name="shippingRequest"></param>
        /// <returns></returns>
        private List<PriceOfferItem> GetVases(ShippingRequest shippingRequest)
        {
            List<PriceOfferItem> items = new List<PriceOfferItem>();
            if (shippingRequest.ShippingRequestVases == null || shippingRequest.ShippingRequestVases.Count <= 0)
            {
                return items;
            }

            var vasIds = shippingRequest.ShippingRequestVases.Select(id => id.VasId).ToList();
            var tenantVases = _vasPriceRepository
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
                var vasDefine = tenantVases.FirstOrDefault(x => x.VasId == vas.VasId);
                if (vasDefine != null)
                {
                    if (vasDefine.Price != null)
                    {
                        item.ItemPrice = (decimal)vasDefine.Price;
                    }

                    item.ItemTotalAmount = item.ItemPrice * item.Quantity;
                }

                items.Add(item);
            }

            return items;
        }


        /// <summary>
        /// Get TachyonDealer default commission settings for tenant
        /// </summary>
        private async Task FillCommissionSettingsIfTms(PriceOfferDto offer, int tenantId)
        {
            if (!await IsTachyonDealer())
            {
                return;
            }

            offer.CommissionType =
                (await FeatureChecker.GetValueAsync(tenantId, AppFeatures.TachyonDealerTripCommissionType))
                .To<PriceOfferCommissionType>();
            if (offer.CommissionType == PriceOfferCommissionType.CommissionPercentage)
            {
                offer.CommissionPercentageOrAddValue = Convert.ToDecimal(
                    await FeatureChecker.GetValueAsync(tenantId, AppFeatures.TachyonDealerTripCommissionPercentage));
            }
            else
            {
                offer.CommissionPercentageOrAddValue = Convert.ToDecimal(
                    await FeatureChecker.GetValueAsync(tenantId, AppFeatures.TachyonDealerTripCommissionValue));
            }

            offer.VasCommissionType =
                (await FeatureChecker.GetValueAsync(tenantId, AppFeatures.TachyonDealerVasCommissionType))
                .To<PriceOfferCommissionType>();

            if (offer.VasCommissionType == PriceOfferCommissionType.CommissionPercentage)
            {
                offer.VasCommissionPercentageOrAddValue = Convert.ToDecimal(
                    await FeatureChecker.GetValueAsync(tenantId, AppFeatures.TachyonDealerVasCommissionPercentage));
            }
            else
            {
                offer.VasCommissionPercentageOrAddValue = Convert.ToDecimal(
                    await FeatureChecker.GetValueAsync(tenantId, AppFeatures.TachyonDealerVasCommissionValue));
            }
        }

        private PriceOfferTenantCommissionSettings GetPriceOfferTenantCommissionSettings(int tenantId)
        {
            PriceOfferTenantCommissionSettings commissionSettings =
                new PriceOfferTenantCommissionSettings()
                {
                    ItemCommissionType =
                        (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(tenantId,
                            AppFeatures.TachyonDealerTripCommissionType)),
                    ItemCommissionPercentage =
                        Convert.ToDecimal(FeatureChecker.GetValue(tenantId,
                            AppFeatures.TachyonDealerTripCommissionPercentage)),
                    ItemCommissionValue =
                        Convert.ToDecimal(FeatureChecker.GetValue(tenantId,
                            AppFeatures.TachyonDealerTripCommissionValue)),
                    ItemMinValueCommission =
                        Convert.ToDecimal(FeatureChecker.GetValue(tenantId,
                            AppFeatures.TachyonDealerTripMinValueCommission)),
                    VasCommissionType =
                        (PriceOfferCommissionType)Convert.ToByte(FeatureChecker.GetValue(tenantId,
                            AppFeatures.TachyonDealerVasCommissionType)),
                    VasCommissionPercentage =
                        Convert.ToDecimal(FeatureChecker.GetValue(tenantId,
                            AppFeatures.TachyonDealerVasCommissionPercentage)),
                    VasCommissionValue =
                        Convert.ToDecimal(FeatureChecker.GetValue(tenantId,
                            AppFeatures.TachyonDealerVasCommissionValue)),
                    VasMinValueCommission = Convert.ToDecimal(FeatureChecker.GetValue(tenantId,
                        AppFeatures.TachyonDealerVasMinValueCommission))
                };

            return commissionSettings;
        }


        //---C

        public async Task<PriceOfferDto> InitPriceOffer(CreateOrEditPriceOfferInput input)
        {
            var offer = await _priceOfferManager.InitPriceOffer(input);
            var dto = ObjectMapper.Map<PriceOfferDto>(offer);
            return dto;
        }
    }
}