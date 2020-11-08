using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Editions;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using TACHYON.Authorization;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentFiles.Exporting;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentsEntities.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Dto;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutSteps;
using TACHYON.Storage;
using TACHYON.Trailers;
using TACHYON.Trucks;

namespace TACHYON.Documents.DocumentFiles
{
    [AbpAuthorize(AppPermissions.Pages_DocumentFiles)]
    public class DocumentFilesAppService : TACHYONAppServiceBase, IDocumentFilesAppService
    {


        public DocumentFilesAppService(IRepository<DocumentsEntity, int> documentEntityRepository, IRepository<Tenant, int> lookupTenantRepository,IRepository<DocumentFile, Guid> documentFileRepository, IDocumentFilesExcelExporter documentFilesExcelExporter, IRepository<DocumentType, long> lookupDocumentTypeRepository, IRepository<Truck, Guid> lookupTruckRepository, IRepository<Trailer, long> lookupTrailerRepository, IRepository<User, long> lookupUserRepository, IRepository<RoutStep, long> lookupRoutStepRepository, ITempFileCacheManager tempFileCacheManager, IBinaryObjectManager binaryObjectManager, IRepository<Edition, int> editionRepository, IRepository<DocumentType, long> documentTypeRepository, DocumentFilesManager documentFilesManager)
        {
            _lookupTenantRepository = lookupTenantRepository;
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
            _documentEntityRepository = documentEntityRepository;
        }

        private readonly IRepository<Tenant, int> _lookupTenantRepository;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly IDocumentFilesExcelExporter _documentFilesExcelExporter;
        private readonly IRepository<DocumentType, long> _lookupDocumentTypeRepository;
        private readonly IRepository<Truck, Guid> _lookupTruckRepository;
        private readonly IRepository<Trailer, long> _lookupTrailerRepository;
        private readonly IRepository<User, long> _lookupUserRepository;
        private readonly IRepository<RoutStep, long> _lookupRoutStepRepository;
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IRepository<DocumentType, long> _documentTypeRepository;
        private readonly IRepository<DocumentsEntity, int> _documentEntityRepository;
        private readonly IRepository<Edition, int> _editionRepository;
        private readonly DocumentFilesManager _documentFilesManager;

