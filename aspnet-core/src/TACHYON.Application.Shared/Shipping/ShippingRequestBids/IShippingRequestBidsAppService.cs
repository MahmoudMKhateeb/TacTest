using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;

namespace TACHYON.Shipping.ShippingRequestBids
{
    public interface IShippingRequestBidsAppService:IApplicationService
    {
        Task<PagedResultDto<GetShippingRequestBidsForViewDto>> GetAllBids(GetAllShippingRequestBidsInput input);
        Task CloseShippingRequestBid(StopShippingRequestBidInput input);
        Task<long> CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input);
        Task AcceptBid(ShippingRequestBidInput input);
        Task<List<ViewCarrierBidsOutput>> ViewAllCarrierBids();
        Task CancelBidRequest(ShippingRequestBidInput input);
        Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> GetShipperbidsRequestDetailsForView(PagedAndSortedResultRequestDto input);

    }
}
