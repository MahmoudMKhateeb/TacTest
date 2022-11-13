using Abp.Application.Services;
using System.Threading.Tasks;
using TACHYON.PriceOffers.Dto;

namespace TACHYON.PriceOffers
{
    public interface IActorsPriceOffersAppService : IApplicationService
    {
        Task<CreateOrEditSrActorShipperPriceInput> GetActorShipperPriceForEdit(long shippingRequestId);
        
        Task<CreateOrEditSrActorCarrierPriceInput> GetActorCarrierPriceForEdit(long shippingRequestId);

        Task CreateOrEditActorShipperPrice(CreateOrEditSrActorShipperPriceInput input);

        Task CreateOrEditActorCarrierPrice(CreateOrEditSrActorCarrierPriceInput input);
    }
}