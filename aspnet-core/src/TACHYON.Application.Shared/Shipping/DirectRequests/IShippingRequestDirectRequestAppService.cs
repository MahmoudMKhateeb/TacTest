using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.DirectRequests.Dto;

namespace TACHYON.Shipping.DirectRequests
{
    public  interface IShippingRequestDirectRequestAppService: IApplicationService
    {
        Task<PagedResultDto<ShippingRequestDirectRequestListDto>> GetAll(ShippingRequestDirectRequestGetAllInput input);

        Task<PagedResultDto<ShippingRequestDirectRequestGetCarrirerListDto>> GetAllCarriers(ShippingRequestDirectRequestGetAllCarrirerInput input);

        Task Create(CreateShippingRequestDirectRequestInput input);
        Task Delete(EntityDto<long> input);

        Task Reject(RejectShippingRequestDirectRequestInput input);
        Task CreateOrEditOffer(CreateOrEditPriceOfferInput input);
        
    }
}
