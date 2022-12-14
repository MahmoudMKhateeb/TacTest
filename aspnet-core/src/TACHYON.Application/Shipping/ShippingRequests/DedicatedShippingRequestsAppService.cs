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
using TACHYON.Authorization.Users;
using TACHYON.Configuration;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Notifications;
using TACHYON.Shipping.Dedicated;
using TACHYON.Shipping.DirectRequests;
using TACHYON.Shipping.DirectRequests.Dto;
using TACHYON.Shipping.ShippingRequests.Dtos.Dedicated;
using TACHYON.Trucks;

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
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IAppNotifier _appNotifier;

        public DedicatedShippingRequestsAppService(IRepository<ShippingRequest, long> shippingRequestRepository,
            ShippingRequestManager shippingRequestManager,
            IRepository<ShippingRequestDestinationCity> shippingRequestDestinationCityRepository,
            ShippingRequestDirectRequestAppService shippingRequestDirectRequestAppService,
            IRepository<DedicatedShippingRequestTruck, long> dedicatedShippingRequestTruckRepository,
            IRepository<DedicatedShippingRequestDriver, long> dedicatedShippingRequestDriverRepository,
            ISettingManager settingManager,
            IRepository<Truck, long> truckRepository,
            IRepository<User, long> lookup_userRepository,
            IAppNotifier appNotifier)
        {
            _shippingRequestRepository = shippingRequestRepository;
            _shippingRequestManager = shippingRequestManager;
            _shippingRequestDestinationCityRepository = shippingRequestDestinationCityRepository;
            _shippingRequestDirectRequestAppService = shippingRequestDirectRequestAppService;
            _dedicatedShippingRequestTruckRepository = dedicatedShippingRequestTruckRepository;
            _dedicatedShippingRequestDriverRepository = dedicatedShippingRequestDriverRepository;
            _settingManager = settingManager;
            _truckRepository = truckRepository;
            _lookup_userRepository = lookup_userRepository;
            _appNotifier = appNotifier;
        }

        #region Wizard
        public async Task<LoadResult> GetAllDedicatedTrucks(GetAllDedicatedTrucksInput input)
        {
            DisableTenancyFilters();
            var query = _dedicatedShippingRequestTruckRepository.GetAll()
                .WhereIf(await IsTachyonDealer(),x=> true)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x => x.ShippingRequest.CarrierTenantId == AbpSession.TenantId ||
                x.ShippingRequest.TenantId == AbpSession.TenantId)
                .WhereIf(input.ShippingRequestId != null, x => x.ShippingRequestId == input.ShippingRequestId)
                .ProjectTo<DedicatedShippingRequestTrucksDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();

            return await LoadResultAsync(query, input.Filter);
            
        }

        public async Task<LoadResult> GetAllDedicatedDrivers(GetAllDedicatedDriversInput input)
        {
            DisableTenancyFilters();
            var query = _dedicatedShippingRequestDriverRepository.GetAll()
                .WhereIf(await IsShipper(), x => x.ShippingRequest.TenantId == AbpSession.TenantId)
                .WhereIf(await IsCarrier(), x => x.ShippingRequest.CarrierTenantId == AbpSession.TenantId)
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
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.CarrierAsASaas, AppFeatures.TachyonDealer, AppFeatures.CarrierClients)]
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
                trucksList.Add(new DedicatedShippingRequestTruck
                {
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

        /// <summary>
        /// This is another service for assign truck and driver
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequiresFeature(AppFeatures.Carrier, AppFeatures.CarrierAsASaas, AppFeatures.TachyonDealer, AppFeatures.CarrierClients)]
        public async Task AssignTrucksAndDriversForDedicated(AssignTrucksAndDriversForDedicatedInput input)
        {
            var shippingRequest = await _shippingRequestManager.GetShippingRequestForAssign(input.ShippingRequestId);

            if (shippingRequest is null) throw new UserFriendlyException(L("ShippingRequestNotFound"));

            await ValidateTrucksAndDrivers(input, shippingRequest);

            var status = shippingRequest.RentalStartDate.Value.Date <= Clock.Now.Date
                ? WorkingStatus.Busy
                : WorkingStatus.Active;

            //Add trucks
            var trucksList = new List<DedicatedShippingRequestTruck>();
            var driversList = new List<DedicatedShippingRequestDriver>();
            foreach (var driverAndtruck in input.DedicatedShippingRequestTrucksAndDriversDtos)
            {
                trucksList.Add(new DedicatedShippingRequestTruck
                {
                    ShippingRequestId = shippingRequest.Id,
                    TruckId = driverAndtruck.TruckId,
                    DriverUserId = driverAndtruck.DriverId,
                    Status = status,
                    ReplacementFlag = ReplacementFlag.Original,
                    KPI = _settingManager.GetSettingValue<double>(AppSettings.KPI.TruckKPI)
                });

                driversList.Add(new DedicatedShippingRequestDriver 
                { 
                    ShippingRequestId = shippingRequest.Id,
                    DriverUserId = driverAndtruck.DriverId, 
                    Status = status ,
                    ReplacementFlag = ReplacementFlag.Original
                });

            }
            shippingRequest.DedicatedShippingRequestTrucks = trucksList;
            shippingRequest.DedicatedShippingRequestDrivers = driversList;

        }

        public async Task<List<SelectItemDto>> GetAllCarrierTrucksByTruckTypeForDropDown(long truckTypeId, int? tenantId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _truckRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x=>x.TenantId == tenantId.Value)
                .Where(x => x.TrucksTypeId == truckTypeId)
                .Select(x => new SelectItemDto
                {
                    DisplayName = x.GetDisplayName(),
                    Id = x.Id.ToString()
                }).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllDriversForDropDown(int? tenantId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _lookup_userRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x=>x.TenantId == tenantId.Value)
                .Where(e => e.IsDriver == true)
                .Select(x => new SelectItemDto { Id = x.Id.ToString(), DisplayName = $"{x.Name} {x.Surname}" })
                .ToListAsync();
        }

        public async Task<List<GetAllTrucksWithDriversListDto>> GetAllTrucksWithDriversList(long truckTypeId, int? tenantId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _truckRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => x.TenantId == tenantId.Value)
                .Where(x => x.TrucksTypeId == truckTypeId)
                .Select(x => new GetAllTrucksWithDriversListDto
                {
                    TruckName = x.GetDisplayName(),
                    TruckId = x.Id,
                    DriverUserId=x.DriverUserId,
                    DriverName = x.DriverUserFk!=null ?x.DriverUserFk.Name :""
                }).ToListAsync();
        }

        
        #endregion

        #region trip
        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<List<GetAllDedicatedDriversOrTrucksForDropDownDto>> GetAllDedicatedDriversForDropDown(long shippingRequestId)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestDriverRepository.GetAll()
                .Include(x=>x.DriverUser)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x=>x.ShippingRequest.TenantId==AbpSession.TenantId)
                .WhereIf(await IsTachyonDealer(), x=>true)
                .Where(e => e.ShippingRequestId == shippingRequestId)
                .Select(x => new GetAllDedicatedDriversOrTrucksForDropDownDto { Id = x.DriverUserId.ToString()
                ,IsAvailable = !x.ReplacementDate.HasValue || 
                (x.ReplacementFlag == ReplacementFlag.Replaced && x.ReplacementDate.Value.Date.AddDays(x.ReplacementIntervalInDays.Value) > Clock.Now.Date ) ||
                (x.ReplacementFlag == ReplacementFlag.Original && x.ReplacementDate.Value.Date.AddDays(x.ReplacementIntervalInDays.Value) < Clock.Now.Date),
                    DisplayName = $"{x.DriverUser.Name} {x.DriverUser.Surname}" }                )
                .ToListAsync();
        }

        [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer)]
        public async Task<List<GetAllDedicatedDriversOrTrucksForDropDownDto>> GetAllDedicateTrucksForDropDown(long shippingRequestId)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestTruckRepository.GetAll()
                .Include(x=>x.Truck)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x => x.ShippingRequest.TenantId == AbpSession.TenantId)
                .WhereIf(await IsTachyonDealer(), x => true)
                .Where(x => x.ShippingRequestId == shippingRequestId)
                .Select(x => new GetAllDedicatedDriversOrTrucksForDropDownDto
                {
                    DisplayName = x.Truck.GetDisplayName(),
                    Id = x.TruckId.ToString(),
                    IsAvailable = !x.ReplacementDate.HasValue ||
                (x.ReplacementFlag == ReplacementFlag.Replaced && x.ReplacementDate.Value.Date.AddDays(x.ReplacementIntervalInDays.Value) > Clock.Now.Date) ||
                (x.ReplacementFlag == ReplacementFlag.Original && x.ReplacementDate.Value.Date.AddDays(x.ReplacementIntervalInDays.Value) <= Clock.Now.Date)
                }).ToListAsync();
        }

        public async Task<long?> GetDriverOrTruckForTripAssign(long? truckId, long? DriverUserId, long shippingRequestId)
        {
            if (truckId == null && DriverUserId == null) throw new UserFriendlyException(L("EvenTruckOrDriverMustHaveValue"));
            await DisableTenancyFiltersIfTachyonDealer();
            var dedicatedTruck= await _dedicatedShippingRequestTruckRepository.GetAll()
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x => x.ShippingRequest.TenantId == AbpSession.TenantId)
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(truckId != null, x => x.TruckId == truckId)
                .WhereIf(DriverUserId != null, x => x.DriverUserId == DriverUserId)
                .Where(x => x.ShippingRequestId == shippingRequestId)
                .FirstOrDefaultAsync();
            if (dedicatedTruck != null && truckId != null)
                return dedicatedTruck.DriverUserId;
            else if (DriverUserId != null && dedicatedTruck != null)
                return dedicatedTruck.TruckId;
            return null;
        }

        #endregion

        #region DropDowns
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<List<SelectItemDto>> GetDedicatedRequestsByTenant(int tenantId)
        {
            DisableTenancyFilters();
            return await _shippingRequestRepository.GetAll()
                .Where(x => (x.TenantId == tenantId || x.CarrierTenantId == tenantId) &&
            (x.Status == ShippingRequestStatus.PostPrice || x.Status == ShippingRequestStatus.Expired)  &&
            x.ShippingRequestFlag == ShippingRequestFlag.Dedicated)
                .Select(x => new SelectItemDto
                {
                    DisplayName = x.ReferenceNumber,
                    Id = x.Id.ToString()
                }).ToListAsync();
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task<List<SelectItemDto>> GetDedicateTrucksByRequest(long shippingRequestId)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestTruckRepository.GetAll()
                .Include(x => x.Truck)
                .Where(x => x.ShippingRequestId == shippingRequestId)
                .Select(x => new SelectItemDto
                {
                    DisplayName = x.ReplacementFlag== ReplacementFlag.Replaced ?$"{x.Truck.GetDisplayName()}{"Replacement"}{x.OriginalTruck.Truck.GetDisplayName()}" :x.Truck.GetDisplayName(),
                    Id = x.Id.ToString()
                }).ToListAsync();
        }

        #endregion

        #region KPI
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task UpdateRequestKPI(UpdateRequestKPIInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var request = await _shippingRequestRepository.FirstOrDefaultAsync(input.ShippingRequestId);
            request.DedicatedKPI = input.KPI;
        }
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task UpdateTruckKPI(UpdateTruckKPIInput input)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            var dedicatedTruck = await _dedicatedShippingRequestTruckRepository.FirstOrDefaultAsync(input.DedicatedTruckId);
            dedicatedTruck.KPI = input.KPI;
        }

        #endregion

        #region Replacement
        #region Trucks
        public async Task<LoadResult> GetDedicatedTrucksForReplace(long shippingRequestId, string Filter)
        {
            DisableTenancyFilters();
            var query = _dedicatedShippingRequestTruckRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x => x.ShippingRequest.CarrierTenantId == AbpSession.TenantId &&
                x.Truck.TenantId == AbpSession.TenantId)
                .Where( x => x.ShippingRequestId == shippingRequestId && x.ShippingRequest.Status == ShippingRequestStatus.PostPrice &&
                x.ReplacementFlag == ReplacementFlag.Original)
                .ProjectTo<GetDedicatedTruckForReplaceDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();

            return await LoadResultAsync(query, Filter);
        }

        /// <summary>
        /// Return All trucks for replacement except dedicated for this request
        /// </summary>
        /// <param name="shippingRequestId"></param>
        /// <param name="truckTypeId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<SelectItemDto>> GetReplacementTrucksForDropDown(long shippingRequestId, long truckTypeId, int? tenantId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _truckRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => x.TenantId == tenantId.Value)
                .Where(x => x.TrucksTypeId == truckTypeId && 
                !x.DedicatedShippingRequestTrucks.Any(y=>y.ShippingRequestId == shippingRequestId && 
                (y.ReplacementFlag == ReplacementFlag.Original || 
                (y.ReplacementFlag== ReplacementFlag.Replaced && y.ReplacementDate.Value.AddDays(y.ReplacementIntervalInDays.Value) < Clock.Now.Date)
                )))
                .Select(x => new SelectItemDto
                {
                    DisplayName = x.GetDisplayName(),
                    Id = x.Id.ToString()
                }).ToListAsync();
        }

        /// <summary>
        /// TMS can make a request to carrier in specific truck to replace
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task RequestReplaceTruck(RequestReplaceTruckInput input)
        {
            var dedicatedTruck =await GetDedicateTruckById(input.DedicatedTruckId);
            if (dedicatedTruck.IsRequestedToReplace) throw new UserFriendlyException(L("TruckIsRequestedToReplaceBefore"));
            if(dedicatedTruck.ReplacementFlag == ReplacementFlag.Replaced) throw new UserFriendlyException(L("OriginalTruckMustnotBeReplaced"));
            dedicatedTruck.ReplacementReason = input.ReplacementReason;
            dedicatedTruck.ReplacementIntervalInDays = input.ReplacementIntervalInDays;
            dedicatedTruck.IsRequestedToReplace = true;

            await _appNotifier.NotifyCarrierWithTruckReplacement(dedicatedTruck);
        }

        public async Task ReplaceTrucks(ReplaceTruckInput input)
        {
            var dedicatedTrucks = await GetDedicateTrucksByIds(input);

            if (dedicatedTrucks.Any(x => x.ReplacementFlag == ReplacementFlag.Replaced)) throw new UserFriendlyException(L("OriginalTruckMustnotBeReplaced"));
            if (dedicatedTrucks.Any(x => x.ReplacementDate != null && x.ReplacementDate.Value.Date.AddDays(x.ReplacementIntervalInDays.Value) > Clock.Now)) throw new UserFriendlyException(L("SomeTrucksAreCurrentlyReplaced"));

            var replacedTrucks = ObjectMapper.Map<List<DedicatedShippingRequestTruck>>(input.ReplaceTruckDtos.Distinct());
            foreach (var item in replacedTrucks)
            {
                item.ReplacementFlag = ReplacementFlag.Replaced;
                item.ReplacementDate = Clock.Now;
                item.KPI = _settingManager.GetSettingValue<double>(AppSettings.KPI.TruckKPI);
                item.ShippingRequestId = input.ShippingRequestId;
                item.Status = dedicatedTrucks.First(x=>x.Id == item.OriginalDedicatedTruckId).Status;

                _dedicatedShippingRequestTruckRepository.Insert(item);
            }

            // cancel request to replace in  original trucks
            foreach(var originalTruck in dedicatedTrucks)
            {
                originalTruck.IsRequestedToReplace = false;
                originalTruck.ReplacementDate = Clock.Now;
                originalTruck.ReplacementIntervalInDays = input.ReplaceTruckDtos.FirstOrDefault(x => x.OriginalDedicatedTruckId == originalTruck.Id).ReplacementIntervalInDays;
            }
           
        }

        #endregion
        #region Drivers
        public async Task<LoadResult> GetDedicatedDriversForReplace(long shippingRequestId, string Filter)
        {
            DisableTenancyFilters();
            var query = _dedicatedShippingRequestDriverRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x => x.ShippingRequest.CarrierTenantId == AbpSession.TenantId &&
                x.DriverUser.TenantId == AbpSession.TenantId)
                .Where(x => x.ShippingRequestId == shippingRequestId && x.ShippingRequest.Status == ShippingRequestStatus.PostPrice &&
                x.ReplacementFlag == ReplacementFlag.Original)
                .ProjectTo<GetDedicatedDriverForReplaceDto>(AutoMapperConfigurationProvider)
                .AsNoTracking();

            return await LoadResultAsync(query, Filter);
        }

        /// <summary>
        /// Return All drivers for replacement except dedicated for this request
        /// </summary>
        /// <param name="shippingRequestId"></param>
        /// <param name="truckTypeId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<List<SelectItemDto>> GetReplacementDriversForDropDown(long shippingRequestId, int? tenantId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _lookup_userRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => x.TenantId == tenantId.Value)
                .Where(e => e.IsDriver == true && !e.DedicatedShippingRequestDrivers.Any(x=>x.ShippingRequestId == shippingRequestId))
                .Select(x => new SelectItemDto { Id = x.Id.ToString(), DisplayName = $"{x.Name} {x.Surname}" })
                .ToListAsync();
        }

        /// <summary>
        /// TMS can make a request to carrier in specific driver to replace
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [RequiresFeature(AppFeatures.TachyonDealer)]
        public async Task RequestReplaceDriver(RequestReplaceDriverInput input)
        {
            var dedicatedDriver = await GetDedicatedDriverById(input.DedicatedDriverId);
            if (dedicatedDriver.IsRequestedToReplace) throw new UserFriendlyException(L("TruckIsRequestedToReplaceBefore"));
            if (dedicatedDriver.ReplacementFlag == ReplacementFlag.Replaced) throw new UserFriendlyException(L("OriginalDriverMustnotBeReplaced"));
            dedicatedDriver.ReplacementReason = input.ReplacementReason;
            dedicatedDriver.ReplacementIntervalInDays = input.ReplacementIntervalInDays;
            dedicatedDriver.IsRequestedToReplace = true;

            await _appNotifier.NotifyCarrierWithDriverReplacement(dedicatedDriver);
        }

        public async Task ReplaceDrivers(ReplaceDriverInput input)
        {
            var dedicatedDrivers = await GetDedicateDriversByIds(input);
            if (dedicatedDrivers.Any(x=>x.ReplacementFlag == ReplacementFlag.Replaced)) throw new UserFriendlyException(L("OriginalDriverMustnotBeReplaced"));
            if (dedicatedDrivers.Any(x => x.ReplacementDate != null && x.ReplacementDate.Value.Date.AddDays(x.ReplacementIntervalInDays.Value) > Clock.Now)) throw new UserFriendlyException(L("SomeDriversAreCurrentlyReplaced"));

            var ReplacedDrivers = ObjectMapper.Map<List<DedicatedShippingRequestDriver>>(input.ReplaceDriverDtos);
            foreach (var item in ReplacedDrivers)
            {
                item.ReplacementFlag = ReplacementFlag.Replaced;
                item.ReplacementDate = Clock.Now;
                item.ShippingRequestId = input.ShippingRequestId;
                item.Status = dedicatedDrivers.First(x => x.Id == item.OriginalDedicatedDriverId).Status;

                _dedicatedShippingRequestDriverRepository.Insert(item);
            }

            // cancel request to replace in  original drivers
            foreach (var originalDriver in dedicatedDrivers)
            {
                originalDriver.IsRequestedToReplace = false;
                originalDriver.ReplacementDate = Clock.Now;
                originalDriver.ReplacementIntervalInDays = input.ReplaceDriverDtos.FirstOrDefault(x => x.OriginalDedicatedDriverId == originalDriver.Id).ReplacementIntervalInDays;
            }

        }
        #endregion

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

            var RentedTruck =await _shippingRequestManager.IsAnyTruckBusyDuringRentalDuration(input.TrucksList.Select(x=>x.Id).ToList(), shippingRequest);
            if (RentedTruck !=null)
            {
                throw new UserFriendlyException(L(String.Format("The truck {0} is rented", input.TrucksList.Where(x=>x.Id == RentedTruck).First().TruckName)));
            }

            var RentedDriver =await _shippingRequestManager.IsAnyDriverBusyDuringRentalDuration(input.DriversList.Select(x=>x.Id).ToList(), shippingRequest);
            if (RentedDriver != null)
            {
                throw new UserFriendlyException(L(String.Format("The driver {0} is rented", input.DriversList.Where(x => x.Id == RentedDriver).First().DriverName)));
            }
        }

        private async Task ValidateTrucksAndDrivers(AssignTrucksAndDriversForDedicatedInput input, ShippingRequest shippingRequest)
        {
            if (shippingRequest.DedicatedShippingRequestTrucks.Count() > 0) throw new UserFriendlyException(L("TrucksAndDriversAlreadyAssigned"));

            if (shippingRequest.NumberOfTrucks != input.DedicatedShippingRequestTrucksAndDriversDtos.Count)
            {
                throw new UserFriendlyException(L(String.Format("TrucksAndDriversMustBe {0}", shippingRequest.NumberOfTrucks)));
            }

            var RentedTruck = await _shippingRequestManager.IsAnyTruckBusyDuringRentalDuration(input.DedicatedShippingRequestTrucksAndDriversDtos.Select(x=>x.TruckId).ToList(), shippingRequest);
            if (RentedTruck != null)
            {
                throw new UserFriendlyException(L(String.Format("The truck {0} is rented", input.DedicatedShippingRequestTrucksAndDriversDtos.Where(x => x.TruckId == RentedTruck).First().TruckName)));
            }

            var RentedDriver = await _shippingRequestManager.IsAnyDriverBusyDuringRentalDuration(input.DedicatedShippingRequestTrucksAndDriversDtos.Select(x=>x.DriverId).ToList(), shippingRequest);
            if (RentedDriver != null)
            {
                throw new UserFriendlyException(L(String.Format("The driver {0} is rented", input.DedicatedShippingRequestTrucksAndDriversDtos.Where(x => x.DriverId == RentedDriver).First().DriverName)));
            }
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

        [RequiresFeature(AppFeatures.TachyonDealer)]
        private async Task<DedicatedShippingRequestTruck> GetDedicateTruckById(long id)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestTruckRepository.GetAll()
                .Include(x=>x.ShippingRequest)
                .ThenInclude(x=>x.CarrierTenantFk)
                .Include(x=>x.Truck)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task<List<DedicatedShippingRequestTruck>> GetDedicateTrucksByIds(ReplaceTruckInput input)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestTruckRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x => x.Truck.TenantId == AbpSession.TenantId)
                .Include(x => x.ShippingRequest)
                .ThenInclude(x => x.CarrierTenantFk)
                .Include(x => x.Truck)
                .Where(x => input.ReplaceTruckDtos.Select(x => x.OriginalDedicatedTruckId).Contains(x.Id) &&
                x.ShippingRequestId== input.ShippingRequestId)
                .ToListAsync();
        }

        [RequiresFeature(AppFeatures.TachyonDealer)]
        private async Task<DedicatedShippingRequestDriver> GetDedicatedDriverById(long id)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestDriverRepository.GetAll()
                .Include(x => x.ShippingRequest)
                .ThenInclude(x => x.CarrierTenantFk)
                .Include(x => x.DriverUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        private async Task<List<DedicatedShippingRequestDriver>> GetDedicateDriversByIds(ReplaceDriverInput input)
        {
            DisableTenancyFilters();
            return await _dedicatedShippingRequestDriverRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x => true)
                .WhereIf(AbpSession.TenantId.HasValue && !await IsTachyonDealer(), x => x.DriverUser.TenantId == AbpSession.TenantId)
                .Include(x => x.ShippingRequest)
                .ThenInclude(x => x.CarrierTenantFk)
                .Include(x => x.DriverUser)
                .Where(x => input.ReplaceDriverDtos.Select(x => x.OriginalDedicatedDriverId).Contains(x.Id) &&
                x.ShippingRequestId == input.ShippingRequestId)
                .ToListAsync();
        }
        #endregion
    }
}
