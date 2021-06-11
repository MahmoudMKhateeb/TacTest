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
