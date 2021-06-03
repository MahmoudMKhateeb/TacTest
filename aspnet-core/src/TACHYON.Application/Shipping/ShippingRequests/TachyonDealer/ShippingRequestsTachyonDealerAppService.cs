using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos;
using TACHYON.TachyonPriceOffers;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Abp.Linq.Extensions;
using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;
using Abp.Domain.Uow;
using TACHYON.MultiTenancy;
using TACHYON.Shipping.ShippingRequestBids;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer
{
    public class ShippingRequestsTachyonDealerAppService : TACHYONAppServiceBase, IShippingRequestsTachyonDealerAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<TachyonPriceOffer> _tachyonPriceOfferRepository;
        private readonly IRepository<ShippingRequestsCarrierDirectPricing> _CarrierDirectPricingRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<ShippingRequestBid, long> _shippingRequestBidsRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly CommissionManager _commissionManager;
        public ShippingRequestsTachyonDealerAppService(
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<TachyonPriceOffer> tachyonPriceOfferRepository,
            IRepository<ShippingRequestsCarrierDirectPricing> shippingRequestsCarrierDirectPricingRepository,
            IRepository<Tenant> tenantRepository,
            IRepository<ShippingRequestBid, long> shippingRequestBidsRepository,
            IAppNotifier appNotifier,
            CommissionManager commissionManager
            )
        {
            _shippingRequestRepository = shippingRequestRepository;
            _tachyonPriceOfferRepository = tachyonPriceOfferRepository;
            _CarrierDirectPricingRepository = shippingRequestsCarrierDirectPricingRepository;
            _tenantRepository = tenantRepository;
            _shippingRequestBidsRepository = shippingRequestBidsRepository;
            _appNotifier = appNotifier;
            _commissionManager = commissionManager;
        }
        [RequiresFeature(AppFeatures.TachyonDealer)]

        public async Task StartBid(TachyonDealerBidDtoInupt Input)
        {
            DisableTenancyFilters();
            ShippingRequest shippingRequest = await _shippingRequestRepository.
                FirstOrDefaultAsync(e => e.Id == Input.Id && e.IsTachyonDeal && !e.IsBid && e.Status == ShippingRequestStatus.PrePrice);
            if (shippingRequest != null)
            {
                shippingRequest.IsBid = true;
                shippingRequest.BidStartDate = Input.StartDate;
                shippingRequest.BidEndDate = Input.EndDate;
                if (Input.StartDate.Date==Clock.Now.Date) shippingRequest.BidStatus = ShippingRequestBidStatus.OnGoing;
            }
        }

        /// <summary>
        /// User tachyon dealer sent request to get offer from carrirer
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.SendDirectRequest)]

        public async Task SendDriectRequestForCarrier(TachyonDealerCreateDirectOfferToCarrirerInuptDto Input)
        {
            if (!FeatureChecker.IsEnabled(Input.TenantId, AppFeatures.Carrier)) new UserFriendlyException(L("TheTenantAddIsNotCarrierTenant"));

            DisableTenancyFilters();
            ShippingRequest shippingRequest= await GetShippingRequestOnPrePriceStage(Input.Id);
            //if (shippingRequest.IsBid) throw new UserFriendlyException(L("YouCanNotSendDriectRequestWhenTheSippingIsBid"));

            if (await _CarrierDirectPricingRepository.GetAll().AnyAsync(x => x.CarrirerTenantId == Input.TenantId && x.RequestId == Input.Id))
                throw new UserFriendlyException(L("YouAlreadyAddThisCarrrierToThisShipping"));
            //shippingRequest.CarrierPriceType = CarrierPriceType.TachyonDirectRequest;
            if (!shippingRequest.IsTachyonDeal)
            {
                shippingRequest.Status = ShippingRequestStatus.NeedsAction;
            }
            await _CarrierDirectPricingRepository.InsertAsync(new ShippingRequestsCarrierDirectPricing(AbpSession.TenantId.Value, shippingRequest.Id,AbpSession.UserId.Value, Input.TenantId));
            await _appNotifier.SendDriectRequestForCarrier(Input.TenantId, shippingRequest);
        }
        /// <summary>
        /// List all direct request
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.TachyonDealer, AppFeatures.Carrier)]
        public async Task<PagedResultDto<ShippingRequestsCarrierDirectPricingListDto>> GetDriectRequestForAllCarriers(TachyonDealerCreateDirectOfferToCarrirerFilterInput Input)
        {
            DisableTenancyFilters();
            var query = _CarrierDirectPricingRepository
                .GetAll()
                .AsNoTracking()
                .Include(r => r.Request)
                 .ThenInclude(t=>t.Tenant)
                .Include(t=>t.Carrier)
                //.Where(x => x.Request.Status == ShippingRequestStatus.PrePrice && x.Request.IsTachyonDeal && x.Request.CarrierPriceType== CarrierPriceType.TachyonDirectRequest)
                .WhereIf(IsEnabled(AppFeatures.Carrier), e => e.CarrirerTenantId == AbpSession.TenantId && (e.Request.Status == ShippingRequestStatus.PrePrice || e.Request.Status == ShippingRequestStatus.NeedsAction))
                .WhereIf(IsEnabled(AppFeatures.TachyonDealer), e => e.TenantId == AbpSession.TenantId)
                .WhereIf(Input.RequestId.HasValue,e=>e.RequestId== Input.RequestId.Value)
                .OrderBy(Input.Sorting ?? "id desc")
                .PageBy(Input);
            var totalCount = await query.CountAsync();
            return new PagedResultDto<ShippingRequestsCarrierDirectPricingListDto>(
                totalCount,
                ObjectMapper.Map<List<ShippingRequestsCarrierDirectPricingListDto>>(await query.ToListAsync())
            );
        }
        /// <summary>
        /// Carrirer set price for request 
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.Carrier)]
        public async Task CarrierSetPriceForDirectRequest(CarrirSetPriceForDirectRequestDto Input)
        {
            DisableTenancyFilters();
            var Pricing = await _CarrierDirectPricingRepository.GetAllIncluding(r => r.Request,c=>c.Carrier)
                .FirstOrDefaultAsync(x => x.Id == Input.Id &&
                (x.Request.Status == ShippingRequestStatus.PrePrice || x.Request.Status == ShippingRequestStatus.NeedsAction) &&
                x.Status == ShippingRequestsCarrierDirectPricingStatus.None &&
                x.CarrirerTenantId == AbpSession.TenantId);
            if (Pricing == null) throw new UserFriendlyException(L("YouCanNotPriceThisRequest"));
            
            Pricing.Status = ShippingRequestsCarrierDirectPricingStatus.Response;

            //var Calculate = await _commissionManager.CalculateAmount(true, Input.Price, Pricing.Request);
            Pricing.Price = Input.Price;
            await _appNotifier.DriectRequestCarrierRespone(Pricing);

        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<ShippingRequestAmountDto> GetCarrierPricing(GetCarrierPricingInputDto Input)
        {
            DisableTenancyFilters();
            decimal Price = 0;
            ShippingRequest shippingRequest=default(ShippingRequest);
            ShippingRequestAmountDto PricingDto = new ShippingRequestAmountDto();
            if (Input.DirectRequestId.HasValue)
            {
                var Pricing = await _CarrierDirectPricingRepository.GetAll()
                    .Include(T => T.Carrier)
                    .Include(r => r.Request)
                    .FirstOrDefaultAsync
                    (
                        x => x.Id == Input.DirectRequestId.Value &&
                        x.Status == ShippingRequestsCarrierDirectPricingStatus.Response &&
                        x.TenantId == AbpSession.TenantId
                    );
                if (Pricing == null) throw new UserFriendlyException(L("YouCanNotPriceThisRequest"));
                Price = Pricing.Price.Value;
                shippingRequest = Pricing.Request;
                ObjectMapper.Map(Pricing, PricingDto);

            }
            else
            {
                ShippingRequestBid bid = await _shippingRequestBidsRepository.GetAll()
                    .Include(x => x.ShippingRequestFk)
                     .Include(x => x.Tenant)
                    .FirstOrDefaultAsync(x => x.Id == Input.ShippingRequestBidId.Value &&
                    x.ShippingRequestFk.IsBid &&
                    x.ShippingRequestFk.IsTachyonDeal &&
                   ( x.ShippingRequestFk.Status == ShippingRequestStatus.NeedsAction || x.ShippingRequestFk.Status == ShippingRequestStatus.PrePrice) &&
                    x.ShippingRequestFk.BidStatus == ShippingRequestBidStatus.OnGoing);
                if (bid == null)
                {
                    throw new UserFriendlyException(L("BidIsNotExistMessage"));
                }
                Price = bid.BasePrice;
                shippingRequest = bid.ShippingRequestFk;
                ObjectMapper.Map(bid, PricingDto);
            }

            var offer = await _tachyonPriceOfferRepository.FirstOrDefaultAsync(x => x.ShippingRequestId == shippingRequest.Id && x.OfferStatus == OfferStatus.AcceptedAndWaitingForCarrier);

            if (offer != null) //If offer guesing price no need to apply the commission
            {
                PricingDto.SubTotalAmount = offer.SubTotalAmount.Value;
                PricingDto.VatAmount = offer.VatSetting.Value;
                PricingDto.TotalAmount = offer.TotalAmount;
                PricingDto.IsGuesingPrice = true;
            }
            else { 
                var Calculate = await _commissionManager.CalculateAmountByDefault(Price, shippingRequest);
                ObjectMapper.Map(Calculate, PricingDto);
            }

            return PricingDto;


        }

        

        [RequiresFeature(AppFeatures.SendDirectRequest)]
        public async Task<PagedResultDto<TachyonDealerGetCarrirerDto>> GetAllCarriers(TachyonDealerGetCarrirerFilterInputDto Input)
        {
            var query =
                _tenantRepository.GetAll()
                .AsNoTracking()
                .Where(t => t.IsActive && t.Edition.DisplayName == TACHYONConsts.CarrierEdtionName)
                .WhereIf(!string.IsNullOrEmpty(Input.Filter), e => e.TenancyName.ToLower().Contains(Input.Filter.ToLower()))
                .Select(r => new TachyonDealerGetCarrirerDto
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .OrderBy(Input.Sorting ?? "id desc")
                .PageBy(Input);
            var result = await query.ToListAsync();
            result.ForEach(r =>
            {
                r.IsRequestSent = _CarrierDirectPricingRepository.FirstOrDefault(f => f.RequestId == Input.RequestId && f.CarrirerTenantId == r.Id) != null;
            });
            var totalCount = await query.CountAsync();
            return new PagedResultDto<TachyonDealerGetCarrirerDto>(
                totalCount,
                result
            );
        }
        #region Heleper
        private async Task<ShippingRequest> GetShippingRequestOnPrePriceStage(long requestId)
        {
            ShippingRequest shippingRequest = await _shippingRequestRepository.FirstOrDefaultAsync(e => e.Id == requestId && e.IsTachyonDeal && (e.Status == ShippingRequestStatus.PrePrice || e.Status == ShippingRequestStatus.NeedsAction) );
            if (shippingRequest == null) throw new UserFriendlyException(L("NoShippingRequest"));

            return shippingRequest;
        }


        #endregion
    }
}
