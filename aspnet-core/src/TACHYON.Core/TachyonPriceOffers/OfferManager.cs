using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.TachyonPriceOffers
{
    public class OfferManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<TachyonPriceOffer> _tachyonPriceOfferRepository;

        public OfferManager(IRepository<TachyonPriceOffer> tachyonPriceOfferRepository)
        {
            _tachyonPriceOfferRepository = tachyonPriceOfferRepository;
        }

        public async Task<TachyonPriceOffer> GetAcceptedAndWaitingForCarrierOffer(long shippingRequestId)
        {
            var item=await _tachyonPriceOfferRepository.GetAll()
                .Where(e => e.ShippingRequestId == shippingRequestId)
                .Where(x => x.OfferStatus == OfferStatus.AcceptedAndWaitingForCarrier)
                .FirstOrDefaultAsync();
            return item;
        }
    }
}
