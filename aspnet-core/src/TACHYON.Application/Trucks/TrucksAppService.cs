using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Runtime.Validation;
using Abp.UI;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Profile.Dto;
using TACHYON.Configuration;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Dto;
using TACHYON.Editions;
using TACHYON.Features;
using TACHYON.Integration.WaslIntegration;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Storage;
using TACHYON.Trucks;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.Exporting;
using TACHYON.Trucks.PlateTypes;
using TACHYON.Trucks.PlateTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using GetAllForLookupTableInput = TACHYON.Trucks.Dtos.GetAllForLookupTableInput;

namespace TACHYON.Trucks
{
    //[AbpAuthorize(AppPermissions.Pages_Trucks)]
    //[RequiresFeature(AppFeatures.Carrier, AppFeatures.TachyonDealer)]
    public class TrucksAppService : TACHYONAppServiceBase, ITrucksAppService
    {
        private const int MaxTruckPictureBytes = 5242880; //5MB
        private readonly IRepository<Truck, long> _truckRepository;
        private readonly ITrucksExcelExporter _trucksExcelExporter;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<TruckStatus, long> _lookup_truckStatusRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly DocumentFilesAppService _documentFilesAppService;
        private readonly IRepository<TransportType, int> _transportTypeRepository;
        private readonly IRepository<Capacity, int> _capacityRepository;
        private readonly IRepository<PlateType> _plateTypesRepository;
        private readonly WaslIntegrationManager _waslIntegrationManager;
        private readonly IRepository<Tenant> _lookupTenantRepository;
        private readonly IRepository<ShippingRequest,long> _shippingRequestRepository;
        private readonly IRepository<User,long> _userRepository;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<UserOrganizationUnit,long> _userOrganizationUnitRepository;
        private readonly IFeatureChecker _featureChecker;





        public TrucksAppService(IRepository<DocumentType, long> documentTypeRepository, IRepository<DocumentFile, Guid> documentFileRepository,
            IRepository<Truck, long> truckRepository, ITrucksExcelExporter trucksExcelExporter,
            IRepository<TrucksType, long> lookup_trucksTypeRepository, IRepository<TruckStatus, long> lookup_truckStatusRepository,
            IRepository<User, long> lookup_userRepository, IAppNotifier appNotifier, ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager, DocumentFilesAppService documentFilesAppService,
            IRepository<TransportType, int> transportTypeRepository, IRepository<Capacity, int> capacityRepository,
            IRepository<PlateType> PlateTypesRepository, IRepository<Tenant> lookupTenantRepository,
            WaslIntegrationManager waslIntegrationManager,
            IRepository<ShippingRequest, long> shippingRequestRepository,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository,
            IRepository<User, long> userRepository, IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, IFeatureChecker featureChecker)
        {
            _documentFileRepository = documentFileRepository;
            _documentTypeRepository = documentTypeRepository;
            _truckRepository = truckRepository;
            _trucksExcelExporter = trucksExcelExporter;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
            _lookup_truckStatusRepository = lookup_truckStatusRepository;
            _lookup_userRepository = lookup_userRepository;
            _appNotifier = appNotifier;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _documentFilesAppService = documentFilesAppService;
            _transportTypeRepository = transportTypeRepository;
            _capacityRepository = capacityRepository;
            _plateTypesRepository = PlateTypesRepository;
            _lookupTenantRepository = lookupTenantRepository;
            _waslIntegrationManager = waslIntegrationManager;
            _shippingRequestRepository = shippingRequestRepository;
            _userRepository = userRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _shippingRequestTripRepository = shippingRequestTripRepository;
            _featureChecker = featureChecker;
        }

