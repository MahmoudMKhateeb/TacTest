using TACHYON.Actors;

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Actors.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Abp.Organizations;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using TACHYON.Organizations.Dto;
using TACHYON.Storage;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Invoices.ActorInvoices;

namespace TACHYON.Actors
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Actors)]
    public class ActorsAppService : TACHYONAppServiceBase, IActorsAppService
    {
        private readonly IRepository<Actor> _actorRepository;
        private readonly OrganizationUnitManager _organizationUnitManager;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly DocumentFilesAppService _documentFilesAppService;
        private readonly IRepository<DocumentFile, Guid> _documentFileRepository;
        private readonly ActorInvoicesManager _actorInvoicesManager;


        public ActorsAppService(IRepository<Actor> actorRepository, OrganizationUnitManager organizationUnitManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository, DocumentFilesAppService documentFilesAppService, IRepository<DocumentFile, Guid> documentFileRepository,
            ActorInvoicesManager actorInvoicesManager)
        {
            _actorRepository = actorRepository;
            _organizationUnitManager = organizationUnitManager;
            _organizationUnitRepository = organizationUnitRepository;
            _documentFilesAppService = documentFilesAppService;
            _documentFileRepository = documentFileRepository;
            _actorInvoicesManager = actorInvoicesManager;
        }

        public async Task<PagedResultDto<GetActorForViewDto>> GetAll(GetAllActorsInput input)
        {
            var actorTypeFilter = input.ActorTypeFilter.HasValue
                        ? (ActorTypesEnum)input.ActorTypeFilter
                        : default;

            var filteredActors = _actorRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.CompanyName.Contains(input.Filter) || e.MoiNumber.Contains(input.Filter) || e.Address.Contains(input.Filter) || e.Logo.Contains(input.Filter) || e.MobileNumber.Contains(input.Filter) || e.Email.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.CompanyNameFilter), e => e.CompanyName == input.CompanyNameFilter)
                        .WhereIf(input.ActorTypeFilter.HasValue && input.ActorTypeFilter > -1, e => e.ActorType == actorTypeFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MoiNumberFilter), e => e.MoiNumber == input.MoiNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.AddressFilter), e => e.Address == input.AddressFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.MobileNumberFilter), e => e.MobileNumber == input.MobileNumberFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email == input.EmailFilter);

            var pagedAndFilteredActors = filteredActors
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var actors = from o in pagedAndFilteredActors
                         select new
                         {

                             o.CompanyName,
                             o.ActorType,
                             o.MoiNumber,
                             o.Address,
                             o.MobileNumber,
                             o.Email,
                             Id = o.Id
                         };

            var totalCount = await filteredActors.CountAsync();

            var dbList = await actors.ToListAsync();
            var results = new List<GetActorForViewDto>();

            foreach (var o in dbList)
            {
                var res = new GetActorForViewDto()
                {
                    Actor = new ActorDto
                    {

                        CompanyName = o.CompanyName,
                        ActorType = o.ActorType,
                        MoiNumber = o.MoiNumber,
                        Address = o.Address,
                        MobileNumber = o.MobileNumber,
                        Email = o.Email,
                        Id = o.Id,
                    }
                };

                results.Add(res);
            }

            return new PagedResultDto<GetActorForViewDto>(
                totalCount,
                results
            );

        }

        public async Task<GetActorForViewDto> GetActorForView(int id)
        {
            var actor = await _actorRepository.GetAsync(id);

            var output = new GetActorForViewDto { Actor = ObjectMapper.Map<ActorDto>(actor) };
            var documentFile =
                  await _documentFileRepository.FirstOrDefaultAsync(x => x.ActorId == id);
            if (documentFile != null)
            {
                output.Actor.DocumentFile = ObjectMapper.Map<DocumentFileDto>(documentFile);
            }
            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Actors_Edit)]
        public async Task<GetActorForEditOutput> GetActorForEdit(EntityDto input)
        {
            var actor = await _actorRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetActorForEditOutput { Actor = ObjectMapper.Map<CreateOrEditActorDto>(actor) };

            var documentFile =
                   await _documentFileRepository.FirstOrDefaultAsync(x => x.ActorId == input.Id);
            if (documentFile != null)
            {
                output.Actor.CreateOrEditDocumentFileDto =
                    ObjectMapper.Map<CreateOrEditDocumentFileDto>(documentFile);
            }
            else
            {
                output.Actor.CreateOrEditDocumentFileDto = new CreateOrEditDocumentFileDto
                {
                    ActorId = actor.Id
                };
                output.Actor.CreateOrEditDocumentFileDto.DocumentTypeDto =
                    ObjectMapper.Map<DocumentTypeDto>(null);
            }
            return output;
        }

        public async Task CreateOrEdit(CreateOrEditActorDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Administration_Actors_Create)]
        protected virtual async Task Create(CreateOrEditActorDto input)
        {
            var organizationUnit = new OrganizationUnit(AbpSession.TenantId, input.CompanyName);
            organizationUnit.Code = await _organizationUnitManager.GetNextChildCodeAsync(organizationUnit.ParentId);

            var organizationUnitId = await _organizationUnitRepository.InsertAndGetIdAsync(organizationUnit);

            await CurrentUnitOfWork.SaveChangesAsync();


            var actor = ObjectMapper.Map<Actor>(input);
            actor.OrganizationUnitId = organizationUnitId;

            if (AbpSession.TenantId != null)
            {
                actor.TenantId = (int)AbpSession.TenantId;
            }



            var requiredDocs = await _documentFilesAppService.GetActorShipperRequiredDocumentFiles("");
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



            var actorId = await _actorRepository.InsertAndGetIdAsync(actor);


            foreach (var item in input.CreateOrEditDocumentFileDtos)
            {
                item.ActorId = actorId;
                item.Name = item.Name + "_" + actorId.ToString();
                await _documentFilesAppService.CreateOrEdit(item);
            }



            var docFileDto = input.CreateOrEditDocumentFileDto;
            if (docFileDto != null)
            {
                docFileDto.Name = "logo_" + actorId;
                docFileDto.ActorId = actorId;
                await _documentFilesAppService.CreateOrEdit(docFileDto);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Actors_Edit)]
        protected virtual async Task Update(CreateOrEditActorDto input)
        {
            var actor = await _actorRepository.FirstOrDefaultAsync((int)input.Id);
            if (input.CreateOrEditDocumentFileDto?.UpdateDocumentFileInput != null)
            {

                input.CreateOrEditDocumentFileDto.Name = "logo_" + actor.Id;

                await _documentFilesAppService.CreateOrEdit(input.CreateOrEditDocumentFileDto);
            }
            ObjectMapper.Map(input, actor);

        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Actors_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _actorRepository.DeleteAsync(input.Id);
        }


        public async Task<bool> GenerateShipperInvoices(int actorId)
        {
            return await _actorInvoicesManager.BuildActorShipperInvoices(actorId);

        }


        public async Task<bool> GenerateCarrierInvoices(int actorId)
        {
            return await _actorInvoicesManager.BuildActorCarrierInvoices(actorId);

        }

    }
}