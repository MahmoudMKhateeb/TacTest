using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.PriceOffers.Dto;

namespace TACHYON.PriceOffers
{
    public interface IPriceOfferAppService : IApplicationService
    {
        Task<ListResultDto<GetShippingRequestForPriceOfferListDto>> GetAllShippingRequest(ShippingRequestForPriceOfferGetAllInput input);

       // Task GetAll(long id);
       // Task Accept(long id);
       // Task Reject(long id);
       //Task SendOffer(long id);

    }
}
