using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos;
namespace TACHYON.Shipping.ShippingRequests.TachyonDealer
{
    public class ShippingRequestsTachyonDealerAppService : TACHYONAppServiceBase, IShippingRequestsTachyonDealerAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        public ShippingRequestsTachyonDealerAppService(
            IRepository<ShippingRequest, long> shippingRequestRepository
            )
        {
            _shippingRequestRepository = shippingRequestRepository;
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
                if (Input.StartDate == null)
                {
                    Input.StartDate = Clock.Now.Date;
                }
                shippingRequest.BidEndDate = Input.EndDate;

                if (Input.StartDate.Value.Date == Clock.Now.Date) shippingRequest.BidStatus = ShippingRequestBidStatus.OnGoing;
            }
            else
            {
                throw new UserFriendlyException(L("NoShippingRequest"));
            }
        }


    }
}