        public async Task<PagedResultDto<GetDocumentFileForViewDto>> GetAll(GetAllDocumentFilesInput input)
        {
            var filteredDocumentFiles = _documentFileRepository.GetAll()
                .Include(e => e.DocumentTypeFk)
                .Include(e => e.TruckFk)
                .Include(e => e.TrailerFk)
                .Include(e => e.UserFk)
                .Include(e => e.RoutStepFk)
                .WhereIf(!AbpSession.TenantId.HasValue, e => e.DocumentTypeFk.DocumentsEntityFk.DisplayName == AppConsts.TenantDocumentsEntityName)
                //.WhereIf(AbpSession.TenantId.HasValue, e => e.DocumentTypeFk.DocumentsEntityFk.DisplayName == AppConsts.TenantDocumentsEntityName)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Extn.Contains(input.Filter))
                //.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name == input.NameFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.ExtnFilter), e => e.Extn == input.ExtnFilter)
                //.WhereIf(!string.IsNullOrWhiteSpace(input.BinaryObjectIdFilter.ToString()), e => e.BinaryObjectId.ToString() == input.BinaryObjectIdFilter.ToString())
                .WhereIf(input.MinExpirationDateFilter != null, e => e.ExpirationDate >= input.MinExpirationDateFilter)
                .WhereIf(input.MaxExpirationDateFilter != null, e => e.ExpirationDate <= input.MaxExpirationDateFilter)
                //.WhereIf(input.IsAcceptedFilter.HasValue, e => e.IsAccepted == input.IsAcceptedFilter.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentTypeDisplayNameFilter), e => e.DocumentTypeFk != null && e.DocumentTypeFk.DisplayName == input.DocumentTypeDisplayNameFilter)
                .WhereIf(input.TruckIdFilter != null, e => e.TruckFk.Id == input.TruckIdFilter)

                .WhereIf(input.EntityIdFilter != null && input.DocumentEntityFilter == "Truck", e => e.TruckFk.Id == Guid.Parse( input.EntityIdFilter))
                .WhereIf(input.EntityIdFilter != null && input.DocumentEntityFilter == "Driver", e => e.UserFk.Id == long.Parse(input.EntityIdFilter))
                .WhereIf(input.EntityIdFilter != null && input.DocumentEntityFilter == "Trailer", e => e.TrailerFk.Id == long.Parse(input.EntityIdFilter))

                //.WhereIf(!string.IsNullOrWhiteSpace(input.TruckIdFilter), e => e.TruckFk != null && e.TruckFk.Id == input.TruckIdFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.TrailerTrailerCodeFilter), e => e.TrailerFk != null && e.TrailerFk.TrailerCode == input.TrailerTrailerCodeFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.DocumentEntityFilter), e => e.DocumentTypeFk.DocumentsEntityFk.DisplayName != null && e.DocumentTypeFk.DocumentsEntityFk.DisplayName == input.DocumentEntityFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);
                //.WhereIf(!string.IsNullOrWhiteSpace(input.RoutStepDisplayNameFilter), e => e.RoutStepFk != null && e.RoutStepFk.DisplayName == input.RoutStepDisplayNameFilter);

            var pagedAndFilteredDocumentFiles = filteredDocumentFiles
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var documentFiles = from o in pagedAndFilteredDocumentFiles
                                join o1 in _lookupDocumentTypeRepository.GetAll() on o.DocumentTypeId equals o1.Id into j1
                                from s1 in j1.DefaultIfEmpty()
                                join o2 in _lookupTruckRepository.GetAll() on o.TruckId equals o2.Id into j2
                                from s2 in j2.DefaultIfEmpty()
                                join o3 in _lookupTrailerRepository.GetAll() on o.TrailerId equals o3.Id into j3
                                from s3 in j3.DefaultIfEmpty()
                                join o4 in _lookupUserRepository.GetAll() on o.UserId equals o4.Id into j4
                                from s4 in j4.DefaultIfEmpty()
                                join o5 in _lookupRoutStepRepository.GetAll() on o.RoutStepId equals o5.Id into j5
                                from s5 in j5.DefaultIfEmpty()
                                join o6 in _lookupTenantRepository.GetAll() on o.TenantId equals o6.Id into j6
                                from s6 in j6.DefaultIfEmpty()

                                select new GetDocumentFileForViewDto
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
                                    SubmitterTenatTenancyName = s6 == null || s6.TenancyName == null ? "Host" : s6.TenancyName.ToString(),
                                    HasDate = o.DocumentTypeFk.HasExpirationDate,
                                    HasNumber = o.DocumentTypeFk.HasNumber,
                                    Number = o.Number,
                                    DocumentEntityDisplayName = (o.DocumentTypeFk.DocumentsEntityFk) == null ? "" : o.DocumentTypeFk.DocumentsEntityFk.DisplayName,
                                    DocumentTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName,
                                    TruckId = (o.TruckFk == null ? (Guid?)null : o.TruckFk.Id).ToString(),
                                    TrailerTrailerCode = s3 == null || s3.TrailerCode == null ? "" : s3.TrailerCode,
                                    UserName = s4 == null || s4.Name == null ? "" : s4.UserName,
                                    RoutStepDisplayName = s5 == null || s5.DisplayName == null ? "" : s5.DisplayName.ToString()
                                };


            if (AbpSession.TenantId.HasValue)
            { var result = await documentFiles.ToListAsync();
                return new PagedResultDto<GetDocumentFileForViewDto>(
                    await filteredDocumentFiles.CountAsync(),
                   result
                );
            }

            using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
            { 
                var result = await documentFiles.ToListAsync();
                return new PagedResultDto<GetDocumentFileForViewDto>(
                    await filteredDocumentFiles.CountAsync(),
                    result

                );
            }
        }


