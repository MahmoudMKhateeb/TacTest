﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;

namespace TACHYON.PriceOffers
{
    public interface IPriceOfferAppService : IApplicationService
    {

        Task<PagedResultDto<PriceOfferListDto>> GetAll(PriceOfferGetAllInput input);

        Task<ListResultDto<GetShippingRequestForPriceOfferListDto>> GetAllShippingRequest(ShippingRequestForPriceOfferGetAllInput input);

        GetShippingRequestSearchListDto GetAllListForSearch();
   
        Task<long> CreateOrEdit(CreateOrEditPriceOfferInput Input);
        Task<PriceOfferDto> GetPriceOfferForCreateOrEdit(long id, long? OfferId);
        Task<PriceOfferViewDto> GetPriceOfferForView(long OfferId);
        Task Delete(EntityDto Input);
        Task<GetShippingRequestForPricingOutput> GetShippingRequestForPricing(GetShippingRequestForPricingInput input);

         Task<PriceOfferStatus> Accept(long id);
         Task Reject(RejectPriceOfferInput input);
         Task Cancel(long id);
        Task CancelShipment(CancelShippingRequestInput input);

    }
}