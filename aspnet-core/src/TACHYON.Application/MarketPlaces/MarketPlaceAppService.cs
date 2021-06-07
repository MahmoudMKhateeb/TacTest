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
using Abp.Threading;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Shipping.ShippingRequests.ShippingRequestsPricing.Dto;

namespace TACHYON.MarketPlaces
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class MarketPlaceAppService : TACHYONAppServiceBase, IMarketPlaceAppService
    {
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private IRepository<ShippingRequestPricing, long> _shippingRequestPricingRepository;
        private readonly ShippingRequestPricingManager _shippingRequestPricingManager;
        public MarketPlaceAppService(
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            IRepository<ShippingRequestPricing, long> shippingRequestPricingRepository,
            ShippingRequestPricingManager shippingRequestPricingManager)
        {
            _shippingRequestsRepository = shippingRequestsRepository;
            _shippingRequestPricingRepository = shippingRequestPricingRepository;
            _shippingRequestPricingManager = shippingRequestPricingManager;
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
                .Where(x=>x.IsBid)
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
                .OrderBy(Input.Sorting ?? "id desc");

            var ResultPaging = await query.PageBy(Input).ToListAsync();
            List<Dictionary<object, string>> GoodsCategory = new List<Dictionary<object, string>>();
            ResultPaging.ForEach(x =>
            {
                GoodsCategory.Add(new Dictionary<object, string>()
                {
                    [x.Id] = ObjectMapper.Map<GoodCategoryDto>(x.GoodCategoryFk).DisplayName
                }
                ) ;
            });
            var marketPlaceListDto = ObjectMapper.Map<List<MarketPlaceListDto>>(ResultPaging);


            if (await IsEnabledAsync(AppFeatures.Carrier))
            {
                marketPlaceListDto.ForEach(  x =>
                {
                    x.CarrierPricing =ObjectMapper.Map<ShippingRequestCarrierPricingDto>(_shippingRequestPricingManager.GetCarrierPricingOrNull(x.Id)) ;
                });
            }
            marketPlaceListDto.ForEach(x =>
            {
                x.GoodsCategory = GoodsCategory.Where(g => g.ContainsKey(x.Id)).Select(g => g[x.Id]).FirstOrDefault();
            });
            var totalCount = await query.CountAsync();
            return new PagedResultDto<MarketPlaceListDto>(
                totalCount,
                marketPlaceListDto
            );

        }

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CreateOrEdit(CreateOrEditPricingInput Input)
        {
           Input.Channel = ShippingRequestPricingChannel.MarketPlace;
           await _shippingRequestPricingManager.CreateOrEdit(Input);

        }

        public async  Task Delete(EntityDto Input)
        {
            await _shippingRequestPricingManager.Delete(Input);
            // DisableTenancyFilters();
            // var pricing = await _shippingRequestPricingRepository
            //     .FirstOrDefaultAsync(x => x.Id == Input.Id && x.TenantId == AbpSession.TenantId.Value && x.ShippingRequestFK.BidStatus == ShippingRequestBidStatus.OnGoing);
            // if (pricing==null) throw new UserFriendlyException(L("TheRecordNotFound"));
            //await _shippingRequestPricingRepository.DeleteAsync(pricing);
        }


    }
}