        public async Task<LoadResult> GetAll(GetAllTrucksInput input)
        {

            DisableTenancyFilters();
            
            bool isCmsEnabled = await FeatureChecker.IsEnabledAsync(AppFeatures.CMS);
            
            List<long> userOrganizationUnits = null;
            if (isCmsEnabled)
            {
                userOrganizationUnits = await _userOrganizationUnitRepository.GetAll().Where(x => x.UserId == AbpSession.UserId)
                    .Select(x => x.OrganizationUnitId).ToListAsync();
            }
            
            var documentQuery = _documentFileRepository.GetAll()
                                               .Where(x => x.DocumentTypeFk.SpecialConstant == TACHYONConsts.TruckIstimaraDocumentTypeSpecialConstant.ToLower());
            var query = from truck in _truckRepository.GetAll()
                                               .WhereIf(AbpSession.TenantId.HasValue && !await IsEnabledAsync(AppFeatures.TachyonDealer), r => r.TenantId == AbpSession.TenantId)
                                               .Include(x=>x.DedicatedShippingRequestTrucks)
                                               .ThenInclude(x=>x.ShippingRequest)
                                               .Include((x=>x.DriverUserFk))
                                               .WhereIf(isCmsEnabled && !userOrganizationUnits.IsNullOrEmpty(),
                                                   x=> x.CarrierActorId.HasValue && userOrganizationUnits.Contains(x.CarrierActorFk.OrganizationUnitId))
                        join tenant in _lookupTenantRepository.GetAll() on truck.TenantId equals tenant.Id
                        join document in documentQuery on truck.Id equals document.TruckId

                        select new TruckDto()
                        {
                            CompanyName = tenant.companyName,
                            Capacity = truck.Capacity,
                            CapacityDisplayName = truck.CapacityFk != null ? truck.CapacityFk.DisplayName : "",
                            ModelName = truck.ModelName,
                            ModelYear = truck.ModelYear,
                            Length = truck.Length,
                            TruckStatusDisplayName = truck.TruckStatusFk != null ? truck.TruckStatusFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                .TranslatedDisplayName : truck.TruckStatusFk.DisplayName,
                            TransportTypeDisplayName = truck.TransportTypeFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                                            ? truck.TransportTypeFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName
                                            : truck.TransportTypeFk.DisplayName,
                            PlateNumber = truck.PlateNumber,
                            Note = truck.Note,
                            TransportTypeId = truck.TransportTypeId,
                            TrucksTypeDisplayName = truck.TrucksTypeFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                                            ? truck.TrucksTypeFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName
                                            : truck.TrucksTypeFk.DisplayName,
                            CapacityId = truck.CapacityId,
                            Id = truck.Id,
                            TrucksTypeId = truck.TrucksTypeId,
                            IstmaraNumber = document.Number,
                            OtherTransportTypeName = truck.OtherTransportTypeName,
                            OtherTrucksTypeName = truck.OtherTrucksTypeName,
                            WorkingTruckStatus = truck.DedicatedShippingRequestTrucks.Any(x=>x.Status == Shipping.Dedicated.WorkingStatus.Busy)== true ?L("Busy") :L("Active"),
                            DriverUser = truck.DriverUserFk.Name +"",
                            WorkingShippingRequestReference= truck.DedicatedShippingRequestTrucks.Any(x => x.Status == Shipping.Dedicated.WorkingStatus.Busy) == true 
                            ? truck.DedicatedShippingRequestTrucks.First().ShippingRequest.ReferenceNumber :"",
                            CarrierActorName = truck.CarrierActorFk.CompanyName,
                            TruckStatusId = truck.TruckStatusId
                        };

            var result = await LoadResultAsync(query, input.Filter);
            await FillIsMissingDocumentFiles(result);
            
            return result;
        }

        public async Task<GetTenantExceedsNumberOfTrucksDto> GetTenantExceedsNumberOfTrucks()
        {
            var dto = new GetTenantExceedsNumberOfTrucksDto();

            if (!await IsTachyonDealer() && AbpSession.TenantId.HasValue)
            {
                var trucksCount =await GetTrucksNo(AbpSession.TenantId.Value);
                var maxNumberOfTrucks = await GetMaxNumberOfTrucks(AbpSession.TenantId.Value);
                if (maxNumberOfTrucks >= trucksCount)
                {
                    dto.IsTenantExceedsNumberOfTrucks = true;
                    dto.CanAddTruck =await HaveAdditionalPrice(AbpSession.TenantId.Value);
                }
            }
            return dto;
        }
        


