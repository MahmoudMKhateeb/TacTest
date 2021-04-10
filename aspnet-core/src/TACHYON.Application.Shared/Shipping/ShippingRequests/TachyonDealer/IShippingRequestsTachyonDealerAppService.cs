using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.TachyonDealer.Dtos;

namespace TACHYON.Shipping.ShippingRequests.TachyonDealer
{
    public interface IShippingRequestsTachyonDealerAppService: IApplicationService
    {
        Task StartBid(TachyonDealerBidDtoInupt Input);
        //Task SendOfferToShipper(TachyonDealerDirectOfferToShipperInputDto Input);
        Task SendDriectRequestForCarrier(TachyonDealerCreateDirectOfferToCarrirerInuptDto Input);
        Task<PagedResultDto<ShippingRequestsCarrierDirectPricingListDto>> GetDriectRequestForAllCarriers(TachyonDealerCreateDirectOfferToCarrirerFilterInput Input);
        Task<PagedResultDto<TachyonDealerGetCarrirerDto>> GetAllCarriers(TachyonDealerGetCarrirerFilterInputDto Input);

        Task CarrierSetPriceForDirectRequest(CarrirSetPriceForDirectRequestDto Input);
        Task<ShippingRequestAmountDto> GetCarrierPricing(GetCarrierPricingInputDto Input);
    }
}
