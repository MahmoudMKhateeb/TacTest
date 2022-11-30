using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.TmsPricePackages
{
    public class TmsPricePackageManager : TACHYONDomainServiceBase, ITmsPricePackageManager
    {
        private readonly IRepository<TmsPricePackage> _tmsPricePackageRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly PriceOfferManager _priceOfferManager;
        private readonly IShippingRequestDirectRequestAppService _directRequestAppService;

        public TmsPricePackageManager(
            IRepository<TmsPricePackage> tmsPricePackageRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            PriceOfferManager priceOfferManager, IShippingRequestDirectRequestAppService directRequestAppService)
        {
            _tmsPricePackageRepository = tmsPricePackageRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _priceOfferManager = priceOfferManager;
            _directRequestAppService = directRequestAppService;
        }

        public async Task SendOfferByPricePackage(int pricePackageId,long srId)
        {
            DisableTenancyFilters();
            
            var pricePackage = await _tmsPricePackageRepository.GetAll().SingleAsync(x=> x.Id == pricePackageId);

            if (pricePackage.DestinationTenantId is null)
                throw new UserFriendlyException(L("PricePackageMustHaveDestinationTenant"));
            
            var shippingRequest = await _shippingRequestRepository.GetAllIncluding(x=> x.ShippingRequestVases)
                .AsNoTracking().SingleAsync(x => x.Id == srId);

            if (pricePackage.ProposalId.HasValue)
            {
                var itemDetails = shippingRequest.ShippingRequestVases?
                    .Select(item => new PriceOfferDetailDto() { ItemId = item.Id, Price = 0 }).ToList();

                var priceOfferDto = new CreateOrEditPriceOfferInput()
                {
                    ShippingRequestId = srId, ItemPrice = pricePackage.Price, ItemDetails = itemDetails, 
                    CommissionType = pricePackage.CommissionType == PricePackageCommissionType.Percentage?
                        PriceOfferCommissionType.CommissionPercentage : PriceOfferCommissionType.CommissionValue,
                    CommissionPercentageOrAddValue = pricePackage.Commission
                };

                var offerId = await _priceOfferManager.CreateOrEdit(priceOfferDto);

                pricePackage.OfferId = offerId;
                pricePackage.Status = PricePackageOfferStatus.SentAndWaitingResponse;
            } else if (pricePackage.AppendixId.HasValue)
            {
                await _directRequestAppService.Create(new CreateShippingRequestDirectRequestInput()
                {
                    CarrierTenantId = pricePackage.DestinationTenantId.Value, ShippingRequestId = srId
                });

            }
            else throw new UserFriendlyException(L("PricePackageMustHaveAppendixOrProposal"));

        }

        public async Task AcceptOfferByPricePackage(TmsPricePackage pricePackage)
        {
            if (!pricePackage.OfferId.HasValue) throw new UserFriendlyException(L("ThereIsNoOfferInThisPricePackage"));
                
            await _priceOfferManager.AcceptOffer(pricePackage.OfferId.Value);

            pricePackage.Status = PricePackageOfferStatus.Confirmed;
        }

    }
}