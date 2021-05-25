using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.MarketPlaces.Dto;
using TACHYON.Shipping.ShippingRequests;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequests.Dtos;
using Abp.Authorization;
using TACHYON.Authorization;
using Abp.Application.Features;
using Abp.UI;
using Abp.Configuration;
using TACHYON.Notifications;

namespace TACHYON.MarketPlaces
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class MarketPlaceAppService : TACHYONAppServiceBase, IMarketPlaceAppService
    {
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private IRepository<ShippingRequestPricing, long> _shippingRequestPricingRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly ISettingManager _settingManager;
        private readonly IAppNotifier _appNotifier;
        public MarketPlaceAppService(
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            IRepository<ShippingRequestPricing, long> shippingRequestPricingRepository,
            IFeatureChecker featureChecker,
            ISettingManager settingManager,
            IAppNotifier appNotifier)
        {
            _shippingRequestsRepository = shippingRequestsRepository;
            _shippingRequestPricingRepository = shippingRequestPricingRepository;
            _featureChecker = featureChecker;
            _settingManager = settingManager;
            _appNotifier = appNotifier;
        }
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer, AppFeatures.Carrier)]
        public async Task<PagedResultDto<MarketPlaceListDto>> GetAll(GetAllMarketPlaceInput Input)
        {
            DisableTenancyFilters();
            var query = _shippingRequestsRepository
                .GetAll()
                .AsNoTracking()
                    .Include(t=>t.Tenant)
                    .Include(oc=> oc.OriginCityFk)
                    .Include(dc => dc.DestinationCityFk)
                    .Include(c => c.GoodCategoryFk)
                .Where(x=>x.IsBid)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
                .OrderBy(Input.Sorting ?? "id desc");

            var ResultPaging = await query.PageBy(Input).ToListAsync();
            var marketPlaceListDto = ObjectMapper.Map<List<MarketPlaceListDto>>(ResultPaging);


            if (await IsEnabledAsync(AppFeatures.Carrier))
            {
                marketPlaceListDto.ForEach(  x =>
                {
                    x.CarrierPricing =ObjectMapper.Map<ShippingRequestCarrierPricingDto>( GetCarrierPricingOrNull(x.Id)) ;
                });
            }
            var totalCount = await query.CountAsync();
            return new PagedResultDto<MarketPlaceListDto>(
                totalCount,
                marketPlaceListDto
            );

        }

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CreateOrEdit(CreateOrEditMarketPlaceBidInput Input)
        {
            DisableTenancyFilters();
            var shippingRequest = await _shippingRequestsRepository
                .GetAll()
                .Include(v => v.ShippingRequestVases)
                .FirstOrDefaultAsync(r => r.Id == Input.ShippingRequestId && r.BidStatus == ShippingRequestBidStatus.OnGoing);
            if (shippingRequest == null) throw new UserFriendlyException(L("The Bid must be Ongoing message"));
            
            if (Input.IsNew)
            {
                await Create(Input, shippingRequest);
            }
            else
            {
                await Update(Input, shippingRequest);
            }

        }

        public async  Task Delete(EntityDto Input)
        {
            DisableTenancyFilters();
            var pricing = await _shippingRequestPricingRepository
                .FirstOrDefaultAsync(x => x.Id == Input.Id && x.TenantId == AbpSession.TenantId.Value && x.ShippingRequestFK.BidStatus == ShippingRequestBidStatus.OnGoing);
            if (pricing==null) throw new UserFriendlyException(L("TheRecordNotFound"));
           await _shippingRequestPricingRepository.DeleteAsync(pricing);
        }

        #region Helper
        /// <summary>
        /// If the current user login have feature carrier get the bid price
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        private ShippingRequestPricing GetCarrierPricingOrNull(long requestId)
        {
            return  _shippingRequestPricingRepository.GetAll().Include(x=>x.ShippingRequestVasesPricing).FirstOrDefault(x => x.ShippingRequestId == requestId && x.TenantId == AbpSession.TenantId.Value);
        }

        private async Task Create(CreateOrEditMarketPlaceBidInput Input, ShippingRequest shippingRequest)
        {
            var Pricing = GetCarrierPricingOrNull(shippingRequest.Id);

            if (Pricing != null) throw new UserFriendlyException(L("YouAlreadyAddPricingForThisShipping"));

            Pricing = ObjectMapper.Map<ShippingRequestPricing>(Input);
            Pricing.Channel = ShippingRequestPricingChannel.MarketPlace;
            Pricing.ShippingRequestVasesPricing = await GetListOfVases(Input, shippingRequest);
            Pricing.Calculate(_featureChecker, _settingManager, shippingRequest);
            await _shippingRequestPricingRepository.InsertAsync(Pricing);

            await _appNotifier.ShippingRequestSendOfferWhenAddPrice(shippingRequest);

        }
        private async Task Update(CreateOrEditMarketPlaceBidInput Input, ShippingRequest shippingRequest)
        {
            var Pricing = GetCarrierPricingOrNull(shippingRequest.Id);
            if (Pricing == null) throw new UserFriendlyException(L("TheShippingIsNotFound"));
            if (Pricing.Status == ShippingRequestPricingStatus.Accepted) throw new UserFriendlyException(L("YourPriceAlreadyAcceptedYouCanEdit"));
            // if (Pricing.Channel  != ShippingRequestPricingChannel.MarketPlace) throw new UserFriendlyException(L("YourPriceAlreadyAcceptedYouCanEdit"));
            ObjectMapper.Map(Input, Pricing);
            Pricing.ShippingRequestVasesPricing.Clear();
            Pricing.ShippingRequestVasesPricing = await GetListOfVases(Input, shippingRequest);
            Pricing.Calculate(_featureChecker, _settingManager, shippingRequest);
            if (Pricing.IsView)
            {
                Pricing.IsView = false;
                await _appNotifier.ShippingRequestSendOfferWhenUpdatePrice(shippingRequest);
            }
        }

        private Task SentNotification(ShippingRequest shippingRequest)
        {
            return Task.CompletedTask;
        }

        private Task<List<ShippingRequestVasPricing>> GetListOfVases(CreateOrEditMarketPlaceBidInput Input, ShippingRequest shippingRequest)
        {
            List<ShippingRequestVasPricing> ShippingRequestVasesPricing = new List<ShippingRequestVasPricing>();
            if (Input.ShippingRequestVasPricing.Count != shippingRequest.ShippingRequestVases.Count)
            {
                throw new UserFriendlyException(L("YouSholudAddPricesForAllVases"));
            }
            if (Input.ShippingRequestVasPricing.Any(x=>x.Price<=0)) throw new UserFriendlyException(L("ThePriceMustBeGreaterThanZero"));
            foreach (var vas in shippingRequest.ShippingRequestVases)
            {
                var vasdto = Input.ShippingRequestVasPricing.FirstOrDefault(x => x.VasId == vas.Id);
                if (vasdto == null) throw new UserFriendlyException(L("YouSholudAddVasRelatedWithShippingRequest"));

                ShippingRequestVasesPricing.Add(new ShippingRequestVasPricing()
                {
                    ShippingRequestVasFK = vas,
                    VasPrice = vasdto.Price
                }); ;
            }
            return Task.FromResult(ShippingRequestVasesPricing);
        }
        #endregion
    }
}
