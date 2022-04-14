using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Notifications;
using TACHYON.PriceOffers;
using TACHYON.PriceOffers.Dto;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.ShippingRequestVases;

namespace TACHYON.Shipping.SrPostPriceUpdates
{
    public class SrPostPriceUpdateManager : TACHYONDomainServiceBase
    {

        private readonly IRepository<SrPostPriceUpdate, long> _updateRepository;
        private readonly IRepository<ShippingRequest, long> _requestRepository;
        private readonly IAppNotifier _notifier;
        private readonly PriceOfferManager _offerManager;
        private readonly IRepository<ShippingRequestVas,long> _shippingRequestVasRepository;

        public SrPostPriceUpdateManager(
            IRepository<SrPostPriceUpdate, long> updateRepository,
            IAppNotifier notifier, PriceOfferManager offerManager,
            IRepository<ShippingRequest, long> requestRepository,
            IRepository<ShippingRequestVas, long> shippingRequestVasRepository)
        {
            _updateRepository = updateRepository;
            _notifier = notifier;
            _offerManager = offerManager;
            _requestRepository = requestRepository;
            _shippingRequestVasRepository = shippingRequestVasRepository;
        }


        public async Task Create(ShippingRequest request, CreateOrEditShippingRequestDto input,long? currentUserId)
        {
            var updateChanges = JsonConvert.SerializeObject(input,
                new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            
            var postPriceUpdate = new SrPostPriceUpdate()
            {
                Action = SrPostPriceUpdateAction.Pending, 
                UpdateChanges = updateChanges,
                ShippingRequestId = request.Id, 
                CreatorUserId = currentUserId, // I can't use IAbpSession in Domain Layer (Not Recommended)
            };

            await _updateRepository.InsertAsync(postPriceUpdate);

            await SendNotificationWhenUpdateOnPostPrice(request);
        }
        
        
        
        public async Task TakeAction(CreateSrPostPriceUpdateActionDto input)
        {
            var postPriceUpdate = await _updateRepository.SingleAsync(x=> x.Id == input.Id);

            if (postPriceUpdate.Action != SrPostPriceUpdateAction.Pending)
                throw new UserFriendlyException(L("YouAlreadyTakeAnActionForThisUpdate"));
            
            switch (input.Action)
            {
                case SrPostPriceUpdateAction.Accept:
                    await Accept(input, postPriceUpdate);
                    break;
                case SrPostPriceUpdateAction.ChangePrice:
                    await ChangePrice(input,postPriceUpdate);
                    break;
                case SrPostPriceUpdateAction.Reject:
                    await Reject(input, postPriceUpdate);
                    break;
                default: return;
            }
        }

        
        public async Task TakeOfferActionByShipper(CreateSrPostPriceUpdateOfferActionDto input)
        {
            var postPriceUpdate = await _updateRepository.GetAllIncluding(x=> x.PriceOffer)
                .SingleAsync(x=> x.Id == input.Id);

            ValidateSrUpdateForChangePrice(postPriceUpdate);

            switch (input.OfferAction)
            {
                case SrPostPriceUpdateOfferAction.Accept:

                    await AcceptOfferAndApplyChange(postPriceUpdate);
                    
                    break;
                case SrPostPriceUpdateOfferAction.Reject:

                    await RejectOffer(postPriceUpdate, input.OfferRejectionReason);

                    break;
                default: throw new UserFriendlyException(L("YouMustAcceptOrRejectOffer"));
            }

        }

       
        

      

        #region Helpers

        #region UpdateActions

        private async Task ChangePrice(CreateSrPostPriceUpdateActionDto input,SrPostPriceUpdate update)
        {
            if (input.Action != SrPostPriceUpdateAction.ChangePrice || input.Offer == null) return;

            input.Offer.IsPostPrice = true;
            input.Offer.ShippingRequestId = update.ShippingRequestId;
            var offerId = await _offerManager.CreateOrEdit(input.Offer);

            update.PriceOfferId = offerId;
            update.IsApplied = false; // Not Applied Yet
            update.Action = input.Action;
            update.OfferStatus = SrPostPriceUpdateOfferStatus.Pending;

            await SendNotificationWhenRequestChangePrice(update.ShippingRequestId);
        }


        private async Task Reject(CreateSrPostPriceUpdateActionDto input,SrPostPriceUpdate update)
        {
            if (input.Action != SrPostPriceUpdateAction.Reject || input.RejectionReason.IsNullOrEmpty()) return;
            
            update.IsApplied = false;
            update.RejectionReason = input.RejectionReason;
            update.Action = input.Action;
             
            await SendNotificationWhenCreateUpdateAction(update);
        }

        private async Task Accept(CreateSrPostPriceUpdateActionDto input, SrPostPriceUpdate update)
        {
            if (input.Action != SrPostPriceUpdateAction.Accept) return;

            await ApplyShippingRequestChanges(update);

            update.IsApplied = true;
            update.Action = input.Action;
            
            await SendNotificationWhenCreateUpdateAction(update);
        }

        #endregion

        #region OfferActions

        private async Task AcceptOfferAndApplyChange(SrPostPriceUpdate update)
        {
            if (!update.PriceOfferId.HasValue) return;

            await _offerManager.AcceptOffer(update.PriceOfferId.Value);

            await ApplyShippingRequestChanges(update);
            
            update.IsApplied = true;
            update.OfferStatus = SrPostPriceUpdateOfferStatus.Accepted;
        }

        private async Task RejectOffer(SrPostPriceUpdate update,string rejectionReason)
        {

            if (!update.PriceOfferId.HasValue) 
                throw new UserFriendlyException(L("OfferNotFound"));

            await _offerManager.RejectPostPriceOffer(update.PriceOfferId.Value,rejectionReason);
            
            update.IsApplied = false;
            update.OfferStatus = SrPostPriceUpdateOfferStatus.Rejected;
        }

        #endregion
        
       

        private async Task ApplyShippingRequestChanges(SrPostPriceUpdate update )
        {
            var updatedSrDto = JsonConvert.DeserializeObject<CreateOrEditShippingRequestDto>(update.UpdateChanges);

            CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant);
            
            var shippingRequest = await _requestRepository.GetAllIncluding(x => x.ShippingRequestVases)
                .SingleOrDefaultAsync(x => x.Id == update.ShippingRequestId);
            
            ObjectMapper.Map(updatedSrDto,shippingRequest);


            // check for deleted vases

            foreach (var vas in shippingRequest.ShippingRequestVases)
            
                if (updatedSrDto.ShippingRequestVasList.All(x => x.Id != vas.Id))
                    await _shippingRequestVasRepository.DeleteAsync(vas);
                
            

            // check for added vases 

            foreach (var vas in updatedSrDto.ShippingRequestVasList)
            {
                if (shippingRequest.ShippingRequestVases.All(x => x.Id != vas.Id))
                {
                    var createdVas = ObjectMapper.Map<ShippingRequestVas>(vas);
                    await _shippingRequestVasRepository.InsertAsync(createdVas);
                }
            }
        }
        
