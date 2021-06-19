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
    }
}
