using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.PriceOffers;
using TACHYON.Rating;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.DirectRequests
{
    public class ShippingRequestDirectRequestAppService : TACHYONAppServiceBase, IShippingRequestDirectRequestAppService
    {
        private readonly IRepository<TenantCarrier, long> _tenantCarrierRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<ShippingRequestDirectRequest, long> _shippingRequestDirectRequestRepository;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly IAppNotifier _appNotifier;
        private readonly PriceOfferManager _priceOfferManager;


        public ShippingRequestDirectRequestAppService(IRepository<TenantCarrier, long> tenantCarrierRepository, IRepository<Tenant> tenantRepository, IRepository<ShippingRequestDirectRequest, long> shippingRequestDirectRequestRepository, ShippingRequestManager shippingRequestManager, IAppNotifier appNotifier, PriceOfferManager priceOfferManager)
        {
            _tenantCarrierRepository = tenantCarrierRepository;
            _tenantRepository = tenantRepository;
            _shippingRequestDirectRequestRepository = shippingRequestDirectRequestRepository;
            _shippingRequestManager = shippingRequestManager;
            _appNotifier = appNotifier;
            _priceOfferManager = priceOfferManager;
        }
        [RequiresFeature(AppFeatures.SendDirectRequest)]
        public async Task<PagedResultDto<ShippingRequestDirectRequestListDto>> GetAll(ShippingRequestDirectRequestGetAllInput input)
        {
            var query = _shippingRequestDirectRequestRepository
                .GetAll()
                .Include(x => x.Carrier)
                .Where(x => x.ShippingRequestId == input.ShippingRequestId)
                .OrderBy(input.Sorting ?? "id desc");
            var Result = await query.PageBy(input).ToListAsync();
            var totalCount = await query.CountAsync();

            var list = ObjectMapper.Map<List<ShippingRequestDirectRequestListDto>>(Result);

            return new PagedResultDto<ShippingRequestDirectRequestListDto>(totalCount, list);
        }
        [RequiresFeature(AppFeatures.SendDirectRequest)]
        public async Task<PagedResultDto<ShippingRequestDirectRequestGetCarrirerListDto>> GetAllCarriers(ShippingRequestDirectRequestGetAllCarrirerInput input)
        {
            IfCanAccessService();
            IQueryable<ShippingRequestDirectRequestGetCarrirerListDto> query = GetCarriers(input);

            var Result = await query.PageBy(input).ToListAsync();

            Result.ForEach(r =>
            {
                r.IsRequestSent = _shippingRequestDirectRequestRepository.FirstOrDefault(f => f.ShippingRequestId == input.ShippingRequestId && f.CarrierTenantId == r.Id) != null;
            });
            var totalCount = await query.CountAsync();
            return new PagedResultDto<ShippingRequestDirectRequestGetCarrirerListDto>(
                totalCount,
                Result
            );
        }
        [RequiresFeature(AppFeatures.SendDirectRequest)]
        public async Task Create(CreateShippingRequestDirectRequestInput input)
        {
            IfCanAccessService();
            await CheckCanAddDriectRequestToCarrirer(input);
            var id = await _shippingRequestDirectRequestRepository.InsertAndGetIdAsync(ObjectMapper.Map<ShippingRequestDirectRequest>(input));

            if (input.BidNormalPricePackage != null)
            {
                await _appNotifier.NotfiyCarrierWhenReceiveBidPricePackage(input.CarrierTenantId, GetCurrentTenant().Name, input.BidNormalPricePackage.PricePackageId, id);
            }
            else
            {
                await _appNotifier.SendDriectRequest(GetCurrentTenant().Name, input.CarrierTenantId, id);
            }
        }

        [RequiresFeature(AppFeatures.SendDirectRequest)]
        public async Task Delete(EntityDto<long> input)
        {
            IfCanAccessService();
            var directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == input.Id && (x.Status == ShippingRequestDirectRequestStatus.New || x.Status == ShippingRequestDirectRequestStatus.Declined));
            if (directRequest == null) throw new UserFriendlyException(L("YouCanDeleteDirectRequestOnlyWhenTheStatusIsNew"));
            await _shippingRequestDirectRequestRepository.DeleteAsync(directRequest);
        }




        [RequiresFeature(AppFeatures.SendDirectRequest)]
        public async Task Reject(RejectShippingRequestDirectRequestInput input)
        {
            IfCanAccessService();
            var directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == input.Id && x.Status == ShippingRequestDirectRequestStatus.Response);
            if (directRequest == null) throw new UserFriendlyException(L("YouCanRejectDirectRequestOnlyWhenTheStatusIsResponse"));
            directRequest.Status = ShippingRequestDirectRequestStatus.Rejected;
            directRequest.RejetcReason = input.Reason;
            DisableTenancyFilters();
            await _appNotifier.RejectedOffer(await _priceOfferManager.GetOfferBySource(directRequest.Id, PriceOfferChannel.DirectRequest), GetCurrentTenant().Name);
        }

        [RequiresFeature(AppFeatures.Carrier)]
        public async Task Decline(long id)
        {
            DisableTenancyFilters();
            var directRequest = await _shippingRequestDirectRequestRepository.FirstOrDefaultAsync(x => x.Id == id && (x.Status == ShippingRequestDirectRequestStatus.New) && x.CarrierTenantId == AbpSession.TenantId.Value);
            if (directRequest == null) throw new UserFriendlyException(L("YouCanDeclineDirectRequestOnlyWhenTheStatusIsNew"));
            directRequest.Status = ShippingRequestDirectRequestStatus.Declined;
            await _appNotifier.DeclineDriectRequest(GetCurrentTenant().Name, directRequest.TenantId, id);
        }
        #region Heleper
        /// <summary>
        /// Get list of carrier can send direct request
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IQueryable<ShippingRequestDirectRequestGetCarrirerListDto> GetCarriers(ShippingRequestDirectRequestGetAllCarrirerInput input)
        {
            IQueryable<ShippingRequestDirectRequestGetCarrirerListDto> query;
            if (IsEnabled(AppFeatures.TachyonDealer))
            {
                query = _tenantRepository
                                .GetAll()
                                .AsNoTracking()
                                .Where(t => t.IsActive && t.Edition.DisplayName == TACHYONConsts.CarrierEdtionName)
                                .WhereIf(!string.IsNullOrEmpty(input.Filter),
                                        e => e.TenancyName.ToLower().Contains(input.Filter.ToLower()) ||
                                             e.Name.ToLower().Contains(input.Filter.ToLower()) ||
                                             e.companyName.ToLower().Contains(input.Filter.ToLower()))
                                .OrderBy(input.Sorting ?? "id desc")
                                .Select(r => new ShippingRequestDirectRequestGetCarrirerListDto
                                {
                                    Id = r.Id,
                                    Name = r.Name,
                                    CarrierRate = r.Rate,
                                    CarrierRateNumber = r.RateNumber
                                });
            }
            else if (IsEnabled(AppFeatures.Shipper))
            {
                query = _tenantCarrierRepository
                                .GetAll()
                                .AsNoTracking()
                                .Where(t => t.CarrierShipper.IsActive)
                                .WhereIf(!string.IsNullOrEmpty(input.Filter),
                                e => e.CarrierShipper.TenancyName.ToLower().Contains(input.Filter.ToLower()) ||
                                     e.CarrierShipper.Name.ToLower().Contains(input.Filter.ToLower()) ||
                                     e.CarrierShipper.companyName.ToLower().Contains(input.Filter.ToLower())
                                )

                                .OrderBy(input.Sorting ?? "id desc")
                                .Select(r => new ShippingRequestDirectRequestGetCarrirerListDto
                                {
                                    Id = r.CarrierTenantId,
                                    Name = r.CarrierShipper.Name,
                                    CarrierRate = r.CarrierShipper.Rate,
                                    CarrierRateNumber = r.CarrierShipper.RateNumber
                                });
            }
            else
            {
                throw new UserFriendlyException(L("NoRecordsFound"));
            }
            return query;
        }

        private async Task CheckCanAddDriectRequestToCarrirer(CreateShippingRequestDirectRequestInput input)
        {
            if (await _shippingRequestDirectRequestRepository.GetAll().AnyAsync(x => x.CarrierTenantId == input.CarrierTenantId && x.ShippingRequestId == input.ShippingRequestId)) throw new UserFriendlyException(L("TheCarrierHaveAlreadyDirectRequestSendBefore"));
            if (IsEnabled(AppFeatures.Shipper))
            {
                if (!await _tenantCarrierRepository.GetAll().AnyAsync(x => x.CarrierTenantId == input.CarrierTenantId)) throw new UserFriendlyException(L("TheCarrirerSelectedIsNotInYourList"));
            }
            DisableTenancyFilters();
            if (await _shippingRequestManager.GetShippingRequestWhenNormalStatus(input.ShippingRequestId) == null) throw new UserFriendlyException(L("TheShippingRequestNotFound"));

        }

        private void IfCanAccessService()
        {
            if (!IsEnabled(AppFeatures.Shipper) && !IsEnabled(AppFeatures.TachyonDealer)) throw new UserFriendlyException(L("YouDoNoHaveAccess"));
        }








        #endregion



    }
}