        public async Task<GetTruckForViewOutput> GetTruckForView(long id)
        {
            var truck = await _truckRepository.GetAsync(id);

            var output = new GetTruckForViewOutput { Truck = ObjectMapper.Map<TruckDto>(truck) };

            if (output.Truck.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.GetAllIncluding(x => x.Translations).FirstOrDefaultAsync(x => x.Id == output.Truck.TrucksTypeId);
                output.TrucksTypeDisplayName = ObjectMapper.Map<TrucksTypeDto>(_lookupTrucksType)?.TranslatedDisplayName;// _lookupTrucksType?.DisplayName?.ToString();
            }

            //if (output.Truck.TruckStatusId != null)
            //{
            //    var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync(output.Truck.TruckStatusId);
            //    output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            //}



            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Edit)]
        public async Task<GetTruckForEditOutput> GetTruckForEdit(EntityDto<long> input)
        {
            await DisableTenancyFiltersIfTachyonDealer();
            var truck = await _truckRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckForEditOutput { Truck = ObjectMapper.Map<CreateOrEditTruckDto>(truck) };

            if (output.Truck.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.GetAllIncluding(x => x.Translations).FirstOrDefaultAsync(x => x.Id == (long)output.Truck.TrucksTypeId);
                output.TrucksTypeDisplayName = ObjectMapper.Map<TrucksTypeDto>(_lookupTrucksType)?.TranslatedDisplayName;//_lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.Truck.TruckStatusId != null)
            {
                var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync((long)output.Truck.TruckStatusId);
                output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            }

            //if (output.Truck.Driver1UserId != null)
            //{
            //    var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Truck.Driver1UserId);
            //    output.UserName = _lookupUser?.Name?.ToString();
            //}


            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTruckDto input)
        {
            //check for zero values 
            if (input.TransportTypeId == 0)
            {
                input.TransportTypeId = null;
            }
            if (input.TrucksTypeId == 0)
            {
                input.TrucksTypeId = null;
            }
            if (input.CapacityId == 0)
            {
                input.CapacityId = null;
            }
            if (input.TruckStatusId == 0)
            {
                input.TruckStatusId = null;
            }



            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Create)]
        protected virtual async Task Create(CreateOrEditTruckDto input)
        {
            await OthersNameValidation(input);

            var truck = ObjectMapper.Map<Truck>(input);


            if (AbpSession.TenantId != null)
            {
                truck.TenantId = (int)AbpSession.TenantId;
            }

            if (!AbpSession.TenantId.HasValue || await IsEnabledAsync(AppFeatures.TachyonDealer))
            {
                // Use AbpValidationException to return Status Code => 400 Bad Request
                if (input.TenantId == null) throw new AbpValidationException(L("YouMustSetTenant"));
                if (!await FeatureChecker.IsEnabledAsync(input.TenantId.Value, AppFeatures.Carrier))
                    throw new AbpValidationException(L("TheTenantMustBeCarrier"));
                truck.TenantId = input.TenantId.Value;
            }

            var requiredDocs = await _documentFilesAppService.GetTruckRequiredDocumentFiles("");
            if (requiredDocs.Count > 0)
            {
                foreach (var item in requiredDocs)
                {
                    var doc = input.CreateOrEditDocumentFileDtos
                        .FirstOrDefault(x => x.DocumentTypeId == item.DocumentTypeId);
                    if (item.DocumentTypeDto.IsRequiredDocumentTemplate)
                    {
                        if (doc.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
                        {
                            throw new UserFriendlyException(L("document missing msg :" + item.Name));
                        }
                    }


                    doc.Name = item.DocumentTypeDto.DisplayName;
                }
            }


            var truckId = await _truckRepository.InsertAndGetIdAsync(truck);


            if (input.UpdateTruckPictureInput != null && !input.UpdateTruckPictureInput.FileToken.IsNullOrEmpty())
            {
                truck.PictureId = await AddOrUpdateTruckPicture(input.UpdateTruckPictureInput);
            }


            foreach (var item in input.CreateOrEditDocumentFileDtos)
            {
                item.TruckId = truckId;
                item.Name = item.Name + "_" + truckId.ToString();
                await _documentFilesAppService.CreateOrEdit(item);
            }


            //Wasl Integration

            if (await FeatureChecker.IsEnabledAsync(truck.TenantId, AppFeatures.IntegrationWslVehicleRegistration))
            {
                await _waslIntegrationManager.QueueVehicleRegistrationJob(truck.Id);

            }
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Edit)]
        protected virtual async Task Update(CreateOrEditTruckDto input)
        {
            await DisableTenancyFiltersIfTachyonDealer();

            await OthersNameValidation(input);

            var truck = await _truckRepository.FirstOrDefaultAsync(input.Id.Value);
            //if (input.Driver1UserId.HasValue && input.Driver1UserId != truck.Driver1UserId)
            //{
            //    await _appNotifier.AssignDriverToTruck(new UserIdentifier(AbpSession.TenantId, input.Driver1UserId.Value), truck.Id);
            //}
            input.TenantId = truck.TenantId;

            ObjectMapper.Map(input, truck);

            //Wasl Integration
            if (await FeatureChecker.IsEnabledAsync(truck.TenantId, AppFeatures.IntegrationWslVehicleRegistration))
            {
                  await _waslIntegrationManager.QueueVehicleRegistrationJob(truck.Id);

            }
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var truck = await _truckRepository.FirstOrDefaultAsync(input.Id);
            await _truckRepository.DeleteAsync(input.Id);

            //Wasl Integration
            if (await FeatureChecker.IsEnabledAsync(truck.TenantId, AppFeatures.IntegrationWslVehicleRegistration))
            {
                 await _waslIntegrationManager.QueueVehicleDeleteJob(truck);

            }
      
           
        }

