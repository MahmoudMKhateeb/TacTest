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
using Abp.Authorization.Roles;
using Abp.Organizations;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using TACHYON.Authorization.Roles;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Invoices.ActorInvoices;
using TACHYON.WebHooks;
using TACHYON.Shipping.ShippingRequestTrips;

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
        private readonly RoleManager _roleManager;
        private readonly AppWebhookPublisher _webhookPublisher;
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;


        public ActorsAppService(
            IRepository<Actor> actorRepository,
            OrganizationUnitManager organizationUnitManager,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            DocumentFilesAppService documentFilesAppService,
            IRepository<DocumentFile, Guid> documentFileRepository,
            ActorInvoicesManager actorInvoicesManager,
            RoleManager roleManager,
            AppWebhookPublisher webhookPublisher,
            IRepository<ShippingRequestTrip> shippingRequestTripRepository)
        {
            _actorRepository = actorRepository;
            _organizationUnitManager = organizationUnitManager;
            _organizationUnitRepository = organizationUnitRepository;
            _documentFilesAppService = documentFilesAppService;
            _documentFileRepository = documentFileRepository;
            _actorInvoicesManager = actorInvoicesManager;
            _roleManager = roleManager;
            _webhookPublisher = webhookPublisher;
            _shippingRequestTripRepository = shippingRequestTripRepository;
        }

        public async Task<PagedResultDto<GetActorForViewDto>> GetAll(GetAllActorsInput input)
        {
            var actorTypeFilter = input.ActorTypeFilter.HasValue
                        ? (ActorTypesEnum)input.ActorTypeFilter
                        : default;

            var filteredActors = _actorRepository.GetAll().Where(x=> x.ActorType != ActorTypesEnum.MySelf)
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
                             Id = o.Id,
                             isactive = o.IsActive,
                              CityId = o.CityId ,
                              FirstName = o.FirstName ,
                            LastName = o.LastName,
                            SalesOfficeType = o.SalesOfficeType,
                            SalesGroup = o.SalesGroup,
                            TrasportationZone = o.TrasportationZone ,
                            Reconsaccoun = o.Reconsaccoun,
                            PostalCode = o.PostalCode,
                            Division = o.Division,
                            District = o.District ,
                            CustomerGroup= o.CustomerGroup ,
                            BuildingCode = o.BuildingCode,
                            AccountType = o.AccountType


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
                        IsActive = o.isactive,
                        CityId = o.CityId ,
                        FirstName = o.FirstName ,
                        LastName = o.LastName,
                        SalesOfficeType = o.SalesOfficeType,
                        SalesGroup = o.SalesGroup,
                        TrasportationZone = o.TrasportationZone ,
                        Reconsaccoun = o.Reconsaccoun,
                        PostalCode = o.PostalCode,
                        Division = o.Division,
                        District = o.District ,
                        CustomerGroup= o.CustomerGroup ,
                        BuildingCode = o.BuildingCode,
                        AccountType = o.AccountType
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
            
            if (actor is { ActorType: ActorTypesEnum.MySelf })
                throw new UserFriendlyException(L("YouCanNotViewThisActor"));
            
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

            if (actor is { ActorType: ActorTypesEnum.MySelf })
                throw new UserFriendlyException(L("YouCanNotEditThisActor"));
            
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
            if (input.ActorType == ActorTypesEnum.Shipper)
            {

                bool isExists = await _roleManager.RoleExistsAsync(StaticRoleNames.Tenants.InternalShipperClients);
                if (!isExists)
                    await CreateInternalShipperClientsRole();
            }
            else if(input.ActorType == ActorTypesEnum.Carrier)
            {
                bool isExists = await _roleManager.RoleExistsAsync(StaticRoleNames.Tenants.InternalCarrierClients);
                if (!isExists)
                    await CreateInternalCarrierClientsRole();
            }


            await CurrentUnitOfWork.SaveChangesAsync();



            int internalClientsRoleId = input.ActorType == ActorTypesEnum.Shipper ? await _roleManager.Roles.Where(x =>
                    x.TenantId == AbpSession.TenantId && x.Name == StaticRoleNames.Tenants.InternalShipperClients)
                .Select(x => x.Id).SingleAsync() : await _roleManager.Roles.Where(x =>
                    x.TenantId == AbpSession.TenantId && x.Name == StaticRoleNames.Tenants.InternalCarrierClients)
                .Select(x => x.Id).SingleAsync();


            await _roleManager.AddToOrganizationUnitAsync(internalClientsRoleId, organizationUnitId,AbpSession.TenantId);

            var actor = ObjectMapper.Map<Actor>(input);
            actor.OrganizationUnitId = organizationUnitId;

            if (AbpSession.TenantId != null)
            {
                actor.TenantId = (int)AbpSession.TenantId;
            }

            var requiredDocs = default(List<CreateOrEditDocumentFileDto>);

            if (input.ActorType == ActorTypesEnum.Shipper)
            {
                requiredDocs = await _documentFilesAppService.GetActorShipperRequiredDocumentFiles("");
            }
            else if (input.ActorType == ActorTypesEnum.Carrier)
            {
                requiredDocs = await _documentFilesAppService.GetActorCarrierRequiredDocumentFiles("");
            }

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

            // publish Actor-Creted-Webhook 
            {
                input.Id = actorId;
                await _webhookPublisher.PublishNewActorCreatedWebhook(input);
            }
        }

        private async Task CreateInternalShipperClientsRole()
        {
            await _roleManager.CreateAsync(new Role()
            {
                TenantId = AbpSession.TenantId,
                DisplayName = "Internal Shipper Client",
                Name = StaticRoleNames.Tenants.InternalShipperClients,
                IsStatic = true,
                Permissions = new List<RolePermissionSetting>()
                {
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Tracking, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Administration, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Administration_ActorsInvoice, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Invoices, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ShippingRequests, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Facilities, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Receivers, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_DocumentFiles, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_DocumentFiles_Actors, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ShippingRequestTrips, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Tenant_Dashboard, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ActorPrices, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ActorPrices_Shipper, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ShipperDashboard, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ShipperDashboard_trackingMap, TenantId = AbpSession.TenantId },


                }
            });
        }

        private async Task CreateInternalCarrierClientsRole()
        {
            await _roleManager.CreateAsync(new Role()
            {
                TenantId = AbpSession.TenantId,
                DisplayName = "Internal Carrier Client",
                Name = StaticRoleNames.Tenants.InternalCarrierClients,
                IsStatic = true,
                Permissions = new List<RolePermissionSetting>()
                {
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Tracking, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Administration, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Administration_SubmitActorsInvoice, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Invoices, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ShippingRequests, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Trucks, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Administration_Users, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Administration_Drivers, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_DocumentFiles, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_DocumentFiles_Actors, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ShippingRequestTrips, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_Tenant_Dashboard, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ActorPrices, TenantId = AbpSession.TenantId },
                    new RolePermissionSetting() { Name = AppPermissions.Pages_ActorPrices_Carrier, TenantId = AbpSession.TenantId },
                    //new RolePermissionSetting() { Name = AppPermissions.Pages_CarrierDashboard, TenantId = AbpSession.TenantId },
                }
            });
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
            var actor = await _actorRepository.GetAll().Where(x => x.Id == input.Id)
                .Select(x => new {x.OrganizationUnitId,x.ActorType}).FirstOrDefaultAsync();

            // Check if the actor has trips
            var actorHasTrips = await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShipperActorId == input.Id)
                .AnyAsync();

            if (actorHasTrips)
            {
                throw new UserFriendlyException(L("ActorLinkedToTripsCannotBeDeleted"));
            }

            if (actor?.ActorType == ActorTypesEnum.MySelf)
                throw new UserFriendlyException(L("ThisActorCanNotBeDeleted"));

            if (actor?.OrganizationUnitId != default)
                await _organizationUnitRepository.DeleteAsync(actor.OrganizationUnitId);
            
            await _actorRepository.DeleteAsync(input.Id);
        }

        #region Invoices

        public async Task<bool> GenerateActorInvoice(int actorId, List<SelectItemDto> trips)
        {
            var actor = await _actorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == actorId);

            if (actor is { ActorType: ActorTypesEnum.MySelf })
                throw new UserFriendlyException(L("ThisActorCanNotBeInvoiced"));
            
            if (actor.ActorType == ActorTypesEnum.Shipper)
            {
                return await _actorInvoicesManager.BuildActorShipperInvoices(actorId, trips);
            }
            else
            {
                return await _actorInvoicesManager.BuildActorCarrierInvoices(actorId, trips);

            }
        }

        #endregion

        #region SAB

        public async Task<GetActorByPurchNoCDto> GetActorByPurchNoC(int purchNoC){
            
              return await  _actorRepository.GetAll()
                .Where(x => x.Id == purchNoC)
                .Select(x=> new GetActorByPurchNoCDto {
                    Salesoffice = x.SalesOfficeType,
                    Street = x.Address,
                    PostalCode = x.PostalCode,
                    Phone = x.MobileNumber,
                    Vatregisteration = x.VatCertificate,
                    Registeration = x.CR,
                    Lastname = x.LastName,
                    Firstname = x.FirstName,
                    District = x.District,
                    Dischannel = x.ActorType,
                    City = x.CityFk.DisplayName,
                    BuildingCode = x.BuildingCode
                }).FirstOrDefaultAsync();
        }
        #endregion

        #region DropDowns

        public async Task<List<SelectItemDto>> GetAllActorsForDropDown(ActorTypesEnum? actorType)
        {
            return await _actorRepository.GetAll()
                .WhereIf(actorType != null, x => x.ActorType == actorType)
                 .Where(x => x.IsActive)
                   .Select(x => new SelectItemDto()
                   {
                       Id = x.Id.ToString(),
                       DisplayName = x.CompanyName
                   }).ToListAsync();
        }

        public async Task<List<SelectItemDto>> GetAllUnInvoicedWaybillsForActor(int id)
        {
            var actor= await _actorRepository.GetAll()
                 .FirstOrDefaultAsync(x => x.Id == id);
            
            if (actor is { ActorType: ActorTypesEnum.MySelf })
                throw new UserFriendlyException(L("ThisActorCanNotHaveInvoices"));

            if(actor.ActorType == ActorTypesEnum.Shipper)
            {
                return (await _actorInvoicesManager.GetAllShipperActorUnInvoicedTrips(id, null))
                    .Select(y => new SelectItemDto { DisplayName = y.WaybillNumber.ToString(), Id = y.Id.ToString() })
                    .ToList();
            }
            else
            {
                return (await _actorInvoicesManager.GetAllCarrierActorUnInvoicedTrips(id, null))
                    .Select(y => new SelectItemDto { DisplayName = y.WaybillNumber.ToString(), Id = y.Id.ToString() })
                    .ToList();
            }
        }

        #endregion


    }
}