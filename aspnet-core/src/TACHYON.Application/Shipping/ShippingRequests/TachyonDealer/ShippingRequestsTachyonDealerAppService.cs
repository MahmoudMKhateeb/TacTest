﻿using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.PricePackages;
using TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer
{
    public class ShippingRequestsTachyonDealerAppService : TACHYONAppServiceBase,
        IShippingRequestsTachyonDealerAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly IPricePackageManager _pricePackageManager;

        public ShippingRequestsTachyonDealerAppService(
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IPricePackageManager pricePackageManager)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _pricePackageManager = pricePackageManager;
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task StartBid(TachyonDealerBidDtoInupt Input)
        {
            DisableTenancyFilters();
            ShippingRequest shippingRequest = await _shippingRequestRepository
                .GetAll()
                .Include(x=>x.ShippingRequestDestinationCities)
                .FirstOrDefaultAsync(e =>
                e.Id == Input.Id && e.IsTachyonDeal &&
                !e.IsBid &&
                (e.Status == ShippingRequestStatus.PrePrice || e.Status == ShippingRequestStatus.NeedsAction));
            if (shippingRequest != null)
            {
                shippingRequest.IsBid = true;
                shippingRequest.BidStartDate = Input.StartDate;
                if (Input.StartDate == null)
                {
                    Input.StartDate = Clock.Now.Date;
                }

                shippingRequest.BidEndDate = Input.EndDate;

                if (Input.StartDate.Value.Date == Clock.Now.Date)
                    shippingRequest.BidStatus = ShippingRequestBidStatus.OnGoing;
            }
            else
            {
                throw new UserFriendlyException(L("NoShippingRequest"));
            }

            await _pricePackageManager.SendNotificationToCarriersWithTheSameTrucks(shippingRequest);
        }
    }
}