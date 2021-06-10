using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.MarketPlaces.Dto;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.MarketPlaces
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class MarketPlaceAppService : TACHYONAppServiceBase, IMarketPlaceAppService
    {
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly PriceOfferManager _priceOfferManager;
        public MarketPlaceAppService(
            IRepository<ShippingRequest, long> shippingRequestsRepository, PriceOfferManager priceOfferManager)
        {
            _shippingRequestsRepository = shippingRequestsRepository;
            _priceOfferManager = priceOfferManager;
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
                     .ThenInclude(x=>x.Translations)
                    .Include(t=>t.TrucksTypeFk)
                     .ThenInclude(x => x.Translations)
                .Where(x=>x.IsBid && (x.Status == ShippingRequestStatus.NeedsAction || x.Status == ShippingRequestStatus.PrePrice))
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
                .OrderBy(Input.Sorting ?? "id desc");

            var ResultPaging = await query.PageBy(Input).ToListAsync();
            List<Dictionary<object, string>> GoodsCategory = new List<Dictionary<object, string>>();
            List<Dictionary<object, string>> TruckTypes = new List<Dictionary<object, string>>();
            ResultPaging.ForEach(x =>
            {
                GoodsCategory.Add(new Dictionary<object, string>()
                {
                    [x.Id] = ObjectMapper.Map<GoodCategoryDto>(x.GoodCategoryFk).DisplayName
                }
                ) ;

                TruckTypes.Add(new Dictionary<object, string>()
                {
                    [x.Id] = ObjectMapper.Map<TrucksTypeDto>(x.TrucksTypeFk).TranslatedDisplayName
                }
                );
            });
            var marketPlaceListDto = ObjectMapper.Map<List<MarketPlaceListDto>>(ResultPaging);


            if (await IsEnabledAsync(AppFeatures.Carrier))
            {
                marketPlaceListDto.ForEach(  x =>
                {
                    x.IsPricing = _priceOfferManager.CheckCarrierIsPricing(x.Id);
                   // x.CarrierPricing =ObjectMapper.Map<ShippingRequestCarrierPricingDto>(_priceOfferManager.GetCarrierPricingOrNull(x.Id)) ;
                });
            }
            marketPlaceListDto.ForEach(x =>
            {
                x.GoodsCategory = GoodsCategory.Where(g => g.ContainsKey(x.Id)).Select(g => g[x.Id]).FirstOrDefault();
                x.TrukType = TruckTypes.Where(g => g.ContainsKey(x.Id)).Select(g => g[x.Id]).FirstOrDefault();

            });
            var totalCount = await query.CountAsync();
            return new PagedResultDto<MarketPlaceListDto>(
                totalCount,
                marketPlaceListDto
            );

        }

        [RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer)]
        public async Task CreateOrEdit(CreateOrEditPriceOfferInput Input)
        {
           Input.Channel = PriceOfferChannel.MarketPlace;
           await _priceOfferManager.CreateOrEdit(Input);

        }

        public async  Task Delete(EntityDto Input)
        {
            await _priceOfferManager.Delete(Input);
        }


    }
}
