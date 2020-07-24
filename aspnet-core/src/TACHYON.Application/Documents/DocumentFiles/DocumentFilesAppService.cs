using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentFiles.Exporting;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Dto;
using TACHYON.Routs.RoutSteps;
using TACHYON.Storage;
using TACHYON.Trailers;
using TACHYON.Trucks;

namespace TACHYON.Documents.DocumentFiles
{
    [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
    public class DocumentFilesAppService : TACHYONAppServiceBase, IDocumentFilesAppService
    {
        private const int MaxDocumentFileBytes = 5242880; //5MB
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IDocumentFilesExcelExporter _documentFilesExcelExporter;
        private readonly IRepository<DocumentType, long> _lookup_documentTypeRepository;
        private readonly IRepository<Truck, Guid> _lookup_truckRepository;
        private readonly IRepository<Trailer, long> _lookup_trailerRepository;
        private readonly IRepository<User, long> _lookup_userRepository;
        private readonly IRepository<RoutStep, long> _lookup_routStepRepository;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;



        public DocumentFilesAppService(IRepository<DocumentFile, Guid> documentFileRepository, IDocumentFilesExcelExporter documentFilesExcelExporter, IRepository<DocumentType, long> lookup_documentTypeRepository, IRepository<Truck, Guid> lookup_truckRepository, IRepository<Trailer, long> lookup_trailerRepository, IRepository<User, long> lookup_userRepository, IRepository<RoutStep, long> lookup_routStepRepository, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager)
        {
            _documentFileRepository = documentFileRepository;
            _documentFilesExcelExporter = documentFilesExcelExporter;
            _lookup_documentTypeRepository = lookup_documentTypeRepository;
            _lookup_truckRepository = lookup_truckRepository;
            _lookup_trailerRepository = lookup_trailerRepository;
            _lookup_userRepository = lookup_userRepository;
            _lookup_routStepRepository = lookup_routStepRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
        }

        public async Task<PagedResultDto<GetDocumentFileForViewDto>> GetAll(GetAllDocumentFilesInput input)
        {

            var filteredDocumentFiles = _documentFileRepository.GetAll()
                        .Include(e => e.DocumentTypeFk)
                        .Include(e => e.TruckFk)
                        .Include(e => e.TrailerFk)
                        .Include(e => e.UserFk)
                        .Include(e => e.RoutStepFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Extn.Contains(input.Filter) || e.IsAccepted.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ExtnFilter), e => e.Extn == input.ExtnFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BinaryObjectIdFilter.ToString()), e => e.BinaryObjectId.ToString() == input.BinaryObjectIdFilter.ToString())
                        .WhereIf(input.MinExpirationDateFilter != null, e => e.ExpirationDate >= input.MinExpirationDateFilter)
                        .WhereIf(input.MaxExpirationDateFilter != null, e => e.ExpirationDate <= input.MaxExpirationDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IsAcceptedFilter), e => e.IsAccepted == input.IsAcceptedFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentTypeDisplayNameFilter), e => e.DocumentTypeFk != null && e.DocumentTypeFk.DisplayName == input.DocumentTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckPlateNumberFilter), e => e.TruckFk != null && e.TruckFk.PlateNumber == input.TruckPlateNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTrailerCodeFilter), e => e.TrailerFk != null && e.TrailerFk.TrailerCode == input.TrailerTrailerCodeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RoutStepDisplayNameFilter), e => e.RoutStepFk != null && e.RoutStepFk.DisplayName == input.RoutStepDisplayNameFilter);

            var pagedAndFilteredDocumentFiles = filteredDocumentFiles
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var documentFiles = from o in pagedAndFilteredDocumentFiles
                                join o1 in _lookup_documentTypeRepository.GetAll() on o.DocumentTypeId equals o1.Id into j1
                                from s1 in j1.DefaultIfEmpty()

                                join o2 in _lookup_truckRepository.GetAll() on o.TruckId equals o2.Id into j2
                                from s2 in j2.DefaultIfEmpty()

                                join o3 in _lookup_trailerRepository.GetAll() on o.TrailerId equals o3.Id into j3
                                from s3 in j3.DefaultIfEmpty()

                                join o4 in _lookup_userRepository.GetAll() on o.UserId equals o4.Id into j4
                                from s4 in j4.DefaultIfEmpty()

                                join o5 in _lookup_routStepRepository.GetAll() on o.RoutStepId equals o5.Id into j5
                                from s5 in j5.DefaultIfEmpty()

                                select new GetDocumentFileForViewDto()
                                {
                                    DocumentFile = new DocumentFileDto
                                    {
                                        Name = o.Name,
                                        Extn = o.Extn,
                                        BinaryObjectId = o.BinaryObjectId,
                                        ExpirationDate = o.ExpirationDate,
                                        IsAccepted = o.IsAccepted,
                                        Id = o.Id
                                    },
                                    DocumentTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                                    TruckPlateNumber = s2 == null || s2.PlateNumber == null ? "" : s2.PlateNumber.ToString(),
                                    TrailerTrailerCode = s3 == null || s3.TrailerCode == null ? "" : s3.TrailerCode.ToString(),
                                    UserName = s4 == null || s4.Name == null ? "" : s4.Name.ToString(),
                                    RoutStepDisplayName = s5 == null || s5.DisplayName == null ? "" : s5.DisplayName.ToString()
                                };

            var totalCount = await filteredDocumentFiles.CountAsync();

            return new PagedResultDto<GetDocumentFileForViewDto>(
                totalCount,
                await documentFiles.ToListAsync()
            );
        }

        public async Task<GetDocumentFileForViewDto> GetDocumentFileForView(Guid id)
        {
            var documentFile = await _documentFileRepository.GetAsync(id);

            var output = new GetDocumentFileForViewDto { DocumentFile = ObjectMapper.Map<DocumentFileDto>(documentFile) };

            if (output.DocumentFile.DocumentTypeId != null)
            {
                var _lookupDocumentType = await _lookup_documentTypeRepository.FirstOrDefaultAsync((long)output.DocumentFile.DocumentTypeId);
                output.DocumentTypeDisplayName = _lookupDocumentType?.DisplayName?.ToString();
            }

            if (output.DocumentFile.TruckId != null)
            {
                var _lookupTruck = await _lookup_truckRepository.FirstOrDefaultAsync((Guid)output.DocumentFile.TruckId);
                output.TruckPlateNumber = _lookupTruck?.PlateNumber?.ToString();
            }

            if (output.DocumentFile.TrailerId != null)
            {
                var _lookupTrailer = await _lookup_trailerRepository.FirstOrDefaultAsync((long)output.DocumentFile.TrailerId);
                output.TrailerTrailerCode = _lookupTrailer?.TrailerCode?.ToString();
            }

            if (output.DocumentFile.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.DocumentFile.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (output.DocumentFile.RoutStepId != null)
            {
                var _lookupRoutStep = await _lookup_routStepRepository.FirstOrDefaultAsync((long)output.DocumentFile.RoutStepId);
                output.RoutStepDisplayName = _lookupRoutStep?.DisplayName?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Edit)]
        public async Task<GetDocumentFileForEditOutput> GetDocumentFileForEdit(EntityDto<Guid> input)
        {
            var documentFile = await _documentFileRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetDocumentFileForEditOutput { DocumentFile = ObjectMapper.Map<CreateOrEditDocumentFileDto>(documentFile) };

            if (output.DocumentFile.DocumentTypeId != null)
            {
                var _lookupDocumentType = await _lookup_documentTypeRepository.FirstOrDefaultAsync((long)output.DocumentFile.DocumentTypeId);
                output.DocumentTypeDisplayName = _lookupDocumentType?.DisplayName?.ToString();
            }

            if (output.DocumentFile.TruckId != null)
            {
                var _lookupTruck = await _lookup_truckRepository.FirstOrDefaultAsync((Guid)output.DocumentFile.TruckId);
                output.TruckPlateNumber = _lookupTruck?.PlateNumber?.ToString();
            }

            if (output.DocumentFile.TrailerId != null)
            {
                var _lookupTrailer = await _lookup_trailerRepository.FirstOrDefaultAsync((long)output.DocumentFile.TrailerId);
                output.TrailerTrailerCode = _lookupTrailer?.TrailerCode?.ToString();
            }

            if (output.DocumentFile.UserId != null)
            {
                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.DocumentFile.UserId);
                output.UserName = _lookupUser?.Name?.ToString();
            }

            if (output.DocumentFile.RoutStepId != null)
            {
                var _lookupRoutStep = await _lookup_routStepRepository.FirstOrDefaultAsync((long)output.DocumentFile.RoutStepId);
                output.RoutStepDisplayName = _lookupRoutStep?.DisplayName?.ToString();
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditDocumentFileDto input)
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

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Create)]
        protected virtual async Task Create(CreateOrEditDocumentFileDto input)
        {
            var documentFile = ObjectMapper.Map<DocumentFile>(input);


            if (AbpSession.TenantId != null)
            {
                documentFile.TenantId = (int?)AbpSession.TenantId;
            }

            if (!input.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
            {
                documentFile.BinaryObjectId = await AddOrUpdateDocumentFile(input.UpdateDocumentFileInput);
            }

            await _documentFileRepository.InsertAsync(documentFile);
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Edit)]
        protected virtual async Task Update(CreateOrEditDocumentFileDto input)
        {
            var documentFile = await _documentFileRepository.FirstOrDefaultAsync((Guid)input.Id);
            ObjectMapper.Map(input, documentFile);

            if (!input.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
            {
                await _binaryObjectManager.DeleteAsync(input.BinaryObjectId);
                documentFile.BinaryObjectId = await AddOrUpdateDocumentFile(input.UpdateDocumentFileInput);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            await _documentFileRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetDocumentFilesToExcel(GetAllDocumentFilesForExcelInput input)
        {

            var filteredDocumentFiles = _documentFileRepository.GetAll()
                        .Include(e => e.DocumentTypeFk)
                        .Include(e => e.TruckFk)
                        .Include(e => e.TrailerFk)
                        .Include(e => e.UserFk)
                        .Include(e => e.RoutStepFk)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Extn.Contains(input.Filter) || e.IsAccepted.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.ExtnFilter), e => e.Extn == input.ExtnFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.BinaryObjectIdFilter.ToString()), e => e.BinaryObjectId.ToString() == input.BinaryObjectIdFilter.ToString())
                        .WhereIf(input.MinExpirationDateFilter != null, e => e.ExpirationDate >= input.MinExpirationDateFilter)
                        .WhereIf(input.MaxExpirationDateFilter != null, e => e.ExpirationDate <= input.MaxExpirationDateFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.IsAcceptedFilter), e => e.IsAccepted == input.IsAcceptedFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentTypeDisplayNameFilter), e => e.DocumentTypeFk != null && e.DocumentTypeFk.DisplayName == input.DocumentTypeDisplayNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckPlateNumberFilter), e => e.TruckFk != null && e.TruckFk.PlateNumber == input.TruckPlateNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTrailerCodeFilter), e => e.TrailerFk != null && e.TrailerFk.TrailerCode == input.TrailerTrailerCodeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.RoutStepDisplayNameFilter), e => e.RoutStepFk != null && e.RoutStepFk.DisplayName == input.RoutStepDisplayNameFilter);

            var query = (from o in filteredDocumentFiles
                         join o1 in _lookup_documentTypeRepository.GetAll() on o.DocumentTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()

                         join o2 in _lookup_truckRepository.GetAll() on o.TruckId equals o2.Id into j2
                         from s2 in j2.DefaultIfEmpty()

                         join o3 in _lookup_trailerRepository.GetAll() on o.TrailerId equals o3.Id into j3
                         from s3 in j3.DefaultIfEmpty()

                         join o4 in _lookup_userRepository.GetAll() on o.UserId equals o4.Id into j4
                         from s4 in j4.DefaultIfEmpty()

                         join o5 in _lookup_routStepRepository.GetAll() on o.RoutStepId equals o5.Id into j5
                         from s5 in j5.DefaultIfEmpty()

                         select new GetDocumentFileForViewDto()
                         {
                             DocumentFile = new DocumentFileDto
                             {
                                 Name = o.Name,
                                 Extn = o.Extn,
                                 BinaryObjectId = o.BinaryObjectId,
                                 ExpirationDate = o.ExpirationDate,
                                 IsAccepted = o.IsAccepted,
                                 Id = o.Id
                             },
                             DocumentTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString(),
                             TruckPlateNumber = s2 == null || s2.PlateNumber == null ? "" : s2.PlateNumber.ToString(),
                             TrailerTrailerCode = s3 == null || s3.TrailerCode == null ? "" : s3.TrailerCode.ToString(),
                             UserName = s4 == null || s4.Name == null ? "" : s4.Name.ToString(),
                             RoutStepDisplayName = s5 == null || s5.DisplayName == null ? "" : s5.DisplayName.ToString()
                         });


            var documentFileListDtos = await query.ToListAsync();

            return _documentFilesExcelExporter.ExportToFile(documentFileListDtos);
        }


        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileDocumentTypeLookupTableDto>> GetAllDocumentTypeForTableDropdown()
        {
            return await _lookup_documentTypeRepository.GetAll()
                .Select(documentType => new DocumentFileDocumentTypeLookupTableDto
                {
                    Id = documentType.Id,
                    DisplayName = documentType == null || documentType.DisplayName == null ? "" : documentType.DisplayName.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileTruckLookupTableDto>> GetAllTruckForTableDropdown()
        {
            return await _lookup_truckRepository.GetAll()
                .Select(truck => new DocumentFileTruckLookupTableDto
                {
                    Id = truck.Id.ToString(),
                    DisplayName = truck == null || truck.PlateNumber == null ? "" : truck.PlateNumber.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileTrailerLookupTableDto>> GetAllTrailerForTableDropdown()
        {
            return await _lookup_trailerRepository.GetAll()
                .Select(trailer => new DocumentFileTrailerLookupTableDto
                {
                    Id = trailer.Id,
                    DisplayName = trailer == null || trailer.TrailerCode == null ? "" : trailer.TrailerCode.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileUserLookupTableDto>> GetAllUserForTableDropdown()
        {
            return await _lookup_userRepository.GetAll()
                .Select(user => new DocumentFileUserLookupTableDto
                {
                    Id = user.Id,
                    DisplayName = user == null || user.Name == null ? "" : user.Name.ToString()
                }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileRoutStepLookupTableDto>> GetAllRoutStepForTableDropdown()
        {
            return await _lookup_routStepRepository.GetAll()
                .Select(routStep => new DocumentFileRoutStepLookupTableDto
                {
                    Id = routStep.Id,
                    DisplayName = routStep == null || routStep.DisplayName == null ? "" : routStep.DisplayName.ToString()
                }).ToListAsync();
        }

        protected virtual async Task<Guid> AddOrUpdateDocumentFile(UpdateDocumentFileInput input)
        {

            var fileBytes = _tempFileCacheManager.GetFile(input.FileToken);

            if (fileBytes == null)
            {
                throw new UserFriendlyException("There is no such document file with the token: " + input.FileToken);
            }

            if (fileBytes.Length > MaxDocumentFileBytes)
            {
                throw new UserFriendlyException(L("DocumentFile_Warn_SizeLimit", AppConsts.MaxDocumentFileBytesUserFriendlyValue));

            }

            var storedFile = new BinaryObject(AbpSession.TenantId, fileBytes);
            await _binaryObjectManager.SaveAsync(storedFile);

            return storedFile.Id;
        }


    }
}