        public async Task<FileDto> GetTrucksToExcel(GetAllTrucksForExcelInput input)
        {

            var filteredTrucks = _truckRepository.GetAll()
                .Include(e => e.TrucksTypeFk)
                .ThenInclude(e => e.Translations)
                .Include(e => e.TruckStatusFk)
                .Include(x => x.TransportTypeFk)
                //.Include(e => e.Driver1UserFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.PlateNumber.Contains(input.Filter) || e.ModelName.Contains(input.Filter) || e.ModelYear.Contains(input.Filter) || e.Note.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter), e => e.PlateNumber == input.PlateNumberFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.ModelNameFilter), e => e.ModelName == input.ModelNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.ModelYearFilter), e => e.ModelYear == input.ModelYearFilter)
                //.WhereIf(input.IsAttachableFilter > -1, e => (input.IsAttachableFilter == 1 && e.IsAttachable) || (input.IsAttachableFilter == 0 && !e.IsAttachable))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.Translations.Any(x => x.TranslatedDisplayName.Contains(input.TrucksTypeDisplayNameFilter)))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TruckStatusDisplayNameFilter), e => e.TruckStatusFk != null && e.TruckStatusFk.DisplayName == input.TruckStatusDisplayNameFilter);
            //.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.Driver1UserFk != null && e.Driver1UserFk.Name == input.UserNameFilter);

            var query = (from o in await filteredTrucks.ToListAsync()
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_truckStatusRepository.GetAll() on o.TruckStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                             //join o3 in _lookup_userRepository.GetAll() on o.Driver1UserId equals o3.Id into j3
                             //from s3 in j3.DefaultIfEmpty()


                         select new GetTruckForViewOutput()
                         {
                             Truck = new TruckDto
                             {
                                 PlateNumber = o.PlateNumber,
                                 ModelName = o.ModelName,
                                 ModelYear = o.ModelYear,
                                 Note = o.Note,
                                 Id = o.Id,
                                 Capacity = o.Capacity
                             },
                             TrucksTypeDisplayName =
                             (o.TransportTypeFk == null ? "" : ObjectMapper.Map<TransportTypeDto>(o.TransportTypeFk).TranslatedDisplayName) + "-" + //o.TransportTypeFk.DisplayName) + " - " +
                             (o.TrucksTypeFk == null ? "" : ObjectMapper.Map<TrucksTypeDto>(o.TrucksTypeFk).TranslatedDisplayName) + " - " +
                             (o.CapacityFk == null ? "" : ObjectMapper.Map<CapacityDto>(o.CapacityFk).DisplayName),
                             TruckStatusDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             //UserName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),

                         });


            var truckListDtos = query.ToList();

            return _trucksExcelExporter.ExportToFile(truckListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_Trucks)]
        public async Task<IEnumerable<ISelectItemDto>> GetAllTrucksTypeForTableDropdown()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                List<TrucksType> trucksTypes = await _lookup_trucksTypeRepository
                    .GetAllIncluding(x => x.Translations)
                    .ToListAsync();