        private void ValidateSrUpdateForChangePrice(SrPostPriceUpdate update)
        {
            if (update.Action != SrPostPriceUpdateAction.ChangePrice)
                throw new UserFriendlyException(L("TheUpdateActionMustBeChangePrice"));
            if (!update.PriceOfferId.HasValue)
                throw new UserFriendlyException(L("TheUpdateHasNotPriceOffer"));
        }

        #endregion
        
        #region Notification

        private async Task SendNotificationWhenUpdateOnPostPrice(ShippingRequest request)
        {

            if (request.Status != ShippingRequestStatus.PostPrice || !request.CarrierTenantId.HasValue) return;
            
            await _notifier.NotifyCarrierWhenPostPriceSrUpdated(request.Id, request.ReferenceNumber,request.CarrierTenantId.Value);

        }
        
        private async Task SendNotificationWhenCreateUpdateAction(SrPostPriceUpdate update)
        {
            
            DisableTenancyFilters();
           var request = await _requestRepository.GetAll()
                .AsNoTracking()
                .Select(x=> new {x.Id,x.ReferenceNumber,x.TenantId})
                .SingleAsync(x => x.Id == update.ShippingRequestId);

            await _notifier.NotifyShipperForPostPriceSrUpdateAction(request.Id,request.TenantId,request.ReferenceNumber,update.Action);
        }
        
        private async Task SendNotificationWhenRequestChangePrice(long requestId)
        {
            var request = await _requestRepository.GetAll()
                .AsNoTracking()
                .Select(x=> new {x.Id,x.ReferenceNumber,x.TenantId})
                .SingleAsync(x => x.Id == requestId);

            await _notifier.NotifyShipperWhenRequestChangePrice(request.Id,request.TenantId,request.ReferenceNumber);
        }

        #endregion
    }
}