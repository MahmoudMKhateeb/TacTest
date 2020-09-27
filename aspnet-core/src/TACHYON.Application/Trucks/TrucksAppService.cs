using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Profile.Dto;
using TACHYON.Configuration;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Notifications;
using TACHYON.Storage;
using TACHYON.Trucks;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.Exporting;
using TACHYON.Trucks.TrucksTypes;
using GetAllForLookupTableInput = TACHYON.Trucks.Dtos.GetAllForLookupTableInput;

namespace TACHYON.Trucks
{
    [AbpAuthorize(AppPermissions.Pages_Trucks)]
    [RequiresFeature(AppFeatures.Carrier)]
    public class TrucksAppService : TACHYONAppServiceBase, ITrucksAppService
    {
        private const int MaxTruckPictureBytes = 5242880; //5MB
        private readonly IRepository<Truck, Guid> _truckRepository;
        private readonly ITrucksExcelExporter _trucksExcelExporter;
        private readonly IRepository<TrucksType, long> _lookup_trucksTypeRepository;
        private readonly IRepository<TruckStatus, long> _lookup_truckStatusRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IAppNotifier _appNotifier;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly DocumentFilesAppService _documentFilesAppService;




        public TrucksAppService(IRepository<Truck, Guid> truckRepository, ITrucksExcelExporter trucksExcelExporter, IRepository<TrucksType, long> lookup_trucksTypeRepository, IRepository<TruckStatus, long> lookup_truckStatusRepository, IRepository<User, long> lookup_userRepository, IAppNotifier appNotifier, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager, IRepository<DocumentType, long> documentTypeRepository, DocumentFilesAppService documentFilesAppService)
        {
            _truckRepository = truckRepository;
            _trucksExcelExporter = trucksExcelExporter;
            _lookup_trucksTypeRepository = lookup_trucksTypeRepository;
            _lookup_truckStatusRepository = lookup_truckStatusRepository;
            _lookup_userRepository = lookup_userRepository;
            _appNotifier = appNotifier;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _documentTypeRepository = documentTypeRepository;
            _documentFilesAppService = documentFilesAppService;
        }