                List<TrucksTypeSelectItemDto> trucksTypeDtos = ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(trucksTypes);

                return trucksTypeDtos;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks)]
        public async Task<List<TruckTruckStatusLookupTableDto>> GetAllTruckStatusForTableDropdown()
        {
            List<TruckStatus> trucksStatuses = await _lookup_truckStatusRepository
                .GetAllIncluding(x => x.Translations)
                .ToListAsync();

            List<TruckTruckStatusLookupTableDto> truckStatusDto = ObjectMapper.Map<List<TruckTruckStatusLookupTableDto>>(trucksStatuses);

            return truckStatusDto;

        }

        public async Task<List<SelectItemDto>> GetAllCarrierTrucksForDropDown()
        {
            return await _truckRepository.GetAll().Select(x => new SelectItemDto
            {
                DisplayName = x.PlateNumber + "-" + x.ModelName,
                Id = x.Id.ToString()
            }).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllCarrierTrucksByTruckTypeForDropDown(long truckTypeId, long tripId)
        {
            int? carrierTenantId;
            using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                carrierTenantId = _shippingRequestTripRepository.GetAll()
                .Where(x => x.Id == tripId)
                .Select(x => x.ShippingRequestFk.CarrierTenantId)
                .FirstOrDefault();
            }



            using (UnitOfWorkManager.Current.SetTenantId(carrierTenantId))
            {
                return await _truckRepository.GetAll()
               .Where(x => x.TrucksTypeId == truckTypeId)
               .Select(x => new SelectItemDto
               {
                   DisplayName = x.GetDisplayName(),
                   Id = x.Id.ToString()
               }).ToListAsync();
            }


        }

        //public async Task<List<SelectItemDto>> GetAllDriversForDropDown(long tripId)
        //{
        //    int? carrierTenantId;
        //    using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
        //    {
        //         carrierTenantId = _shippingRequestTripRepository.GetAll()
        //                   .Where(x => x.Id == tripId)
        //                   .Select(x => x.ShippingRequestFk.CarrierTenantId)
        //                   .FirstOrDefault();
        //    }


        //    using (UnitOfWorkManager.Current.SetTenantId(carrierTenantId))
        //    {
        //        return await _lookup_userRepository.GetAll().Where(e => e.IsDriver == true)
        //          .Select(x => new SelectItemDto { Id = x.Id.ToString(), DisplayName = $"{x.Name} {x.Surname}" })
        //          .ToListAsync();
        //    }


        //}

        public async Task<List<SelectItemDto>> GetAllDriversForDropDown(long? tenantId)

        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return await _lookup_userRepository.GetAll().Where(e => e.IsDriver == true)
                .WhereIf(await IsTachyonDealer(), x => x.TenantId == tenantId)
                .Select(x => new SelectItemDto { Id = x.Id.ToString(), DisplayName = $"{x.Name} {x.Surname}" })

                .ToListAsync();

        }


        
        
        public async Task<List<SelectItemDto>> GetDriversByShippingRequestId(long shippingRequestId)
        {
            var isTms = await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            
            var drivers = await (from driver in _userRepository.GetAll()

                join sr in _shippingRequestRepository.GetAll() on shippingRequestId equals sr.Id
                where driver.IsDriver && driver.TenantId == sr.CarrierTenantId &&
                      (!sr.CarrierActorId.HasValue || driver.CarrierActorId == sr.CarrierActorId)
                      && (isTms || sr.CarrierTenantId == AbpSession.TenantId)
                select new SelectItemDto()
                {
                    DisplayName = $"{driver.Name} {driver.Surname}", Id = driver.Id.ToString()
                }).ToListAsync();
            return drivers;
        }
        
        
        public async Task<List<SelectItemDto>> GetTrucksByShippingRequestId(long truckTypeId, long shippingRequestId)
        {
            var isTms = await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer);
            DisableTenancyFilters();
            
            var trucks = await (from truck in _truckRepository.GetAll()
                join sr in _shippingRequestRepository.GetAll() on shippingRequestId equals sr.Id
                where  truck.TrucksTypeId == truckTypeId && truck.TenantId == sr.CarrierTenantId &&
                      (!sr.CarrierActorId.HasValue || truck.CarrierActorId == sr.CarrierActorId) && (isTms || sr.CarrierTenantId == AbpSession.TenantId)
                select new SelectItemDto()
                {
                    DisplayName = truck.GetDisplayName(), Id = truck.Id.ToString()
                }).ToListAsync();
            return trucks;
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks)]
        public async Task<PagedResultDto<TruckUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().Where(e => e.IsDriver == true).WhereIf(
                   !string.IsNullOrWhiteSpace(input.Filter),
                  e => e.Name != null && e.Name.Contains(input.Filter)
               );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

            var lookupTableDtoList = new List<TruckUserLookupTableDto>();
            foreach (var user in userList)
            {
                lookupTableDtoList.Add(new TruckUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user.UserName?.ToString()
                });
            }

            return new PagedResultDto<TruckUserLookupTableDto>(
                totalCount,
                lookupTableDtoList
            );
        }

        public async Task<Guid> AddOrUpdateTruckPicture(UpdateTruckPictureInput input)
        {
            byte[] byteArray;

            var imageBytes = _tempFileCacheManager.GetFile(input.FileToken);

            if (imageBytes == null)
            {
                throw new UserFriendlyException("There is no such image file with the token: " + input.FileToken);
            }

            using (var bmpImage = new Bitmap(new MemoryStream(imageBytes)))
            {
                var width = (input.Width == 0 || input.Width > bmpImage.Width) ? bmpImage.Width : input.Width;
                var height = (input.Height == 0 || input.Height > bmpImage.Height) ? bmpImage.Height : input.Height;
                var bmCrop = bmpImage.Clone(new Rectangle(input.X, input.Y, width, height), bmpImage.PixelFormat);

                using (var stream = new MemoryStream())
                {
                    bmCrop.Save(stream, bmpImage.RawFormat);
                    byteArray = stream.ToArray();
                }
            }

            if (byteArray.Length > MaxTruckPictureBytes)
            {
                throw new UserFriendlyException(L("ResizedProfilePicture_Warn_SizeLimit",
                    AppConsts.ResizedMaxProfilPictureBytesUserFriendlyValue));
            }

            var storedFile = new BinaryObject(AbpSession.TenantId, byteArray);
            await _binaryObjectManager.SaveAsync(storedFile);

            return storedFile.Id;
        }

        [AbpAllowAnonymous]
        public async Task<string> GetPictureContentForTruck(long truckId)
        {
            var truck = await _truckRepository.GetAsync(truckId);
            if (truck.PictureId == null)
            {
                return "";
            }

            var file = await _binaryObjectManager.GetOrNullAsync(truck.PictureId.Value);
            return file == null ? "" : Convert.ToBase64String(file.Bytes);
        }


        #region Truck Categories
        public async Task<IEnumerable<ISelectItemDto>> GetAllTransportTypesForDropdown()
        {
            List<TransportType> transportTypes = await _transportTypeRepository
                .GetAllIncluding(x => x.Translations)
                .ToListAsync();

            List<TransportTypeSelectItemDto> transportTypeDtos = ObjectMapper.Map<List<TransportTypeSelectItemDto>>(transportTypes);

            return transportTypeDtos;
        }

        public async Task<IEnumerable<ISelectItemDto>> GetAllTruckTypesByTransportTypeIdForDropdown(int transportTypeId)
        {
            List<TrucksType> trucksTypes = await _lookup_trucksTypeRepository
                .GetAllIncluding(x => x.Translations)
                .Where(x => x.TransportTypeId == transportTypeId)
                .ToListAsync();

            List<TrucksTypeSelectItemDto> trucksTypeDtos = ObjectMapper.Map<List<TrucksTypeSelectItemDto>>(trucksTypes);

            return trucksTypeDtos;
        }

        public async Task<IEnumerable<ISelectItemDto>> GetAllTuckCapacitiesByTuckTypeIdForDropdown(int truckTypeId)
        {
            List<Capacity> capacity = await _capacityRepository
                .GetAllIncluding(x => x.Translations)
                .Where(x => x.TrucksTypeId == truckTypeId)
                .ToListAsync();

            List<CapacitySelectItemDto> capacityDto = ObjectMapper.Map<List<CapacitySelectItemDto>>(capacity);

            return capacityDto;
        }

        public async Task<List<PlateTypeSelectItemDto>> GetAllPlateTypeIdForDropdown()
        {
            var plateTypes = await _plateTypesRepository.GetAllIncluding(x => x.Translations)
                .ToListAsync();

            return ObjectMapper.Map<List<PlateTypeSelectItemDto>>(plateTypes);
        }

        public async Task<long?> GetTruckByDriverId(long driverId,long truckTypeId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return (await _truckRepository.GetAll().FirstOrDefaultAsync(x => x.DriverUserId == driverId && x.TrucksTypeId == truckTypeId))?.Id;
        }

        public async Task<long?> GetDriverByTruckId(long truckId)
        {
            await DisableTenancyFilterIfTachyonDealerOrHost();
            return (await _truckRepository.GetAll().FirstOrDefaultAsync(x => x.Id == truckId))?.DriverUserId;
        }

        #endregion

        #region Helpers

        private async Task OthersNameValidation(CreateOrEditTruckDto input)
        {
            #region Validate TransportType

            if (input.TransportTypeId != null)
            {
                var transportType = await _transportTypeRepository
                    .FirstOrDefaultAsync(input.TransportTypeId.Value);

                if (transportType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName) &&
                    input.OtherTransportTypeName.IsNullOrEmpty())
                    throw new UserFriendlyException(L("TransportTypeCanNotBeOtherAndEmptyAtSameTime"));
            }

            #endregion

            #region Validate TrucksType

            //? FYI TrucksTypeId Not Nullable 
            var trucksType = await _lookup_trucksTypeRepository
                .FirstOrDefaultAsync((long)input.TrucksTypeId);

            if (trucksType.DisplayName.ToLower().Contains(TACHYONConsts.OthersDisplayName) &&
                input.OtherTrucksTypeName.IsNullOrEmpty())
                throw new UserFriendlyException(L("TrucksTypeCanNotBeOtherAndEmptyAtSameTime"));

            #endregion


        }

        private async Task<int?> GetMaxNumberOfTrucks(int tenantId)
        {
            var trucksFeature = await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.NumberOfTrucks);
            if (trucksFeature)
            {
                var maxTrucks = await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxNumberOfTrucks);
                if (maxTrucks != null) return Convert.ToInt32(maxTrucks);
            }
            return null;
        }

        private async Task<bool> HaveAdditionalPrice(int tenantId)
        {
            var trucksFeature = await _featureChecker.IsEnabledAsync(tenantId, AppFeatures.NumberOfTrucks);
            if (trucksFeature)
            {
                return Convert.ToInt32(await _featureChecker.GetValueAsync(tenantId, AppFeatures.AdditionalTruckPrice)) > 0 ?true :false;
            }
            return false;
        }

        private async Task<int?> GetTrucksNo(int tenantId)
        {
            DisableTenancyFilters();
            return await _truckRepository.CountAsync(x => x.TenantId == tenantId);
        }

        private async Task FillIsMissingDocumentFiles(LoadResult pagedResultDto)
        {
            var ids = pagedResultDto.data.ToDynamicList<TruckDto>().Select(x => x.Id);
            var documentTypesCount = await _documentTypeRepository.GetAll()
                .Where(doc => doc.DocumentsEntityId == DocumentsEntitiesEnum.Truck)
                .Where(x => x.IsRequired)
                .CountAsync();

            if (documentTypesCount == 0)
                return;

            var submittedDocuments = await (_documentFileRepository.GetAll()
                    .Where(x => ids.Contains((long)x.TruckId))
                    .Where(x => x.DocumentTypeFk.IsRequired)
                    .GroupBy(x => x.TruckId)
                    .Select(x => new { TruckId = x.Key, IsMissingDocumentFiles = x.Count() == documentTypesCount }))
                .ToListAsync();

            foreach (TruckDto truckDto in pagedResultDto.data.ToDynamicList<TruckDto>())
            {
                if (submittedDocuments == null)
                    continue;


                var t = submittedDocuments.FirstOrDefault(x => x.TruckId == truckDto.Id);
                if (t != null)
                {
                    truckDto.IsMissingDocumentFiles = t.IsMissingDocumentFiles;
                }
                else
                {
                    truckDto.IsMissingDocumentFiles = true;
                }
            }
        }
        #endregion
    }
}