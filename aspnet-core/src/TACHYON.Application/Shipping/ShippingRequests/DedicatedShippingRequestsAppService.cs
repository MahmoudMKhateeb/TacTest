using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Features;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;

namespace TACHYON.Shipping.ShippingRequests
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequests)]
    public class DedicatedShippingRequestsAppService: TACHYONAppServiceBase, IDedicatedShippingRequestsAppService
    {
        private readonly IRepository<ShippingRequest, long> _shippingRequestRepository;
        private readonly ShippingRequestManager _shippingRequestManager;
        private readonly IRepository<ShippingRequestDestinationCity> _shippingRequestDestinationCityRepository;
        private readonly ShippingRequestDirectRequestAppService _shippingRequestDirectRequestAppService;


        public DedicatedShippingRequestsAppService(IRepository<ShippingRequest, long> shippingRequestRepository,
            ShippingRequestManager shippingRequestManager,
            IRepository<ShippingRequestDestinationCity> shippingRequestDestinationCityRepository,
            ShippingRequestDirectRequestAppService shippingRequestDirectRequestAppService)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestManager = shippingRequestManager;
            _shippingRequestDestinationCityRepository = shippingRequestDestinationCityRepository;
            _shippingRequestDirectRequestAppService = shippingRequestDirectRequestAppService;
        }

        public async Task<long> CreateOrEditStep1(CreateOrEditDedicatedStep1Dto input)
        {
            await _shippingRequestManager.ValidateShippingRequestStep1(input);
            ValidateDestinationCities(input);
            await _shippingRequestManager.OthersNameValidation(input);

            if (input.Id == null)
            {
                return await CreateStep1(input);
            }
            else
            {
                return await UpdateStep1(input);
            }
        }

        public async Task<CreateOrEditDedicatedStep1Dto> GetStep1ForEdit(long id)
        {
            var shippingRequest=await _shippingRequestManager.GetDraftedShippingRequest(id);
                return ObjectMapper.Map<CreateOrEditDedicatedStep1Dto>(shippingRequest);
        }

        public async Task EditStep2(EditDedicatedStep2Dto input)
        {
            ShippingRequest shippingRequest = await _shippingRequestManager.GetDraftedShippingRequest(input.Id);

            //delete vases
            await _shippingRequestManager.EditVasStep(shippingRequest, input);

            if (shippingRequest.DraftStep < 2)
            {
                shippingRequest.DraftStep = 2;
            }

            ObjectMapper.Map(input, shippingRequest);
        }

        public async Task<EditDedicatedStep2Dto> GetStep2ForEdit(long id)
        {
            var shippingRequest = await _shippingRequestManager.GetDraftedShippingRequest(id);
            return ObjectMapper.Map<EditDedicatedStep2Dto>(shippingRequest);
        }


        public async Task PublishDedicatedShippingRequest(long id)
        {
            ShippingRequest shippingRequest = await _shippingRequestManager.GetDraftedShippingRequest(id);

            if (shippingRequest.DraftStep < 2)
            {
                throw new UserFriendlyException(L("YouMustCompleteWizardStepsFirst"));
            }
            await _shippingRequestManager.PublishShippingRequestManager(shippingRequest);
            if (!shippingRequest.IsSaas())
            {
                await SendtoCarrierIfShippingRequestIsDirectRequest(shippingRequest);
            }

        }


        #region Helper
        private async Task<long> CreateStep1(CreateOrEditDedicatedStep1Dto input)
        {
            ShippingRequest shippingRequest = ObjectMapper.Map<ShippingRequest>(input);
            shippingRequest.ShippingRequestFlag = ShippingRequestFlag.Dedicated;

            await _shippingRequestManager.CreateStep1Manager(shippingRequest, input);
            var SR= await _shippingRequestRepository.InsertAndGetIdAsync(shippingRequest);

            //add new or remove destinaton cities
            await AddOrRemoveDestinationCities(input, shippingRequest);
            return SR;
        }


        private async Task<long> UpdateStep1(CreateOrEditDedicatedStep1Dto input)
        {
            var shippingRequest = await _shippingRequestManager.GetDraftedShippingRequest(input.Id.Value);

            ObjectMapper.Map(input, shippingRequest);
            await AddOrRemoveDestinationCities(input, shippingRequest);
            return shippingRequest.Id;
        }


        private async Task AddOrRemoveDestinationCities(CreateOrEditDedicatedStep1Dto input, ShippingRequest shippingRequest)
        {
            foreach (var destinationCity in input.ShippingRequestDestinationCities)
            {
                destinationCity.ShippingRequestId = shippingRequest.Id;
                var exists = await _shippingRequestDestinationCityRepository.GetAll().AnyAsync(c => c.CityId == destinationCity.CityId &&
                c.ShippingRequestId == destinationCity.ShippingRequestId);

                if (!exists)
                {
                    if (shippingRequest.ShippingRequestDestinationCities == null) shippingRequest.ShippingRequestDestinationCities = new List<ShippingRequestDestinationCity>();
                    shippingRequest.ShippingRequestDestinationCities.Add(ObjectMapper.Map<ShippingRequestDestinationCity>(destinationCity));
                }
            }
            //remove uncoming destination cities
            foreach (var destinationCity in shippingRequest.ShippingRequestDestinationCities)
            {
                if (!input.ShippingRequestDestinationCities.Any(x => x.CityId == destinationCity.CityId))
                {
                    await _shippingRequestDestinationCityRepository.DeleteAsync(destinationCity);
                }
            }
        }

        private void ValidateDestinationCities(CreateOrEditDedicatedStep1Dto input)
        {
            if (input.ShippingTypeId == 1 && input.ShippingRequestDestinationCities.Count > 1)
            {
                throw new UserFriendlyException(L("OneDestinationCityAllowed"));
            }
        }

        private async Task SendtoCarrierIfShippingRequestIsDirectRequest(ShippingRequest shippingRequest)
        {
            if (shippingRequest.IsDirectRequest && shippingRequest.CarrierTenantIdForDirectRequest.HasValue)
            {
                var directRequestInput = new CreateShippingRequestDirectRequestInput();
                directRequestInput.CarrierTenantId = shippingRequest.CarrierTenantIdForDirectRequest.Value;
                directRequestInput.ShippingRequestId = shippingRequest.Id;
                await _shippingRequestDirectRequestAppService.Create(directRequestInput);
            }
        }
        #endregion
    }
}
