using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Validation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequestUpdate;

namespace TACHYON.Shipping.ShippingRequestUpdates
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestUpdates)]
    public class ShippingRequestUpdateAppService : TACHYONAppServiceBase
    {
        private readonly IRepository<ShippingRequestUpdate,Guid> _srUpdateRepository;
        private readonly PriceOfferManager _offerManager;

        public ShippingRequestUpdateAppService(
            IRepository<ShippingRequestUpdate, Guid> srUpdateRepository,
            PriceOfferManager offerManager)
        {
            _srUpdateRepository = srUpdateRepository;
            _offerManager = offerManager;
        }


        public async Task<PagedResultDto<ShippingRequestUpdateListDto>> GetAll(GetAllSrUpdateInputDto input)
        {
            var srUpdates = _srUpdateRepository.GetAll()
                .Where(x => x.PriceOfferId == input.PriceOfferId
                            && x.ShippingRequestId == input.ShippingRequestId)
                .AsNoTracking().OrderBy(input.Sorting ?? "CreationTime asc");

            var pageResult = await srUpdates.PageBy(input).ToListAsync();

            var totalCount = await srUpdates.CountAsync();

            return new PagedResultDto<ShippingRequestUpdateListDto>()
            {
                Items = ObjectMapper.Map<List<ShippingRequestUpdateListDto>>(pageResult), TotalCount = totalCount
            };
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestUpdates_TakeAction)]
        public async Task<ShippingRequestUpdateStatus> TakeAction(CreateSrUpdateActionInputDto input)
        {
            var srUpdate = await GetWithValidateSrUpdate(input);

            // I can't use s
            switch (input.Status)
            {
                case ShippingRequestUpdateStatus.None:
                    throw new AbpValidationException(L("YouMustTakeAnAction"));
                case ShippingRequestUpdateStatus.Repriced:
                    return await RepriceOffer(srUpdate, input.PriceOfferInput);
                case ShippingRequestUpdateStatus.KeepSamePrice:
                    return KeepSamePrice(srUpdate);
                case ShippingRequestUpdateStatus.Dismissed:
                    return await DismissOffer(srUpdate);
                default:
                    throw new AbpValidationException();
            }
        }

        #region Helpers

        private async Task<ShippingRequestUpdateStatus> DismissOffer(ShippingRequestUpdate srUpdate)
        {
            srUpdate.Status = ShippingRequestUpdateStatus.Dismissed;
            await _offerManager.Delete(new EntityDto<long>(srUpdate.PriceOfferId));
            return srUpdate.Status;
        }
        
        private static ShippingRequestUpdateStatus KeepSamePrice(ShippingRequestUpdate srUpdate)
        {
            srUpdate.Status = ShippingRequestUpdateStatus.KeepSamePrice;
            return srUpdate.Status;
        }

        private async Task<ShippingRequestUpdateStatus> RepriceOffer(ShippingRequestUpdate srUpdate, CreateOrEditPriceOfferInput input)
        {
            srUpdate.Status = ShippingRequestUpdateStatus.Repriced;
            // Here we need to delete the old offer and create a new one
            await _offerManager.Delete(new EntityDto<long>(srUpdate.PriceOfferId));
            var createdOfferId = await _offerManager.CreateOrEdit(input);
            srUpdate.OldPriceOfferId = srUpdate.PriceOfferId;
            srUpdate.PriceOfferId = createdOfferId;
            
            return srUpdate.Status;
        }

        private async Task<ShippingRequestUpdate> GetWithValidateSrUpdate(CreateSrUpdateActionInputDto input)
        {
            var shippingRequestUpdate = await _srUpdateRepository.FirstOrDefaultAsync(input.Id);

            if (shippingRequestUpdate == null)
                throw new EntityNotFoundException(L("ShippingRequestUpdateNotFound"));

            if (shippingRequestUpdate.Status != ShippingRequestUpdateStatus.None)
                throw new AbpValidationException(L("YouAlreadyTakeAnActionOfThisUpdate"));
            return shippingRequestUpdate;
        }
        #endregion
    }
}