        public async Task<GetDocumentFileForViewDto> GetDocumentFileForView(Guid id)
        {
            var documentFile = await _documentFileRepository.GetAsync(id);

            var output = new GetDocumentFileForViewDto { DocumentFile = ObjectMapper.Map<DocumentFileDto>(documentFile) };

            {
                var lookupDocumentType = await _lookupDocumentTypeRepository.FirstOrDefaultAsync(output.DocumentFile.DocumentTypeId);
                output.DocumentTypeDisplayName = lookupDocumentType?.DisplayName;
            }

            if (output.DocumentFile.TruckId != null)
            {
                var lookupTruck = await _lookupTruckRepository.FirstOrDefaultAsync((Guid)output.DocumentFile.TruckId);
                output.TruckId = (lookupTruck ==null? (Guid?)null:lookupTruck.Id).ToString();
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

            if (output.DocumentFile.RoutStepId != null)
            {
                var lookupRoutStep = await _lookupRoutStepRepository.FirstOrDefaultAsync((long)output.DocumentFile.RoutStepId);
                output.RoutStepDisplayName = lookupRoutStep?.DisplayName;
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_DocumentFiles_Edit)]
        public async Task<DocumentFileForCreateOrEditDto> GetDocumentFileForEdit(EntityDto<Guid> input)
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


        public async Task CreateDocument(CreateOrEditDocumentFileDto input)
        {
                await Create(input);   
        }     
        public async Task UpdateDocument(DocumentFileForCreateOrEditDto input)
        {

               await Update(input);
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

            var binaryObject = await _binaryObjectManager.GetOrNullAsync(documentFile.BinaryObjectId);

            var file = new FileDto(documentFile.Name, documentFile.Extn);

            _tempFileCacheManager.SetFile(file.FileToken, binaryObject.Bytes);

            return file;
        }


        /// <summary>
        ///     truck required documents list template
        /// </summary>
        /// <returns></returns>
        public async Task<List<CreateOrEditDocumentFileDto>> GetTruckRequiredDocumentFiles(string truckId )
        {
            return await GetRequiredDocumentFileListForCreateOrEdit(AppConsts.TruckDocumentsEntityName, truckId);
        }

        public async Task<List<CreateOrEditDocumentFileDto>> GetDriverRequiredDocumentFiles(string userId)
        {
            return await GetRequiredDocumentFileListForCreateOrEdit(AppConsts.DriverDocumentsEntityName , userId);
        }

        /// <summary>
        /// tenant required documents files list as a template to fill later in create
        /// </summary>
        /// <returns></returns>
        public async Task<List<CreateOrEditDocumentFileDto>> GetTenantRequiredDocumentFilesTemplateForCreate()
        {
            var list = await _documentFilesManager.GetAllTenantMissingRequiredDocumentTypesListAsync(AbpSession.GetTenantId());
            return list.Select(x => new CreateOrEditDocumentFileDto
            {
                DocumentTypeId = x.Id,
                DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x)
            }).ToList();
        }

