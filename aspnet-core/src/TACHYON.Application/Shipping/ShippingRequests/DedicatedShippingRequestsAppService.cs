using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Configuration;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Shipping.Dedicated;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
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
        private readonly IRepository<DedicatedShippingRequestTruck, long> _dedicatedShippingRequestTruckRepository;
        private readonly IRepository<DedicatedShippingRequestDriver, long> _dedicatedShippingRequestDriverRepository;
        private readonly ISettingManager _settingManager;

        public DedicatedShippingRequestsAppService(IRepository<ShippingRequest, long> shippingRequestRepository,
            ShippingRequestManager shippingRequestManager,
            IRepository<ShippingRequestDestinationCity> shippingRequestDestinationCityRepository,
            ShippingRequestDirectRequestAppService shippingRequestDirectRequestAppService,
            IRepository<DedicatedShippingRequestTruck, long> dedicatedShippingRequestTruckRepository,
            IRepository<DedicatedShippingRequestDriver, long> dedicatedShippingRequestDriverRepository,
            ISettingManager settingManager)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestManager = shippingRequestManager;
            _shippingRequestDestinationCityRepository = shippingRequestDestinationCityRepository;
            _shippingRequestDirectRequestAppService = shippingRequestDirectRequestAppService;
            _dedicatedShippingRequestTruckRepository = dedicatedShippingRequestTruckRepository;
            _dedicatedShippingRequestDriverRepository = dedicatedShippingRequestDriverRepository;
            _settingManager = settingManager;
        }

        #region Wizard
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<LoadResult> GetAllDedicatedTrucks(GetAllDedicatedTrucksInput input)
        {
            DisableTenancyFilters();
            var query = _dedicatedShippingRequestTruckRepository.GetAll()
                .WhereIf(input.ShippingRequestId != null, x => x.ShippingRequestId == input.ShippingRequestId)
                .ProjectTo<DedicatedShippingRequestTrucksDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();

            return await LoadResultAsync(query, input.Filter);
            
        }

        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<LoadResult> GetAllDedicatedDrivers(GetAllDedicatedDriversInput input)
        {
            DisableTenancyFilters();
            var query = _dedicatedShippingRequestDriverRepository.GetAll()
                .WhereIf(await IsShipper(), x => x.ShippingRequest.TenantId == AbpSession.TenantId)
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(input.ShippingRequestId != null, x => x.ShippingRequestId == input.ShippingRequestId)
                .ProjectTo<DedicatedShippingRequestDriversDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();

            return await LoadResultAsync(query, input.Filter);

        }

        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
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

        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<CreateOrEditDedicatedStep1Dto> GetStep1ForEdit(long id)
        {
            var shippingRequest=await GetDraftedDedicatedShippingRequestForStep1(id);
                return ObjectMapper.Map<CreateOrEditDedicatedStep1Dto>(shippingRequest);
        }
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task EditStep2(EditDedicatedStep2Dto input)
        {
            ShippingRequest shippingRequest = await GetDraftedDedicatedShippingRequestForStep2(input.Id);
            if (shippingRequest == null) throw new UserFriendlyException(L("ShippingRequestNotFound"));
            //delete vases
            await _shippingRequestManager.EditVasStep(shippingRequest, input);

            if (shippingRequest.DraftStep < 2)
            {
                shippingRequest.DraftStep = 2;
            }

            ObjectMapper.Map(input, shippingRequest);
        }
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<EditDedicatedStep2Dto> GetStep2ForEdit(long id)
        {
            var shippingRequest = await GetDraftedDedicatedShippingRequestForStep2(id);
            return ObjectMapper.Map<EditDedicatedStep2Dto>(shippingRequest);
        }

        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task PublishDedicatedShippingRequest(long id)
        {
            ShippingRequest shippingRequest = await _shippingRequestManager.GetDraftedShippingRequest(id);
            if (shippingRequest == null) throw new UserFriendlyException(L("ShippingRequestNotFound"));
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

        #endregion

        #region Assign trucks
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.CarrierAsASaas)]
        public async Task AssignDedicatedTrucksAndDrivers(AssignDedicatedTrucksAndDriversInput input)
        {
            var shippingRequest = await _shippingRequestManager.GetShippingRequestForAssign(input.ShippingRequestId);

            if (shippingRequest is null) throw new UserFriendlyException(L("ShippingRequestNotFound"));

            await ValidateTrucksAndDrivers(input, shippingRequest);

            var status = shippingRequest.RentalStartDate.Value.Date <= Clock.Now.Date
                ? WorkingStatus.Busy
                : WorkingStatus.Active;

            //Add trucks
            var trucksList = new List<DedicatedShippingRequestTruck>();
            foreach (var truck in input.TrucksList)
            {
                trucksList.Add(new DedicatedShippingRequestTruck { 
                    ShippingRequestId = shippingRequest.Id,
                    TruckId = truck.Id, 
                    Status = status,
                KPI = _settingManager.GetSettingValue<double>(AppSettings.KPI.TruckKPI)
            });
            }
            shippingRequest.DedicatedShippingRequestTrucks = trucksList;

            //Add drivers
            var driversList = new List<DedicatedShippingRequestDriver>();
            foreach (var driver in input.DriversList)
            {
                driversList.Add(new DedicatedShippingRequestDriver { ShippingRequestId = shippingRequest.Id, DriverUserId = driver.Id, Status = status });
            }
            shippingRequest.DedicatedShippingRequestDrivers = driversList;
        }
        #endregion

        #region trip
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<List<SelectItemDto>> GetAllDedicatedDriversForDropDown(long shippingRequestId)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestDriverRepository.GetAll()
                .Include(x=>x.DriverUser)
                .WhereIf(await IsShipper(), x=>x.ShippingRequest.TenantId==AbpSession.TenantId)
                .WhereIf(await IsTachyonDealer(), x=>true)
                .Where(e => e.ShippingRequestId == shippingRequestId)
                .Select(x => new SelectItemDto { Id = x.DriverUserId.ToString(), DisplayName = $"{x.DriverUser.Name} {x.DriverUser.Surname}" })
                .ToListAsync();
        }

        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<List<SelectItemDto>> GetAllDedicateTrucksForDropDown(long shippingRequestId)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestTruckRepository.GetAll()
                .Include(x=>x.Truck)
                .WhereIf(await IsShipper(), x => x.ShippingRequest.TenantId == AbpSession.TenantId)
                .WhereIf(await IsTachyonDealer(), x => true)
                .Where(x => x.ShippingRequestId == shippingRequestId)
                .Select(x => new SelectItemDto
                {
                    DisplayName = x.Truck.GetDisplayName(),
                    Id = x.TruckId.ToString()
                }).ToListAsync();
        }

        #endregion

        #region KPI
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task UpdateRequestKPI(UpdateRequestKPIInput input)
        {
            var request = await _shippingRequestRepository.FirstOrDefaultAsync(input.ShippingRequestId);
            request.DedicatedKPI = input.KPI;
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task UpdateTruckKPI(UpdateTruckKPIInput input)
        {
            var dedicatedTruck = await _dedicatedShippingRequestTruckRepository.FirstOrDefaultAsync(input.DedicatedTruckId);
            dedicatedTruck.KPI = input.KPI;
        }

        #endregion

        #region Helper
        private async Task<long> CreateStep1(CreateOrEditDedicatedStep1Dto input)
        {
            ShippingRequest shippingRequest = ObjectMapper.Map<ShippingRequest>(input);
            shippingRequest.ShippingRequestFlag = ShippingRequestFlag.Dedicated;
            shippingRequest.DedicatedKPI = _settingManager.GetSettingValue<double>(AppSettings.KPI.RequestKPI);
            await _shippingRequestManager.CreateStep1Manager(shippingRequest, input);
            var SR= await _shippingRequestRepository.InsertAndGetIdAsync(shippingRequest);
            

            //add new or remove destinaton cities
            await AddOrRemoveDestinationCities(input, shippingRequest);
            return SR;
        }


        private async Task<long> UpdateStep1(CreateOrEditDedicatedStep1Dto input)
        {
            var shippingRequest = await GetDraftedDedicatedShippingRequestForStep1(input.Id.Value);
            if (shippingRequest.Status == ShippingRequestStatus.PostPrice) throw new UserFriendlyException(L("UpdateNotAllowed"));
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

        private async Task ValidateTrucksAndDrivers(AssignDedicatedTrucksAndDriversInput input, ShippingRequest shippingRequest)
        {
            if (shippingRequest.DedicatedShippingRequestTrucks.Count() > 0) throw new UserFriendlyException(L("TrucksAndDriversAlreadyAssigned"));

            if (input.TrucksList.Count != input.DriversList.Count || shippingRequest.NumberOfTrucks != input.TrucksList.Count ||
                shippingRequest.NumberOfTrucks != input.DriversList.Count)
            {
                throw new UserFriendlyException(L(String.Format("TrucksAndDriversMustBe {0}", shippingRequest.NumberOfTrucks)));
            }

            //check if truck is busy


            //foreach(var truck in input.TrucksList)
            //{
            //    if (await _shippingRequestManager.CheckIfTruckIsRented(truck.Id))
            //        throw new UserFriendlyException(L(String.Format("The truck {0} is rented", truck.TruckName)));

            //}
            var RentedTruck =await IsAnyTruckBusyDuringRentalDuration(input.TrucksList.ToList(), shippingRequest);
            if (RentedTruck !=null)
            {
                throw new UserFriendlyException(L(String.Format("The truck {0} is rented", input.TrucksList.Where(x=>x.Id == RentedTruck).First().TruckName)));
            }

            //foreach (var driver in input.DriversList)
            //{
            //    if (await _shippingRequestManager.CheckIfDriverIsRented(driver.Id))
            //        throw new UserFriendlyException(L(String.Format("The driver {0} is rented", driver.DriverName)));

            //}
            var RentedDriver =await IsAnyDriverBusyDuringRentalDuration(input.DriversList.ToList(), shippingRequest);
            if (RentedDriver != null)
            {
                throw new UserFriendlyException(L(String.Format("The driver {0} is rented", input.DriversList.Where(x => x.Id == RentedDriver).First().DriverName)));
            }
        }

        private async Task<long?> IsAnyTruckBusyDuringRentalDuration(List<DedicatedTruckDto> truckDtos, ShippingRequest shippingRequest)
        {
            var item = await _dedicatedShippingRequestTruckRepository.GetAll()
                .Where(x => truckDtos.Select(y=>y.Id).Contains(x.TruckId) && x.ShippingRequestId != shippingRequest.Id &&
                shippingRequest.RentalStartDate.Value.Date <= x.ShippingRequest.RentalEndDate.Value.Date &&
                x.ShippingRequest.RentalStartDate.Value.Date <= shippingRequest.RentalEndDate.Value.Date)
                .FirstOrDefaultAsync();
            if (item != null) return item.TruckId;
            return null;
        }

        private async Task<long?> IsAnyDriverBusyDuringRentalDuration(List<DedicatedDriversDto> driverDtos, ShippingRequest shippingRequest)
        {
            var item = await _dedicatedShippingRequestDriverRepository.GetAll()
                .Where(x => driverDtos.Select(y=>y.Id).Contains(x.DriverUserId) && x.ShippingRequestId != shippingRequest.Id &&
                shippingRequest.RentalStartDate.Value.Date <= x.ShippingRequest.RentalEndDate.Value.Date &&
                x.ShippingRequest.RentalStartDate.Value.Date <= shippingRequest.RentalEndDate.Value.Date)
                .FirstOrDefaultAsync();
            if(item != null) return item.DriverUserId;
            return null;
        }

        private async Task<ShippingRequest> GetDraftedDedicatedShippingRequestForStep1(long id)
        {
            DisableDraftedFilter();
            if (await IsTachyonDealer())
                DisableTenancyFilters();
            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x => x.ShippingRequestDestinationCities)
                .ThenInclude(x => x.CityFk)
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(await IsShipper(), x=>x.TenantId==AbpSession.TenantId)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return shippingRequest;
        }

        private async Task<ShippingRequest> GetDraftedDedicatedShippingRequestForStep2(long id)
        {
            DisableDraftedFilter();
            if (await IsTachyonDealer())
                DisableTenancyFilters();
            ShippingRequest shippingRequest = await _shippingRequestRepository.GetAll()
                .Include(x => x.ShippingRequestVases)
                .WhereIf(await IsTachyonDealer(), x => true)
                //.WhereIf(await IsShipper(), x => x.TenantId == AbpSession.TenantId)
                .Where(x => x.Id == id && x.Status!=ShippingRequestStatus.PostPrice)
                .FirstOrDefaultAsync();
            return shippingRequest;
        }
        #endregion
    }
}
