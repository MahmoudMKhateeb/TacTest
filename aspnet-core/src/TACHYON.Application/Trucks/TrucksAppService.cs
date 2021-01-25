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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
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
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TrucksTypes;
using GetAllForLookupTableInput = TACHYON.Trucks.Dtos.GetAllForLookupTableInput;

namespace TACHYON.Trucks
{
    [AbpAuthorize(AppPermissions.Pages_Trucks)]
    [RequiresFeature(AppFeatures.Carrier)]
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




        public TrucksAppService(IRepository<DocumentType, long> documentTypeRepository, IRepository<DocumentFile, Guid> documentFileRepository, IRepository<Truck, long> truckRepository, ITrucksExcelExporter trucksExcelExporter, IRepository<TrucksType, long> lookup_trucksTypeRepository, IRepository<TruckStatus, long> lookup_truckStatusRepository, IRepository<User, long> lookup_userRepository, IAppNotifier appNotifier, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager, DocumentFilesAppService documentFilesAppService, IRepository<TransportType, int> transportTypeRepository, IRepository<Capacity, int> capacityRepository)
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
        }

        public async Task<PagedResultDto<GetTruckForViewDto>> GetAll(GetAllTrucksInput input)
        {

            var filteredTrucks = _truckRepository.GetAll()
                .Include(e => e.TruckStatusFk)
                //.Include(e => e.Driver1UserFk)
                //truck type related
                .Include(e => e.TrucksTypeFk)
                .Include(e => e.TransportTypeFk)
                .Include(e => e.CapacityFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.PlateNumber.Contains(input.Filter) ||
                e.ModelName.Contains(input.Filter) || e.ModelYear.Contains(input.Filter) || e.Note.Contains(input.Filter))
                //e.Driver1UserFk.Name.Contains(input.Filter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter), e => e.PlateNumber == input.PlateNumberFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.ModelNameFilter), e => e.ModelName == input.ModelNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.ModelYearFilter), e => e.ModelYear == input.ModelYearFilter)
                //.WhereIf(input.IsAttachableFilter > -1, e => (input.IsAttachableFilter == 1 && e.IsAttachable) || (input.IsAttachableFilter == 0 && !e.IsAttachable))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.TruckStatusDisplayNameFilter), e => e.TruckStatusFk != null && e.TruckStatusFk.DisplayName == input.TruckStatusDisplayNameFilter);
            //.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.Driver1UserFk != null && e.Driver1UserFk.Name == input.UserNameFilter);

            var pagedAndFilteredTrucks = filteredTrucks
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var documentTypesCount = await _documentTypeRepository.GetAll().Include(ent => ent.DocumentsEntityFk)
                .Where(a => a.DocumentsEntityFk.DisplayName == AppConsts.TruckDocumentsEntityName).CountAsync();


            var trucks = from o in pagedAndFilteredTrucks
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_truckStatusRepository.GetAll() on o.TruckStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                             //join o3 in _lookup_userRepository.GetAll() on o.Driver1UserId equals o3.Id into j3
                             //from s3 in j3.DefaultIfEmpty()

                         select new GetTruckForViewDto()
                         {
                             Truck = new TruckDto
                             {
                                 PlateNumber = o == null ? "" : o.PlateNumber,
                                 ModelName = o.ModelName,
                                 ModelYear = o.ModelYear,
                                 Note = o.Note,
                                 Id = o.Id,
                                 Capacity = o.Capacity,
                                 Length = o.Length
                             },
                             TrucksTypeDisplayName =
                             (o.TransportTypeFk == null ? "" : o.TransportTypeFk.DisplayName) + " - " +
                             (o.TrucksTypeFk == null ? "" : o.TrucksTypeFk.DisplayName) + " - " +
                             (o.CapacityFk == null ? "" : o.CapacityFk.DisplayName),
                             TruckStatusDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             //UserName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),
                             IsMissingDocumentFiles = documentTypesCount != _documentFileRepository.GetAll().Where(t => t.TruckId == o.Id).Count()
                         };

            var totalCount = await filteredTrucks.CountAsync();
            var result = await trucks.ToListAsync();
            return new PagedResultDto<GetTruckForViewDto>(
                totalCount,
                result
            );
        }

        public async Task<GetTruckForViewDto> GetTruckForView(long id)
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



            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Edit)]
        public async Task<GetTruckForEditOutput> GetTruckForEdit(EntityDto<long> input)
        {
            var truck = await _truckRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTruckForEditOutput { Truck = ObjectMapper.Map<CreateOrEditTruckDto>(truck) };

            if (output.Truck.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((long)output.Truck.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
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
            var truck = ObjectMapper.Map<Truck>(input);


            if (AbpSession.TenantId != null)
            {
                truck.TenantId = (int)AbpSession.TenantId;
            }

            var requiredDocs = await _documentFilesAppService.GetTruckRequiredDocumentFiles("");
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

                    doc.Name = item.DocumentTypeDto.DisplayName;
                }
            }


            var truckId = await _truckRepository.InsertAndGetIdAsync(truck);
            //if (input.Driver1UserId != null)
            //{
            //    try
            //    {
            //    await _appNotifier.AssignDriverToTruck(new UserIdentifier(AbpSession.TenantId, input.Driver1UserId.Value), truckId);

            //    }
            //    catch (Exception ex)
            //    {

            //        throw;
            //    }
            //}


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
            var truck = await _truckRepository.FirstOrDefaultAsync(input.Id.Value);
            //if (input.Driver1UserId.HasValue && input.Driver1UserId != truck.Driver1UserId)
            //{
            //    await _appNotifier.AssignDriverToTruck(new UserIdentifier(AbpSession.TenantId, input.Driver1UserId.Value), truck.Id);
            //}


            ObjectMapper.Map(input, truck);

            //if (!input.UpdateTruckPictureInput.FileToken.IsNullOrEmpty())
            //{
            //    if (truck.PictureId.HasValue)
            //    {
            //        await _binaryObjectManager.DeleteAsync(truck.PictureId.Value);
            //    }

            //    truck.PictureId = await AddOrUpdateTruckPicture(input.UpdateTruckPictureInput);
            //}

        }

        [AbpAuthorize(AppPermissions.Pages_Trucks_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            await _truckRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetTrucksToExcel(GetAllTrucksForExcelInput input)
        {

            var filteredTrucks = _truckRepository.GetAll()
                .Include(e => e.TrucksTypeFk)
                .Include(e => e.TruckStatusFk)
                .Include(x => x.TransportTypeFk)
                //.Include(e => e.Driver1UserFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.PlateNumber.Contains(input.Filter) || e.ModelName.Contains(input.Filter) || e.ModelYear.Contains(input.Filter) || e.Note.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter), e => e.PlateNumber == input.PlateNumberFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.ModelNameFilter), e => e.ModelName == input.ModelNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.ModelYearFilter), e => e.ModelYear == input.ModelYearFilter)
                //.WhereIf(input.IsAttachableFilter > -1, e => (input.IsAttachableFilter == 1 && e.IsAttachable) || (input.IsAttachableFilter == 0 && !e.IsAttachable))
                .WhereIf(!string.IsNullOrWhiteSpace(input.TrucksTypeDisplayNameFilter), e => e.TrucksTypeFk != null && e.TrucksTypeFk.DisplayName == input.TrucksTypeDisplayNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.TruckStatusDisplayNameFilter), e => e.TruckStatusFk != null && e.TruckStatusFk.DisplayName == input.TruckStatusDisplayNameFilter);
            //.WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.Driver1UserFk != null && e.Driver1UserFk.Name == input.UserNameFilter);

            var query = (from o in filteredTrucks
                         join o1 in _lookup_trucksTypeRepository.GetAll() on o.TrucksTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_truckStatusRepository.GetAll() on o.TruckStatusId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                             //join o3 in _lookup_userRepository.GetAll() on o.Driver1UserId equals o3.Id into j3
                             //from s3 in j3.DefaultIfEmpty()


                         select new GetTruckForViewDto()
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
                             (o.TransportTypeFk == null ? "" : o.TransportTypeFk.DisplayName) + " - " +
                             (o.TrucksTypeFk == null ? "" : o.TrucksTypeFk.DisplayName) + " - " +
                             (o.CapacityFk == null ? "" : o.CapacityFk.DisplayName),
                             TruckStatusDisplayName = s2 == null || s2.DisplayName == null ? "" : s2.DisplayName.ToString(),
                             //UserName = s3 == null || s3.Name == null ? "" : s3.Name.ToString(),

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
        public async Task<IEnumerable<ISelectItemDto>> GetAllTruckStatusForTableDropdown()
        {
            List<TruckStatus> trucksStatuses = await _lookup_truckStatusRepository
                .GetAllIncluding(x => x.Translations)
                .ToListAsync();

            List<TruckStatusSelectItemDto> truckStatusDto =
                ObjectMapper.Map<List<TruckStatusSelectItemDto>>(trucksStatuses);

            return truckStatusDto;

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


        public async Task<List<SelectItemDto>> GetAllTruckTypesByTransportTypeIdForDropdown(int transportTypeId)
        {


            return await _lookup_trucksTypeRepository.GetAll()
                .Where(x => x.TransportTypeId == transportTypeId)
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }
        public async Task<List<SelectItemDto>> GetAllTuckCapacitiesByTuckTypeIdForDropdown(int truckTypeId)
        {
            return await _capacityRepository.GetAll()
                .Where(x => x.TrucksTypeId == truckTypeId)
                .Select(x => new SelectItemDto()
                {
                    Id = x.Id.ToString(),
                    DisplayName = x.DisplayName
                }).ToListAsync();
        }

        #endregion
    }
}