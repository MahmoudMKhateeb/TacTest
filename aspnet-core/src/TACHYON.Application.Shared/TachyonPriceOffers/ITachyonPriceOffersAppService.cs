using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.TachyonPriceOffers.dtos;

namespace TACHYON.TachyonPriceOffers
{
    public interface ITachyonPriceOffersAppService : IApplicationService
    {
        Task<PagedResultDto<GetAllTachyonPriceOfferOutput>>
            GetAllTachyonPriceOffers(GetAllTachyonPriceOfferInput input);

        Task CreateOrEditTachyonPriceOffer(CreateOrEditTachyonPriceOfferDto input);

        //  Task<GetTachyonPriceOfferForEditOutput> GetTachyonPriceForEdit(EntityDto entity);
        Task<GetTachyonPriceOfferForViewOutput> GetTachyonPriceOfferForView(EntityDto entity);
        Task Delete(EntityDto entity);
        Task AcceptOrRejectOfferByShipper(AcceptOrRejectOfferByShipperInput input);
    }
}