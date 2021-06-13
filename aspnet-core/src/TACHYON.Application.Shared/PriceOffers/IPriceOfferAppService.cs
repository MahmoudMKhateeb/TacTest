using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.PriceOffers
{
    public interface IPriceOfferAppService : IApplicationService
    {
        Task<ListResultDto<GetShippingRequestForPriceOfferListDto>> GetAllShippingRequest(ShippingRequestForPriceOfferGetAllInput input);
        Task<long> CreateOrEdit(CreateOrEditPriceOfferInput Input);
        Task<PriceOfferDto> GetPriceOfferForCreateOrEdit(long id);
        Task<PriceOfferViewDto> GetPriceOfferForView(long OfferId);
        Task Delete(EntityDto Input);
        Task<GetShippingRequestForPricingOutput> GetShippingRequestForPricing(GetShippingRequestForPricingInput input);

        // Task GetAll(long id);
        // Task Accept(long id);
        // Task Reject(long id);
        //Task SendOffer(long id);

    }
}