        public async Task AddTenantRequiredDocumentFiles(List<CreateOrEditDocumentFileDto> input)
        {
            var requiredDocumentTypes = await _documentFilesManager.GetAllTenantMissingRequiredDocumentTypesListAsync(AbpSession.GetTenantId());
            if (requiredDocumentTypes.Count > 0)
            {
                foreach (var documentType in requiredDocumentTypes)
                {
                    var doc = input.FirstOrDefault(x => x.DocumentTypeId == documentType.Id);

                    if (doc.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
                    {
                        throw new UserFriendlyException(L("document missing msg :" + documentType.DisplayName));
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
            
            if (input.DocumentTypeDto.IsNumberUnique)
            {
                var count = await _documentFileRepository.CountAsync(x => x.Number == input.Number && x.DocumentTypeId == input.DocumentTypeId);
                if (count > 0)
                {
                    throw new UserFriendlyException(L("document number should be unique message"));
                }
            }
                documentFile.Name = input.DocumentTypeDto.DisplayName + "_" + AbpSession.GetTenantId();


                if (AbpSession.TenantId != null)
                {
                    documentFile.TenantId = AbpSession.TenantId;
                }
            


            if (input.EntityType == AppConsts.TruckDocumentsEntityName)
            {
                documentFile.TruckId = Guid.Parse(input.EntityId);
            }

            if (input.EntityType == AppConsts.DriverDocumentsEntityName)
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
        protected virtual async Task Update(DocumentFileForCreateOrEditDto input)
        {
            //host can update tenants documents 
            DisableTenancyFiltersIfHost();

            DocumentFile documentFile = await _documentFileRepository.FirstOrDefaultAsync((Guid)input.Id);

            if (input.UpdateDocumentFileInput != null && !input.UpdateDocumentFileInput.FileToken.IsNullOrEmpty())
            {
                await _binaryObjectManager.DeleteAsync(documentFile.BinaryObjectId);
                documentFile.BinaryObjectId = await _documentFilesManager.SaveDocumentFileBinaryObject(input.UpdateDocumentFileInput.FileToken, AbpSession.TenantId);
            }

            //ObjectMapper.Map(input, documentFile);

            documentFile.Extn = input.Extn;
            if (input.HasNumber)
            {
            documentFile.Number = input.Number;
            }
            if (input.HasNotes)
            {
            documentFile.Notes = input.Notes;
            }
            if (input.HasExpirationDate)
            {
            documentFile.ExpirationDate =  input.ExpirationDate ;
            }
        }

        private async Task<DocumentFileForCreateOrEditDto> _GetDocumentFileForEdit(EntityDto<Guid> input)
        {
            var output = await _documentFileRepository.GetAll().Where(doc=>doc.Id==input.Id)
                .Include(a => a.DocumentTypeFk)
                .Select(res => new DocumentFileForCreateOrEditDto
                {
                    //document type data
                    DocumentTypeDisplayName= res.DocumentTypeFk==null?null: res.DocumentTypeFk.DisplayName,
                    HasExpirationDate = res.DocumentTypeFk == null ? false : res.DocumentTypeFk.HasExpirationDate,
                    HasHijriExpirationDate= res.DocumentTypeFk==null?false: res.DocumentTypeFk.HasHijriExpirationDate,
                    HasNotes = res.DocumentTypeFk == null ? false : res.DocumentTypeFk.HasNotes,
                    HasNumber= res.DocumentTypeFk == null ? false : res.DocumentTypeFk.HasNumber,
                    IsNumberUnique= res.DocumentTypeFk == null ? false : res.DocumentTypeFk.IsNumberUnique,
                    NumberMinDigits = res.DocumentTypeFk == null ? null : res.DocumentTypeFk.NumberMinDigits,
                    NumberMaxDigits= res.DocumentTypeFk == null ? null : res.DocumentTypeFk.NumberMaxDigits,

                    //document data
                    Id= res.Id,
                    ExpirationDate =res.ExpirationDate,
                    Extn= res.Extn,
                    Notes= res.Notes,
                    Number = res.Number

                }).FirstOrDefaultAsync();

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
        private async Task<List<CreateOrEditDocumentFileDto>> GetRequiredDocumentFileListForCreateOrEdit(string documentsEntityName ,string entityId)
        {
            var list = new List<DocumentType>();
            if (entityId == null|| entityId =="") {
                list = await _documentTypeRepository.GetAll()
                .Where(x => x.DocumentsEntityFk.DisplayName == documentsEntityName)
                .ToListAsync();
            }
 

            if (entityId!=null && entityId != "")
            {
                if(documentsEntityName == AppConsts.DriverDocumentsEntityName)
                {
                    var resultList = _documentTypeRepository.GetAll().Include(ent=>ent.DocumentsEntityFk)
                        .Where(doc => doc.DocumentsEntityFk.DisplayName == AppConsts.DriverDocumentsEntityName);

                   
                    var query = from o in resultList
                           join o1 in _documentFileRepository.GetAll().Where(a=>a.UserId == long.Parse(entityId) ) on o.Id equals o1.DocumentTypeId into j1
                           from s1 in j1.DefaultIfEmpty()
                           where  s1.UserId == null
                                select new DocumentType{
                                    DisplayName = o.DisplayName,
                                    HasExpirationDate = o.HasExpirationDate,
                                    Id = o.Id,
                                    HasHijriExpirationDate = o.HasHijriExpirationDate,
                                    HasNotes = o.HasNotes,
                                    HasNumber = o.HasNumber,
                                    IsNumberUnique = o.IsNumberUnique,
                                    IsRequired = o.IsRequired,
                                    NumberMaxDigits = o.NumberMaxDigits,
                                    NumberMinDigits = o.NumberMinDigits
                                };

                    list = await query.ToListAsync();
                }
                else if (documentsEntityName == AppConsts.TruckDocumentsEntityName)
                {
                    var resultList = _documentTypeRepository.GetAll().Include(ent => ent.DocumentsEntityFk)
                        .Where(doc => doc.DocumentsEntityFk.DisplayName == AppConsts.TruckDocumentsEntityName);

                    var query = from o in resultList
                                join o1 in _documentFileRepository.GetAll().Where(a => a.TruckId == Guid.Parse(entityId)) on o.Id equals o1.DocumentTypeId into j1
                                from s1 in j1.DefaultIfEmpty()
                                where s1.TruckId == null
                                select new DocumentType {
                                    DisplayName = o.DisplayName,
                                    HasExpirationDate = o.HasExpirationDate,
                                    Id = o.Id,
                                    HasHijriExpirationDate = o.HasHijriExpirationDate,
                                    HasNotes = o.HasNotes,
                                    HasNumber = o.HasNumber,
                                    IsNumberUnique = o.IsNumberUnique,
                                    IsRequired = o.IsRequired,
                                    NumberMaxDigits = o.NumberMaxDigits,
                                    NumberMinDigits = o.NumberMinDigits
                                };

                    list = await query.ToListAsync();
                }

            }
            return list.Select(x => new CreateOrEditDocumentFileDto { DocumentTypeId = x.Id, DocumentTypeDto = ObjectMapper.Map<DocumentTypeDto>(x) }).ToList();
        }



        /// <summary>
        ///     check if the entity is missing document files and are needed to be upload them
        /// </summary>
        /// <returns>
        /// true if the entity is missing document files
        /// </returns>
        public async Task<bool> CheckIfMissingDocumentFiles(string entityId, string entityType)
        {
            var result = false;
            if (entityType == AppConsts.DriverDocumentsEntityName)
            {
                var documentTypes = await _documentTypeRepository.GetAll().Include(ent => ent.DocumentsEntityFk)
                    .Where(doc => doc.DocumentsEntityFk.DisplayName == AppConsts.DriverDocumentsEntityName).CountAsync();

                var submittedDocuments = await _documentFileRepository.GetAll().Where(t => t.UserId == long.Parse(entityId))
                    .CountAsync();
                result = documentTypes != submittedDocuments;
            }


            if (entityType == AppConsts.TruckDocumentsEntityName)
            {
                var documentTypes = await _documentTypeRepository.GetAll().Include(ent => ent.DocumentsEntityFk)
                    .Where(doc => doc.DocumentsEntityFk.DisplayName == AppConsts.TruckDocumentsEntityName).CountAsync();

                var submittedDocuments = await _documentFileRepository.GetAll().Where(t => t.TruckId == Guid.Parse(entityId))
                    .CountAsync();
                result = documentTypes != submittedDocuments;
            }



            return result;
        }





        public async Task<List<GetDocumentEntitiesLookupDto>> GetDocumentEntitiesForDocumentFile()
        {

            var result = await _documentEntityRepository.GetAll().Select(res => new GetDocumentEntitiesLookupDto
            {
                DisplayName = res.DisplayName
            }
            ).ToListAsync();
            return result;
        }

        /// <summary>
        /// using the Abpsession in the front end creates issues when using thet Imparsonate Function
        /// </summary>
        /// <returns></returns>
        public bool GetIsCurrentTenantHost()
        {
            return AbpSession.TenantId == null;
        }



    }
}