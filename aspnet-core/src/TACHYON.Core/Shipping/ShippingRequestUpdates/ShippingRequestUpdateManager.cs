using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestUpdate;

namespace TACHYON.Shipping.ShippingRequestUpdates
{
    public class ShippingRequestUpdateManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestUpdate, Guid> _srUpdateRepository;
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IRepository<PriceOffer, long> _priceOfferRepository;

        public ShippingRequestUpdateManager(
            IRepository<ShippingRequestUpdate, Guid> srUpdateRepository,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<PriceOffer, long> priceOfferRepository)
        {
            _srUpdateRepository = srUpdateRepository;
            _shippingRequestRepository = shippingRequestRepository;
            _priceOfferRepository = priceOfferRepository;
        }
        
        public async Task Create(CreateSrUpdateInputDto input)
        {
            // Here We Check is the Shipping Request in pre-price stage (Status NeedAction)
            // And Has Offer not accepted Yet
            var isSrHasOffers = await _shippingRequestRepository.GetAll()
                .Where(x=> x.Id == input.ShippingRequestId && x.Status == ShippingRequestStatus.NeedsAction)
                .AnyAsync(sr =>  sr.TotalOffers > 0);
            
            if (!isSrHasOffers) return;

            var createdSrUpdates = await _priceOfferRepository.GetAll()
                .Where(x => x.ShippingRequestId == input.ShippingRequestId
                            && x.Status == PriceOfferStatus.New
                            && x.ParentId == null)
                .Select(x => new ShippingRequestUpdate()
                {
                    ShippingRequestId = input.ShippingRequestId,
                    EntityLogId = input.EntityLogId,
                    PriceOfferId = x.Id,
                    Status = ShippingRequestUpdateStatus.None
                }).ToListAsync();
            
            foreach (var requestUpdate in createdSrUpdates) 
                await _srUpdateRepository.InsertAsync(requestUpdate);
            
        }
        
        public async Task<bool> IsRequestHasBendingUpdates(long priceOfferId)
            => await _srUpdateRepository.GetAll().Where(x => x.Status == ShippingRequestUpdateStatus.None)
                .AnyAsync(x => x.PriceOfferId == priceOfferId);

    }
}