        public async Task<PagedResultDto<GetTruckForViewDto>> GetAll(GetAllTrucksInput input)
        {

            var filteredTrucks = _truckRepository.GetAll()
                        .Include(e => e.TrucksTypeFk)
                        .Include(e => e.TruckStatusFk)
                        .Include(e => e.Driver1UserFk)
                        .Include(e => e.Driver2UserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.PlateNumber.Contains(input.Filter) || e.ModelName.Contains(input.Filter) || e.ModelYear.Contains(input.Filter) || e.LicenseNumber.Contains(input.Filter) || e.Note.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter), e => e.PlateNumber == input.PlateNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ModelNameFilter), e => e.ModelName == input.ModelNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ModelYearFilter), e => e.ModelYear == input.ModelYearFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LicenseNumberFilter), e => e.LicenseNumber == input.LicenseNumberFilter)
                        .WhereIf(input.MinLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate >= input.MinLicenseExpirationDateFilter)
                        .WhereIf(input.MaxLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate <= input.MaxLicenseExpirationDateFilter)
                        .WhereIf(input.IsAttachableFilter > -1, e => (input.IsAttachableFilter == 1 && e.IsAttachable) || (input.IsAttachableFilter == 0 && !e.IsAttachable))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckStatusDisplayNameFilter), e => e.TruckStatusFk != null && e.TruckStatusFk.DisplayName == input.TruckStatusDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.Driver1UserFk != null && e.Driver1UserFk.Name == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserName2Filter), e => e.Driver2UserFk != null && e.Driver2UserFk.Name == input.UserName2Filter);

            var pagedAndFilteredTrucks = filteredTrucks
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var trucks = from o in pagedAndFilteredTrucks
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_truckStatusRepository.GetAll() on o.TruckStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_userRepository.GetAll() on o.Driver1UserId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_userRepository.GetAll() on o.Driver2UserId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetTruckForViewDto()
                         {
                             Truck = new TruckDto
                             {
                                 PlateNumber = o.PlateNumber,
                                 ModelName = o.ModelName,
                                 ModelYear = o.ModelYear,
                                 LicenseNumber = o.LicenseNumber,
                                 LicenseExpirationDate = o.LicenseExpirationDate,
                                 IsAttachable = o.IsAttachable,
                                 Note = o.Note,
                                 Id = o.Id,
                                 RentPrice = o.RentPrice,
                                 RentDuration = o.RentDuration
                             },
                             TrucksTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                             TruckStatusDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             UserName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),
                             UserName2 = s4 == null || s4.Name == null ? "" : s4.Name.ToString()
                         };

            var totalCount = await filteredTrucks.CountAsync();

            return new PagedResultDto<GetTruckForViewDto>(
                totalCount,
                await trucks.ToListAsync()
            );
        }

        public async Task<GetTruckForViewDto> GetTruckForView(Guid id)
        {
            var truck = await _truckRepository.GetAsync(id);

            var output = new GetTruckForViewDto { Truck = ObjectMapper.Map<TruckDto>(truck) };

            if (output.Truck.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync(output.Truck.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.Truck.TruckStatusId != null)
            {
                var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync(output.Truck.TruckStatusId);
                output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            }

            if (output.Truck.Driver1UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Truck.Driver1UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (output.Truck.Driver2UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Truck.Driver2UserId);
                output.UserName2 = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Edit)]
        public async Task<GetTruckForEditOutput> GetTruckForEdit(EntityDto<Guid> input)
        {
            var truck = await _truckRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckForEditOutput { Truck = ObjectMapper.Map<CreateOrEditTruckDto>(truck) };

            if (output.Truck.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync(output.Truck.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

            if (output.Truck.TruckStatusId != null)
            {
                var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync(output.Truck.TruckStatusId);
                output.TruckStatusDisplayName = _lookupTruckStatus?.DisplayName?.ToString();
            }

            if (output.Truck.Driver1UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Truck.Driver1UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (output.Truck.Driver2UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.Truck.Driver2UserId);
                output.UserName2 = _lookupUser?.Name?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTruckDto input)
        {
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
            var truck = ObjectMapper.Map<Truck>(input);


            if (AbpSession.TenantId != null)
            {
                truck.TenantId = (int)AbpSession.TenantId;
            }

            var requiredDocs = await GetRequiredDocumentFileListForCreateOrEdit();
            if (requiredDocs.Count > 0)
            {
                foreach (var item in requiredDocs)
                {
                    var doc = input.CreateOrEditDocumentFileDtos
                        .FirstOrDefault(x => x.DocumentTypeId == item.DocumentTypeId);

                    if (doc.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L("document missing msg :" + item.Name));
                    }

                    doc.Name = item.Name;
                }
            }


            var truckId = await _truckRepository.InsertAndGetIdAsync(truck);
            if (input.Driver1UserId != null)
            {
                await _appNotifier.AssignDriverToTruck(new UserIdentifier(AbpSession.TenantId, input.Driver1UserId.Value), truckId);
            }

            if (input.Driver2UserId != null)
            {
                await _appNotifier.AssignDriverToTruck(new UserIdentifier(AbpSession.TenantId, input.Driver2UserId.Value), truckId);
            }

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



        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Edit)]
        protected virtual async Task Update(CreateOrEditTruckDto input)
        {
            var truck = await _truckRepository.FirstOrDefaultAsync((Guid)input.Id);
            if (input.Driver1UserId.HasValue && input.Driver1UserId != truck.Driver1UserId)
            {
                await _appNotifier.AssignDriverToTruck(new UserIdentifier(AbpSession.TenantId, input.Driver1UserId.Value), truck.Id);
            }

            if (input.Driver2UserId.HasValue && input.Driver2UserId != truck.Driver2UserId)
            {
                await _appNotifier.AssignDriverToTruck(new UserIdentifier(AbpSession.TenantId, input.Driver2UserId.Value), truck.Id);
            }

            ObjectMapper.Map(input, truck);

            if (!input.UpdateTruckPictureInput.FileToken.IsNullOrEmpty())
            {
                if (truck.PictureId.HasValue)
                {
                    await _binaryObjectManager.DeleteAsync(truck.PictureId.Value);
                }

                truck.PictureId = await AddOrUpdateTruckPicture(input.UpdateTruckPictureInput);
            }

        }


        /// <summary>
        /// get list of required documents types to use in create truck
        /// </summary>
        /// <returns></returns>
        public async Task<List<CreateOrEditDocumentFileDto>> GetRequiredDocumentFileListForCreateOrEdit()
        {
            return await _documentTypeRepository.GetAll()
                 .Where(x => x.DocumentsEntityFk.DisplayName == AppConsts.TruckDocumentsEntityName)
                 .Select(x => new CreateOrEditDocumentFileDto
                 {
                     DocumentTypeId = x.Id,
                     Name = x.DisplayName,
                     IsRequired = x.IsRequired,
                     HasExpirationDate = x.HasExpirationDate
                 })
                 .ToListAsync();

        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _truckRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetTrucksToExcel(GetAllTrucksForExcelInput input)
        {

            var filteredTrucks = _truckRepository.GetAll()
                        .Include(e => e.TrucksTypeFk)
                        .Include(e => e.TruckStatusFk)
                        .Include(e => e.Driver1UserFk)
                        .Include(e => e.Driver2UserFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.PlateNumber.Contains(input.Filter) || e.ModelName.Contains(input.Filter) || e.ModelYear.Contains(input.Filter) || e.LicenseNumber.Contains(input.Filter) || e.Note.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter), e => e.PlateNumber == input.PlateNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ModelNameFilter), e => e.ModelName == input.ModelNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ModelYearFilter), e => e.ModelYear == input.ModelYearFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.LicenseNumberFilter), e => e.LicenseNumber == input.LicenseNumberFilter)
                        .WhereIf(input.MinLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate >= input.MinLicenseExpirationDateFilter)
                        .WhereIf(input.MaxLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate <= input.MaxLicenseExpirationDateFilter)
                        .WhereIf(input.IsAttachableFilter > -1, e => (input.IsAttachableFilter == 1 && e.IsAttachable) || (input.IsAttachableFilter == 0 && !e.IsAttachable))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckStatusDisplayNameFilter), e => e.TruckStatusFk != null && e.TruckStatusFk.DisplayName == input.TruckStatusDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.Driver1UserFk != null && e.Driver1UserFk.Name == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserName2Filter), e => e.Driver2UserFk != null && e.Driver2UserFk.Name == input.UserName2Filter);

            var query = (from o in filteredTrucks
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_truckStatusRepository.GetAll() on o.TruckStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_userRepository.GetAll() on o.Driver1UserId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_userRepository.GetAll() on o.Driver2UserId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         select new GetTruckForViewDto()
                         {
                             Truck = new TruckDto
                             {
                                 PlateNumber = o.PlateNumber,
                                 ModelName = o.ModelName,
                                 ModelYear = o.ModelYear,
                                 LicenseNumber = o.LicenseNumber,
                                 LicenseExpirationDate = o.LicenseExpirationDate,
                                 IsAttachable = o.IsAttachable,
                                 Note = o.Note,
                                 Id = o.Id
                             },
                             TrucksTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                             TruckStatusDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             UserName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),
                             UserName2 = s4 == null || s4.Name == null ? "" : s4.Name.ToString()
                         });


            var truckListDtos = await query.ToListAsync();

            return _trucksExcelExporter.ExportToFile(truckListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_Trucks)]
        public async Task<List<TruckTrucksTypeLookupTableDto>> GetAllTrucksTypeForTableDropdown()
        {
            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                return await _lookup_trucksTypeRepository.GetAll()
                   .Select(trucksType => new TruckTrucksTypeLookupTableDto
                   {
                       Id = trucksType.Id.ToString(),
                       DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString()
                   }).ToListAsync();
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks)]
        public async Task<List<TruckTruckStatusLookupTableDto>> GetAllTruckStatusForTableDropdown()
        {
            return await _lookup_truckStatusRepository.GetAll()
                .Select(truckStatus => new TruckTruckStatusLookupTableDto
                {
                    Id = truckStatus.Id.ToString(),
                    DisplayName = truckStatus == null || truckStatus.DisplayName == null ? "" : truckStatus.DisplayName.ToString()
                }).ToListAsync();
        }


        [AbpAuthorize(AppPermissions.Pages_Trucks)]
        public async Task<PagedResultDto<TruckUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
        {
            var query = _lookup_userRepository.GetAll().WhereIf(
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
                    DisplayName = user.Name?.ToString()
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
        public async Task<string> GetPictureContentForTruck(Guid truckId)
        {
            var truck = await _truckRepository.GetAsync(truckId);
            if (truck.PictureId == null)
            {
                return "";
            }

            var file = await _binaryObjectManager.GetOrNullAsync(truck.PictureId.Value);
            return file == null ? "" : Convert.ToBase64String(file.Bytes);
        }

    }
}