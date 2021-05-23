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

namespace TACHYON.MarketPlaces
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestBids)]
    public class MarketPlaceAppService : TACHYONAppServiceBase, IMarketPlaceAppService
    {
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private IRepository<ShippingRequestPricing, long> _shippingRequestPricingRepository;
        private CommissionManager _commissionManager;

        public MarketPlaceAppService(
            IRepository<ShippingRequest, long> shippingRequestsRepository,
            IRepository<ShippingRequestPricing, long> shippingRequestPricingRepository,
            CommissionManager commissionManager)
        {
            _shippingRequestsRepository = shippingRequestsRepository;
            _shippingRequestPricingRepository = shippingRequestPricingRepository;
            _commissionManager = commissionManager;
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
                marketPlaceListDto.ForEach( async x =>
                {
                    x.CarrierPricing =ObjectMapper.Map<ShippingRequestCarrierPricingDto>(await GetCarrierPricingOrNull(x.Id)) ;
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
            var shippingRequest = await _shippingRequestsRepository.FirstOrDefaultAsync(r=>r.Id== Input.Id && r.BidStatus == ShippingRequestBidStatus.OnGoing);
           if (shippingRequest==null) throw new UserFriendlyException(L("The Bid must be Ongoing message"));
           if (Input.IsNew)
            {
               await Create(Input, shippingRequest);
            }
           else
            {
                await Update(Input, shippingRequest);
            }
           
        }

        public Task Delete(EntityDto Input)
        {
            throw new NotImplementedException();
        }

        #region Helper
        /// <summary>
        /// If the current user login have feature carrier get the bid price
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        private async Task<ShippingRequestPricing> GetCarrierPricingOrNull(long requestId)
        {
            return await _shippingRequestPricingRepository.FirstOrDefaultAsync(x => x.ShippingRequestId == requestId && x.TenantId == AbpSession.TenantId.Value);
        }

        private async Task Create(CreateOrEditMarketPlaceBidInput Input, ShippingRequest shippingRequest)
        {
            var Pricing = await GetCarrierPricingOrNull(shippingRequest.Id);
            if (Pricing !=null) throw new UserFriendlyException(L("YouAlreadyAddPricingForThisShipping"));

             Pricing=ObjectMapper.Map<ShippingRequestPricing>(Input);

        }
        private async Task Update(CreateOrEditMarketPlaceBidInput Input, ShippingRequest shippingRequest)
        {
            var Pricing = await GetCarrierPricingOrNull(shippingRequest.Id);
            if (Pricing == null) throw new UserFriendlyException(L("TheShippingIsNotFound"));
            if (Pricing.Status== ShippingRequestPricingStatus.Accepted) throw new UserFriendlyException(L("YourPriceAlreadyAcceptedYouCanEdit"));
        }

        private Task SentNotification(ShippingRequest shippingRequest)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
