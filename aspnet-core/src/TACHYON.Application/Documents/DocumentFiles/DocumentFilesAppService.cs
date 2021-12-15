using Abp;
using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using DevExtreme.AspNet.Data.ResponseModel;
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
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Dto;
using TACHYON.MultiTenancy;
using TACHYON.Notifications;
using TACHYON.Routs.RoutSteps;
using TACHYON.Storage;
using TACHYON.Trailers;
using TACHYON.Trucks;
using TACHYON.Url;


namespace TACHYON.Documents.DocumentFiles
{
    [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
    public class DocumentFilesAppService : TACHYONAppServiceBase, IDocumentFilesAppService
    {

        private readonly IRepository<Tenant, int> _lookupTenantRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IDocumentFilesExcelExporter _documentFilesExcelExporter;
        private readonly IRepository<DocumentType, long> _lookupDocumentTypeRepository;
        private readonly IRepository<Truck, long> _lookupTruckRepository;
        private readonly IRepository<Trailer, long> _lookupTrailerRepository;
        private readonly IRepository<User, long> _lookupUserRepository;
        private readonly IRepository<RoutStep, long> _lookupRoutStepRepository;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IRepository<DocumentsEntity, int> _documentEntityRepository;
        private readonly IRepository<Edition, int> _editionRepository;
        private readonly DocumentFilesManager _documentFilesManager;
        private readonly TenantManager _tenantManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IUserEmailer _userEmailer;

        public IAppUrlService _appUrlService { get; set; }

        public DocumentFilesAppService(TenantManager tenantManager,
            IRepository<DocumentFile, Guid> documentFileRepository,
            IDocumentFilesExcelExporter documentFilesExcelExporter,
            IRepository<DocumentType, long> lookupDocumentTypeRepository,
            IRepository<Truck, long> lookupTruckRepository,
            IRepository<Trailer, long> lookupTrailerRepository,
            IRepository<User, long> lookupUserRepository,
            IRepository<RoutStep, long> lookupRoutStepRepository,
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager,
            IRepository<Edition, int> editionRepository,
            IRepository<DocumentType, long> documentTypeRepository,
            DocumentFilesManager documentFilesManager,
            IRepository<Tenant, int> lookupTenantRepository,
            IRepository<DocumentsEntity, int> documentEntityRepository,
            IAppNotifier appNotifier,
            IUserEmailer userEmailer)
        {
            _documentFileRepository = documentFileRepository;
            _documentFilesExcelExporter = documentFilesExcelExporter;
            _lookupDocumentTypeRepository = lookupDocumentTypeRepository;
            _lookupTruckRepository = lookupTruckRepository;
            _lookupTrailerRepository = lookupTrailerRepository;
            _lookupUserRepository = lookupUserRepository;
            _lookupRoutStepRepository = lookupRoutStepRepository;
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _editionRepository = editionRepository;
            _documentTypeRepository = documentTypeRepository;
            _documentFilesManager = documentFilesManager;
            _lookupTenantRepository = lookupTenantRepository;
            _documentEntityRepository = documentEntityRepository;
            _tenantManager = tenantManager;
            _appNotifier = appNotifier;
            _userEmailer = userEmailer;
            _appUrlService = NullAppUrlService.Instance;
        }


        public async Task<LoadResult> GetAllTenantsSubmittedDocuments(GetAllForListDocumentFilesInput input)
        {
            DisableTenancyFiltersIfHost();

            var filteredDocumentFiles = _documentFileRepository.GetAll()
                .Where(e => e.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Tenant)
                .ProjectTo<GetAllTenantsSubmittedDocumentsDto>(AutoMapperConfigurationProvider);

            var result = await LoadResultAsync(filteredDocumentFiles, input.Filter);
            return result;
        }
        public async Task<LoadResult> GetAllTrucksSubmittedDocuments(GetAllTrucksSubmittedDocumentsInput input)
        {
            DisableTenancyFiltersIfHost();

            var filteredDocumentFiles = _documentFileRepository.GetAll()
                .Where(e => e.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Truck)
                .WhereIf(input.TruckId.HasValue, x => x.TruckId == input.TruckId)
                .ProjectTo<GetAllTrucksSubmittedDocumentsDto>(AutoMapperConfigurationProvider);

            var result = await LoadResultAsync(filteredDocumentFiles, input.Filter);
            return result;
        }
        public async Task<LoadResult> GetAllDriversSubmittedDocuments(GetAllDriversSubmittedDocumentsInput input)
        {

            DisableTenancyFiltersIfHost();
            await DisableTenancyFiltersIfTachyonDealer();

            var filteredDocumentFiles = _documentFileRepository.GetAll()
                .Where(e => e.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Driver)
                .WhereIf(input.DriverId.HasValue, x => x.UserId == input.DriverId)
                .ProjectTo<GetAllDriversSubmittedDocumentsDto>(AutoMapperConfigurationProvider);

            var result = await LoadResultAsync(filteredDocumentFiles, input.Filter);

            return result;
        }
        public async Task<GetDocumentFileForViewDto> GetDocumentFileForView(Guid id)
        {
            var documentFile = await _documentFileRepository.GetAsync(id);

            var output = new GetDocumentFileForViewDto { DocumentFile = ObjectMapper.Map<DocumentFileDto>(documentFile) };

            {
                var lookupDocumentType = await _lookupDocumentTypeRepository.FirstOrDefaultAsync(output.DocumentFile.DocumentTypeId);
                output.DocumentType = ObjectMapper.Map<DocumentTypeDto>(lookupDocumentType);
            }

            if (output.DocumentFile.TruckId != null)
            {
                var lookupTruck = await _lookupTruckRepository.FirstOrDefaultAsync((long)output.DocumentFile.TruckId);
                output.TruckId = (lookupTruck == null ? (long?)null : lookupTruck.Id).ToString();
            }

            if (output.DocumentFile.TrailerId != null)
            {
                var lookupTrailer = await _lookupTrailerRepository.FirstOrDefaultAsync((long)output.DocumentFile.TrailerId);
                output.TrailerTrailerCode = lookupTrailer?.TrailerCode;
            }

            if (output.DocumentFile.UserId != null)
            {
                var lookupUser = await _lookupUserRepository.FirstOrDefaultAsync((long)output.DocumentFile.UserId);
                output.UserName = lookupUser?.Name;
            }


            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Edit)]
        public async Task<GetDocumentFileForEditOutput> GetDocumentFileForEdit(EntityDto<Guid> input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                return await _GetDocumentFileForEdit(input);
            }

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant))
            {
                return await _GetDocumentFileForEdit(input);
            }
        }

        //todo add this action to domain service -- trucksServers used it
        public async Task CreateOrEdit(CreateOrEditDocumentFileDto input)
        {
            //x todo convert this to custom validation


            //NumberUnique
            var document = await _documentFileRepository.FirstOrDefaultAsync(doc => doc.Id != input.Id &&
                                                         doc.DocumentTypeId == input.DocumentTypeId &&
                                                         doc.Number == input.Number &&
                                                         doc.DocumentTypeFk.IsNumberUnique == true);
            if (document != null)
            {
                throw new UserFriendlyException(L("DocumentNumberShouldBeUnique"));
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

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            DisableTenancyFiltersIfHost();
            await _documentFileRepository.DeleteAsync(input.Id);
        }

        //public async Task<FileDto> GetDocumentFilesToExcel(GetAllDocumentFilesForExcelInput input)
        //{
        //    var filteredDocumentFiles = _documentFileRepository.GetAll()
        //        .Include(e => e.DocumentTypeFk)
        //        .Include(e => e.TruckFk)
        //        .Include(e => e.TrailerFk)
        //        .Include(e => e.UserFk)
        //        .Include(e => e.RoutStepFk)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Extn.Contains(input.Filter))
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.ExtnFilter), e => e.Extn == input.ExtnFilter)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.BinaryObjectIdFilter.ToString()), e => e.BinaryObjectId.ToString() == input.BinaryObjectIdFilter.ToString())
        //        .WhereIf(input.MinExpirationDateFilter != null, e => e.ExpirationDate >= input.MinExpirationDateFilter)
        //        .WhereIf(input.MaxExpirationDateFilter != null, e => e.ExpirationDate <= input.MaxExpirationDateFilter)
        //        .WhereIf(input.IsAcceptedFilter.HasValue, e => e.IsAccepted == input.IsAcceptedFilter.Value)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentTypeDisplayNameFilter), e => e.DocumentTypeFk != null && e.DocumentTypeFk.DisplayName == input.DocumentTypeDisplayNameFilter)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.TruckPlateNumberFilter), e => e.TruckFk != null && e.TruckFk.PlateNumber == input.TruckPlateNumberFilter)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTrailerCodeFilter), e => e.TrailerFk != null && e.TrailerFk.TrailerCode == input.TrailerTrailerCodeFilter)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter)
        //        .WhereIf(!string.IsNullOrWhiteSpace(input.RoutStepDisplayNameFilter), e => e.RoutStepFk != null && e.RoutStepFk.DisplayName == input.RoutStepDisplayNameFilter);

        //    var query = from o in filteredDocumentFiles
        //                join o1 in _lookupDocumentTypeRepository.GetAll() on o.DocumentTypeId equals o1.Id into j1
        //                from s1 in j1.DefaultIfEmpty()
        //                join o2 in _lookupTruckRepository.GetAll() on o.TruckId equals o2.Id into j2
        //                from s2 in j2.DefaultIfEmpty()
        //                join o3 in _lookupTrailerRepository.GetAll() on o.TrailerId equals o3.Id into j3
        //                from s3 in j3.DefaultIfEmpty()
        //                join o4 in _lookupUserRepository.GetAll() on o.UserId equals o4.Id into j4
        //                from s4 in j4.DefaultIfEmpty()
        //                join o5 in _lookupRoutStepRepository.GetAll() on o.RoutStepId equals o5.Id into j5
        //                from s5 in j5.DefaultIfEmpty()
        //                select new GetDocumentFileForViewDto
        //                {
        //                    DocumentFile = new DocumentFileDto
        //                    {
        //                        Name = o.Name,
        //                        Extn = o.Extn,
        //                        BinaryObjectId = o.BinaryObjectId,
        //                        ExpirationDate = o.ExpirationDate,
        //                        IsAccepted = o.IsAccepted,
        //                        Id = o.Id
        //                    },
        //                    DocumentTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName,
        //                    TruckId = s2 == null || s2.PlateNumber == null ? "" : s2.PlateNumber,
        //                    TrailerTrailerCode = s3 == null || s3.TrailerCode == null ? "" : s3.TrailerCode,
        //                    UserName = s4 == null || s4.Name == null ? "" : s4.Name,
        //                    RoutStepDisplayName = s5 == null || s5.DisplayName == null ? "" : s5.DisplayName.ToString()
        //                };


        //    var documentFileListDtos = await query.ToListAsync();

        //    return _documentFilesExcelExporter.ExportToFile(documentFileListDtos);
        //}


        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileDocumentTypeLookupTableDto>> GetAllDocumentTypeForTableDropdown()
        {
            return await _lookupDocumentTypeRepository.GetAll()
                .Select(documentType => new DocumentFileDocumentTypeLookupTableDto { Id = documentType.Id, DisplayName = documentType == null || documentType.DisplayName == null ? "" : documentType.DisplayName.ToString() }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileTruckLookupTableDto>> GetAllTruckForTableDropdown()
        {
            return await _lookupTruckRepository.GetAll()
                .Select(truck => new DocumentFileTruckLookupTableDto { Id = truck.Id.ToString(), DisplayName = truck == null || truck.PlateNumber == null ? "" : truck.PlateNumber.ToString() }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileTrailerLookupTableDto>> GetAllTrailerForTableDropdown()
        {
            return await _lookupTrailerRepository.GetAll()
                .Select(trailer => new DocumentFileTrailerLookupTableDto { Id = trailer.Id, DisplayName = trailer == null || trailer.TrailerCode == null ? "" : trailer.TrailerCode.ToString() }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileUserLookupTableDto>> GetAllUserForTableDropdown()
        {
            return await _lookupUserRepository.GetAll()
                .Select(user => new DocumentFileUserLookupTableDto { Id = user.Id, DisplayName = user == null || user.Name == null ? "" : user.Name.ToString() }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
        public async Task<List<DocumentFileRoutStepLookupTableDto>> GetAllRoutStepForTableDropdown()
        {
            return await _lookupRoutStepRepository.GetAll()
                .Select(routStep => new DocumentFileRoutStepLookupTableDto { Id = routStep.Id, DisplayName = routStep == null || routStep.DisplayName == null ? "" : routStep.DisplayName.ToString() }).ToListAsync();
        }

        /// <summary>
        ///     to download the file
        /// </summary>
        /// <param name="documentFileId"></param>
        /// <returns>FileDto to send to download controller</returns>
        [AbpAllowAnonymous]
        public async Task<FileDto> GetDocumentFileDto(Guid documentFileId)
        {
            DisableTenancyFiltersIfHost();
            var documentFile = await _documentFileRepository.GetAsync(documentFileId);

            var binaryObject = await _binaryObjectManager.GetOrNullAsync(documentFile.BinaryObjectId.Value);
            if (binaryObject == null) throw new UserFriendlyException(L("TheFileDoesNotExists"));
            var file = new FileDto(documentFile.Name, documentFile.Extn);

            _tempFileCacheManager.SetFile(file.FileToken, binaryObject.Bytes);

            return file;
        }


        /// <summary>
        ///     truck required documents list template
        /// </summary>
        /// <returns></returns>
        public async Task<List<CreateOrEditDocumentFileDto>> GetTruckRequiredDocumentFiles(string truckId)
        {
            return await GetRequiredDocumentFileListForCreateOrEdit(DocumentsEntitiesEnum.Truck, truckId);
        }

        public async Task<List<CreateOrEditDocumentFileDto>> GetDriverRequiredDocumentFiles(string userId)
        {
            return await GetRequiredDocumentFileListForCreateOrEdit(DocumentsEntitiesEnum.Driver, userId);
        }



        public async Task AddTenantRequiredDocumentFiles(List<CreateOrEditDocumentFileDto> input)
        {
            var requiredDocumentTypes = await GetTenentMissingDocuments(true);
            if (requiredDocumentTypes.Count > 0)
            {
                foreach (var documentType in requiredDocumentTypes)
                {
                    var doc = input.FirstOrDefault(x => x.DocumentTypeId == documentType.DocumentTypeId);

                    if (doc.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L("document missing msg :" + documentType.DocumentTypeDto.DisplayName));
                    }

                }
            }

            foreach (var item in input)
            {
                await Create(item);
            }
            //todo add notifications to host
        }

        public async Task<List<SelectItemDto>> GetAllEditionsForDropdown()
        {
            return await _editionRepository.GetAll()
                .Select(x => new SelectItemDto { DisplayName = x.DisplayName, Id = x.Id.ToString() }).ToListAsync();
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Create)]
        protected virtual async Task Create(CreateOrEditDocumentFileDto input)
        {

            var documentFile = ObjectMapper.Map<DocumentFile>(input);

            if (input.DocumentTypeDto != null && input.DocumentTypeDto.IsNumberUnique)
            //if(input.DocumentTypeDto.IsNumberUnique)
            {
                var count = await _documentFileRepository.CountAsync(x => x.Number == input.Number && x.DocumentTypeId == input.DocumentTypeId);
                if (count > 0)
                {
                    throw new UserFriendlyException(L("document number should be unique message"));
                }
            }
            if (string.IsNullOrEmpty(documentFile.Name))
            {
                documentFile.Name = input.DocumentTypeDto.DisplayName + "_" + AbpSession.GetTenantId();
            }


            if (AbpSession.TenantId != null)
            {
                documentFile.TenantId = AbpSession.TenantId;
            }



            if (input.EntityType == DocumentsEntitiesEnum.Truck)
            {
                documentFile.TruckId = long.Parse(input.EntityId);
            }

            if (input.EntityType == DocumentsEntitiesEnum.Driver)
            {
                documentFile.UserId = long.Parse(input.EntityId);
            }


            if (!input.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
            {
                documentFile.BinaryObjectId = await _documentFilesManager.SaveDocumentFileBinaryObject(input.UpdateDocumentFileInput.FileToken, AbpSession.TenantId);
            }
            await _documentFileRepository.InsertAsync(documentFile);

        }



        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Edit)]
        protected virtual async Task Update(CreateOrEditDocumentFileDto input)
        {
            //host can update tenants documents 
            DisableTenancyFiltersIfHost();

            await _documentFilesManager.UpdateDocumentFile(input);

        }

        private async Task<GetDocumentFileForEditOutput> _GetDocumentFileForEdit(EntityDto<Guid> input)
        {

            var documentFile = await _documentFileRepository
                                     .GetAll()
                                     .Include(a => a.DocumentTypeFk)
                                     .FirstOrDefaultAsync(x => x.Id == input.Id);


            var docFileDto = ObjectMapper.Map<CreateOrEditDocumentFileDto>(documentFile);
            docFileDto.DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(documentFile.DocumentTypeFk);

            var output = new GetDocumentFileForEditOutput
            {
                DocumentFile = docFileDto
            };

            return output;
        }


        /// <summary>
        ///     get list of required documents by documentType-entity-name truck, carrier, shipper, driver etc.
        /// </summary>
        /// <param name="documentsEntityName">documentType-entity-name can be found in  <see cref="AppConsts" />  </param>
        /// <returns>
        ///     list of <see cref="CreateOrEditDocumentFileDto" /> with empty <see cref="UpdateDocumentFileInput" /> ready to
        ///     fill with uploaded FileToken
        /// </returns>
        private async Task<List<CreateOrEditDocumentFileDto>> GetRequiredDocumentFileListForCreateOrEdit(DocumentsEntitiesEnum documentsEntityId, string entityId)
        {
            var list = new List<DocumentType>();
            if (string.IsNullOrEmpty(entityId))
            {
                list = await _documentTypeRepository.GetAll()
                    .Include(x => x.Translations)
                .Where(x => x.DocumentsEntityId == (int)documentsEntityId)
                .ToListAsync();

                return list.Select(x => new CreateOrEditDocumentFileDto
                {
                    DocumentTypeId = x.Id,
                    DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x)
                }).ToList();
            }

            var mainQuery = _documentTypeRepository.GetAll()
             .Include(ent => ent.DocumentsEntityFk)
             .Include(x => x.Translations)
             .Where(doc => doc.DocumentsEntityId == (int)documentsEntityId);

            if (documentsEntityId == DocumentsEntitiesEnum.Driver)
            {



                var query = from o in mainQuery
                            join o1 in _documentFileRepository.GetAll()
                                .Where(a => a.UserId == long.Parse(entityId)) on o.Id equals o1.DocumentTypeId into j1
                            from s1 in j1.DefaultIfEmpty()
                            where s1.UserId == null
                            select o;

                list = await query.ToListAsync();
                return list.Select(x => new CreateOrEditDocumentFileDto
                {
                    DocumentTypeId = x.Id,
                    UserId = long.Parse(entityId),
                    DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x)
                }).ToList();

            }
            else if (documentsEntityId == DocumentsEntitiesEnum.Truck)
            {


                var query = from o in mainQuery
                            join o1 in _documentFileRepository.GetAll()
                                .Where(a => a.TruckId == long.Parse(entityId)) on o.Id equals o1.DocumentTypeId into j1
                            from s1 in j1.DefaultIfEmpty()
                            where s1.TruckId == null
                            select o;

                list = await query.ToListAsync();
                return list.Select(x => new CreateOrEditDocumentFileDto
                {
                    DocumentTypeId = x.Id,
                    TruckId = long.Parse(entityId),
                    DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x)
                }).ToList();

            }

            return list.Select(x => new CreateOrEditDocumentFileDto
            {
                DocumentTypeId = x.Id,
                DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x)
            }).ToList();
        }

        public async Task AcceptAsync(Guid id)
        {
            DisableTenancyFiltersIfHost();

            var documentFile = await _documentFileRepository.GetAll()
                .Include(x => x.TenantFk)
                .FirstOrDefaultAsync(x => x.Id == id);
            documentFile.IsAccepted = true;
            documentFile.IsRejected = false;
            documentFile.RejectionReason = "";
            if (documentFile.CreatorUserId != null)
            {
                await _appNotifier.AcceptedSubmittedDocument(new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value), documentFile);
            }

            if (await IsAllRequiredDocumentsApproved(documentFile.TenantId.Value))
            {
                await _userEmailer.SendAllApprovedDocumentsAsync(documentFile.TenantId.Value, _appUrlService.GetTachyonPlatformLoginUrl());
            }

        }

        public async Task Reject(Guid id, string reason)
        {
            DisableTenancyFiltersIfHost();

            var documentFile = await _documentFileRepository.FirstOrDefaultAsync(id);
            if (documentFile == null)
            {
                throw new UserFriendlyException(L("DocumentNotFound"));
            }
            documentFile.IsAccepted = false;
            documentFile.IsRejected = true;
            documentFile.RejectionReason = reason;
            if (documentFile.CreatorUserId != null)
            {
                await _userEmailer.SendRejectedDocumentEmail(documentFile.TenantId.Value, documentFile.Name, reason);
                await _appNotifier.RejectedSubmittedDocument(new UserIdentifier(documentFile.TenantId, documentFile.CreatorUserId.Value), documentFile);
            }
        }


        /// <summary>
        /// is for ...
        /// </summary>
        /// <returns></returns>
        public async Task<List<GetTenantSubmittedDocumnetForView>> GetAllTenantSubmittedDocumentsWithStatuses(bool isMandatory = true)
        {

            var docs = await _documentFileRepository.GetAll()
                     .Include(doc => doc.DocumentTypeFk)
                     .ThenInclude(doc => doc.Translations)
                     .Where(x => x.DocumentTypeFk.DocumentsEntityId == (int)DocumentsEntitiesEnum.Tenant && (x.DocumentTypeFk.DocumentRelatedWithId.HasValue == false || x.DocumentTypeFk.DocumentRelatedWithId == AbpSession.TenantId))
                     .Where(x => x.DocumentTypeFk.IsRequired == isMandatory)
                     //.Where(d => d.TenantId == AbpSession.GetTenantId())
                     .ToListAsync();

            var result = docs
                .Select(x => new GetTenantSubmittedDocumnetForView()
                {
                    Id = x.Id,
                    Extn = x.Extn,
                    IsAccepted = x.IsAccepted,
                    IsRejected = x.IsRejected,
                    RejectionReason = x.RejectionReason,
                    Name = x.DocumentTypeFk.DisplayName,
                    DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x.DocumentTypeFk),
                    ExpirationDate = x.ExpirationDate
                });
            return result.ToList();

        }

        public async Task<bool> IsDocumentTypeNumberUnique(DocumentUniqueCheckOutput input)
        {
            var result = await _documentFileRepository
                .FirstOrDefaultAsync(x => x.Number == input.Number && x.DocumentTypeId == long.Parse(input.DocumentTypeId));
            return result == null;
        }
        /// <summary>
        ///     check if the entity is missing document files and are needed to be upload them
        /// </summary>
        /// <returns>
        /// true if the entity is missing document files
        /// </returns>

        public async Task<bool> CheckIfMissingDocumentFiles(string entityId, DocumentsEntitiesEnum documentsEntityId)
        {
            return await _documentFilesManager.CheckIfMissingDocumentFiles(entityId, documentsEntityId);
        }
        public async Task<List<CreateOrEditDocumentFileDto>> GetTenantRequiredDocumentFilesTemplateForCreate()
        {
            var list = await _documentFilesManager.GetAllTenantMissingRequiredDocumentTypesListAsync(AbpSession.GetTenantId());
            return list.Select(x => new CreateOrEditDocumentFileDto
            {
                DocumentTypeId = x.Id,
                DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x)
            }).ToList();
        }

        public async Task<List<CreateOrEditDocumentFileDto>> GetTenentMissingDocuments(bool isMandatory = true)
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return new List<CreateOrEditDocumentFileDto>();
            }
            var tenant = _tenantManager.GetById(AbpSession.TenantId.Value);

            var documentFiles = await _documentFileRepository.GetAll()
                    .Include(doc => doc.DocumentTypeFk)
                    .ThenInclude(doc => doc.DocumentsEntityFk)
                    .Where(x => x.DocumentTypeFk.IsRequired == isMandatory)
                    .ToListAsync();

            var documentTypes = await _documentTypeRepository.GetAll()
                 .Include(x => x.Translations)
                 .Where(x => (x.EditionId == tenant.EditionId && x.DocumentRelatedWithId == null) || x.DocumentRelatedWithId == AbpSession.TenantId)
                 .Where(x => x.IsRequired == isMandatory)
                 .ToListAsync();


            if (documentTypes != null && documentTypes.Any())
            {
                foreach (var item in documentTypes.ToList())
                {
                    if (documentFiles.Any(x => x.DocumentTypeId == item.Id))
                    {
                        documentTypes.Remove(item);
                    }
                }
            }

            return documentTypes.Select(x => new CreateOrEditDocumentFileDto
            {
                DocumentTypeId = x.Id,
                DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x)
            }).ToList();
        }

        public async Task<bool> IsAllRequiredDocumentsApproved(int tenantId)
        {
            DisableTenancyFilters();
            var tenant = _tenantManager.GetById(tenantId);

            var documentFiles = await _documentFileRepository.GetAll()
                    .Where(x => x.TenantId == tenantId)
                    .Where(x => x.IsAccepted == true)
                    .Include(doc => doc.DocumentTypeFk)
                    .ThenInclude(doc => doc.DocumentsEntityFk)
                    .Where(x => x.DocumentTypeFk.IsRequired)
                    .ToListAsync();

            var documentTypes = await _documentTypeRepository.GetAll()
                 .Include(x => x.Translations)
                 .Where(x => x.EditionId == tenant.EditionId)
                 .Where(x => x.IsRequired)
                 .ToListAsync();


            if (documentTypes != null && documentTypes.Any())
            {
                foreach (var item in documentTypes.ToList())
                {
                    if (documentFiles.Any(x => x.DocumentTypeId == item.Id))
                    {
                        documentTypes.Remove(item);
                    }
                }
            }

            return documentTypes.Count == 0;
        }

        public async Task<List<SelectItemDto>> GetDocumentEntitiesForTableDropdown()
        {
            using (this.CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant, AbpDataFilters.MustHaveTenant))
            {
                var entities = await _documentEntityRepository
               .GetAll()
               .Select(x => new SelectItemDto
               {
                   DisplayName = x.DisplayName,
                   Id = x.Id.ToString()
               }
               ).ToListAsync();

                return entities;
            }


        }

        /// <summary>
        /// using the Abpsession in the front end creates issues when using thet Imparsonate Function
        /// </summary>
        /// <returns></returns>
        //public bool GetIsCurrentTenantHost()
        //{
        //    return AbpSession.TenantId == null;
        //}




    }
}