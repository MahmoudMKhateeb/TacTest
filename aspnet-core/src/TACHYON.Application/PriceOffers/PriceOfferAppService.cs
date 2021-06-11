using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks.TrucksTypes.Dtos;

namespace TACHYON.PriceOffers
{
    public class PriceOfferAppService : TACHYONAppServiceBase, IPriceOfferAppService
    {
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private IRepository<ShippingRequest, long> _shippingRequestsRepository;
        private readonly PriceOfferManager _priceOfferManager;


        public PriceOfferAppService(IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository, IRepository<ShippingRequest, long> shippingRequestsRepository, PriceOfferManager priceOfferManager)
        {
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _shippingRequestsRepository = shippingRequestsRepository;
            _priceOfferManager = priceOfferManager;
        }

        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer, AppFeatures.Carrier)]
        public async Task<ListResultDto<GetShippingRequestForPriceOfferListDto>> GetAllShippingRequest(ShippingRequestForPriceOfferGetAllInput input)
        {
            DisableTenancyFilters();
            List<GetShippingRequestForPriceOfferListDto> query=new List<GetShippingRequestForPriceOfferListDto>();
            if (input.Channel== PriceOfferChannel.DirectRequest)
            {
                query = await GetFromDirectRequest(input);
            }
            else if (input.Channel == PriceOfferChannel.MarketPlace)
            {
                query = await GetFromMarketPlace(input);
            }

            return new ListResultDto<GetShippingRequestForPriceOfferListDto>(query);
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
                            .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.ShippingRequestFK.TenantId == AbpSession.TenantId && !x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(await IsEnabledAsync(AppFeatures.TachyonDealer), x => x.ShippingRequestFK.IsTachyonDeal)
                            .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.CarrierTenantId == AbpSession.TenantId)
                            .OrderBy(input.Sorting ?? "id desc")
                            .PageBy(input);

            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList=new List<GetShippingRequestForPriceOfferListDto>();

            foreach(var request in directRequests)
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request.ShippingRequestFK);
                if (!IsEnabled(AppFeatures.Carrier))
                {
                    dto.Name = request.Carrier.Name;
                    dto.RemainingDays = string.Empty;
                    dto.isPriced = request.Status == ShippingRequestDirectRequestStatus.Response;

                }
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
                .WhereIf(await IsEnabledAsync(AppFeatures.Shipper), x => x.TenantId == AbpSession.TenantId && !x.IsTachyonDeal)
                .WhereIf(await IsEnabledAsync(AppFeatures.Carrier), x => x.BidStatus == ShippingRequestBidStatus.OnGoing)
                .OrderBy(input.Sorting ?? "id desc")
                .PageBy(input);


            List<GetShippingRequestForPriceOfferListDto> ShippingRequestForPriceOfferList = new List<GetShippingRequestForPriceOfferListDto>();

            foreach (var request in query)
            {
                var dto = ObjectMapper.Map<GetShippingRequestForPriceOfferListDto>(request);

                if (IsEnabled(AppFeatures.Carrier)) {
                    dto.isPriced = _priceOfferManager.CheckCarrierIsPricing(request.Id);
                    dto.StatusTitle = "";
                }

                dto.TrukType = ObjectMapper.Map<TrucksTypeDto>(request.TrucksTypeFk).TranslatedDisplayName;
                dto.GoodsCategory = ObjectMapper.Map<GoodCategoryDto>(request.GoodCategoryFk).DisplayName;
                ShippingRequestForPriceOfferList.Add(dto);

            }

            return ShippingRequestForPriceOfferList;
        }
    }
}
