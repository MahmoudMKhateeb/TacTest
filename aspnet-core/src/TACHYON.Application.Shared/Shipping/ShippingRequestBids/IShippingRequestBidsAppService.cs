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
        Task<PagedResultDto<GetShippingRequestBidsForViewDto>> GetAllBidsByShippingRequestIdPaging(GetAllShippingRequestBidsInput input);
        Task CancelShippingRequestBid(StopShippingRequestBidInput input);
        Task<long> CreateOrEditShippingRequestBid(CreatOrEditShippingRequestBidDto input);
        Task AcceptBid(ShippingRequestBidInput input);
        Task<List<ViewCarrierBidsOutput>> GetAllCarrierBidsForView();
        Task CancelBidRequest(ShippingRequestBidInput input);
        Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> GetShipperbidsRequestDetailsForView(PagedAndSortedResultRequestDto input);
        Task<PagedResultDto<ViewShipperBidsReqDetailsOutputDto>> GetAllMarketPlaceSRForCarrier(PagedAndSortedResultRequestDto input, GetAllBidsInput input2);

    }
}
