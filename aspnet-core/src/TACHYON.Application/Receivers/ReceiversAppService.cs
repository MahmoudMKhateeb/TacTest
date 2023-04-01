using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Castle.MicroKernel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.AddressBook;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Receivers.Dtos;
using TACHYON.Receivers.Exporting;

namespace TACHYON.Receivers
{
    [AbpAuthorize(AppPermissions.Pages_Receivers)]
    [RequiresFeature(AppFeatures.Shipper, AppFeatures.TachyonDealer, AppFeatures.CarrierAsASaas)]
    public class ReceiversAppService : TACHYONAppServiceBase, IReceiversAppService
    {
        private readonly IRepository<Receiver> _receiverRepository;
        private readonly IReceiversExcelExporter _receiversExcelExporter;
        private readonly IRepository<Facility, long> _lookup_facilityRepository;

        public ReceiversAppService(IRepository<Receiver> receiverRepository,
            IReceiversExcelExporter receiversExcelExporter,
            IRepository<Facility, long> lookup_facilityRepository)
        {
            _receiverRepository = receiverRepository;
            _receiversExcelExporter = receiversExcelExporter;
            _lookup_facilityRepository = lookup_facilityRepository;
        }

        public async Task<PagedResultDto<GetReceiverForViewDto>> GetAll(GetAllReceiversInput input)
        {

            DisableTenancyFilters();

            var filteredReceivers = _receiverRepository.GetAll()
                .Include(e => e.FacilityFk)
                .ThenInclude(x=>x.ShipperActorFk)
                .Include(x => x.Tenant)
                .WhereIf(!await IsTachyonDealer(), x => x.TenantId == AbpSession.TenantId)
                .WhereIf(input.FromDate.HasValue && input.ToDate.HasValue,
                    i => i.CreationTime >= input.FromDate && i.CreationTime <= input.ToDate)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.FullName.Contains(input.Filter) || e.Email.Contains(input.Filter) ||
                         e.PhoneNumber.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.FullNameFilter), e => e.FullName == input.FullNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email == input.EmailFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneNumberFilter),
                    e => e.PhoneNumber == input.PhoneNumberFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.FacilityNameFilter),
                    e => e.FacilityFk != null && e.FacilityFk.Name == input.FacilityNameFilter)
                .WhereIf(input.FacilityId != null, e => e.FacilityId == input.FacilityId);

            var pagedAndFilteredReceivers = filteredReceivers
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var receivers = from o in pagedAndFilteredReceivers
                join o1 in _lookup_facilityRepository.GetAll() on o.FacilityId equals o1.Id into j1
                from s1 in j1.DefaultIfEmpty()
                select new GetReceiverForViewDto()
                {
                    Receiver = new ReceiverDto
                    {
                        FullName = o.FullName, Email = o.Email, PhoneNumber = o.PhoneNumber, Id = o.Id
                    },
                    FacilityName = s1 == null || s1.Name == null ? "" : s1.Name.ToString(),
                    CreationTime = o.CreationTime,
                    TenantName=o.Tenant.TenancyName,
                    ActorName = o.FacilityFk.ShipperActorFk != null ? o.FacilityFk.ShipperActorFk.CompanyName : ""
                };

            var totalCount = await filteredReceivers.CountAsync();

            return new PagedResultDto<GetReceiverForViewDto>(
                totalCount,
                await receivers.ToListAsync()
            );
        }

        public async Task<GetReceiverForViewDto> GetReceiverForView(int id)
        {
            var receiver = await _receiverRepository.GetAsync(id);

            var output = new GetReceiverForViewDto { Receiver = ObjectMapper.Map<ReceiverDto>(receiver) };

            if (output.Receiver.FacilityId != null)
            {
                var _lookupFacility =
                    await _lookup_facilityRepository.FirstOrDefaultAsync((int)output.Receiver.FacilityId);
                output.FacilityName = _lookupFacility?.Name?.ToString();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Receivers_Edit)]
        public async Task<GetReceiverForEditOutput> GetReceiverForEdit(EntityDto input)
        {
            var receiver = await _receiverRepository.FirstOrDefaultAsync(input.Id);

            var output =
                new GetReceiverForEditOutput { Receiver = ObjectMapper.Map<CreateOrEditReceiverDto>(receiver) };

            if (output.Receiver.FacilityId != null)
            {
                var _lookupFacility =
                    await _lookup_facilityRepository.FirstOrDefaultAsync((int)output.Receiver.FacilityId);
                output.FacilityName = _lookupFacility?.Name?.ToString();
                output.ShipperActorId = _lookupFacility?.ShipperActorId;
            }

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditReceiverDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Receivers_Create)]
        protected virtual async Task Create(CreateOrEditReceiverDto input)
        {
            var receiver = ObjectMapper.Map<Receiver>(input);

            if ((await IsTachyonDealer() || AbpSession.TenantId == null) && input.tenantId != null)
            {
                receiver.TenantId = input.tenantId.Value;
            }
            else
            {
                receiver.TenantId = AbpSession.TenantId.Value;
            }

            await _receiverRepository.InsertAsync(receiver);
        }

        [AbpAuthorize(AppPermissions.Pages_Receivers_Edit)]
        protected virtual async Task Update(CreateOrEditReceiverDto input)
        {
            var receiver = await _receiverRepository.FirstOrDefaultAsync((int)input.Id);
            if ((await IsTachyonDealer() || AbpSession.TenantId == null) && input.tenantId != null)
            {
                receiver.TenantId = input.tenantId.Value;
            }
            ObjectMapper.Map(input, receiver);
        }

        public async Task<bool> CheckIfPhoneNumberValid(string phoneNumber, long? reciverId)
        {
            var result =
                await _receiverRepository.FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && x.Id != reciverId);
            return (result == null);
        }

        [AbpAuthorize(AppPermissions.Pages_Receivers_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _receiverRepository.DeleteAsync(input.Id);
        }

        public async Task<FileDto> GetReceiversToExcel(GetAllReceiversForExcelInput input)
        {
            var filteredReceivers = _receiverRepository.GetAll()
                .Include(e => e.FacilityFk)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    e => false || e.FullName.Contains(input.Filter) || e.Email.Contains(input.Filter) ||
                         e.PhoneNumber.Contains(input.Filter))
                .WhereIf(!string.IsNullOrWhiteSpace(input.FullNameFilter), e => e.FullName == input.FullNameFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.EmailFilter), e => e.Email == input.EmailFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.PhoneNumberFilter),
                    e => e.PhoneNumber == input.PhoneNumberFilter)
                .WhereIf(!string.IsNullOrWhiteSpace(input.FacilityNameFilter),
                    e => e.FacilityFk != null && e.FacilityFk.Name == input.FacilityNameFilter);

            var query = (from o in filteredReceivers
                join o1 in _lookup_facilityRepository.GetAll() on o.FacilityId equals o1.Id into j1
                from s1 in j1.DefaultIfEmpty()
                select new GetReceiverForViewDto()
                {
                    Receiver = new ReceiverDto
                    {
                        FullName = o.FullName, Email = o.Email, PhoneNumber = o.PhoneNumber, Id = o.Id
                    },
                    FacilityName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
                });

            var receiverListDtos = await query.ToListAsync();

            return _receiversExcelExporter.ExportToFile(receiverListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Receivers)]
        public async Task<List<ReceiverFacilityLookupTableDto>> GetAllFacilityForTableDropdown(int? tenantId)
        {
            DisableTenancyFilters();
            return await _lookup_facilityRepository.GetAll()
                .WhereIf(await IsTachyonDealer(), x=> (x.FacilityType == FacilityType.Facility && x.TenantId == tenantId) || x.FacilityType != FacilityType.Facility)
                .WhereIf(!await IsTachyonDealer(), x => (x.FacilityType == FacilityType.Facility && x.TenantId == AbpSession.TenantId) || x.FacilityType != FacilityType.Facility)
                .Select(facility => new ReceiverFacilityLookupTableDto
                {
                    Id = facility.Id,
                    DisplayName = facility == null || facility.Name == null ? "" : facility.Name.ToString()
                }).ToListAsync();
        }
        [AbpAuthorize(AppPermissions.Pages_Receivers)]
        [RequiresFeature(AppFeatures.ShipperClients)]
        public async Task<List<ReceiverFacilityLookupTableDto>> GetAllFacilitiesByActorId(int actorId)
        {
            DisableTenancyFilters();
            return await _lookup_facilityRepository.GetAll()
                .Where(x => (x.TenantId == AbpSession.TenantId) || x.FacilityType != FacilityType.Facility)
                .Where(x=> x.ShipperActorId == actorId)
                .Select(facility => new ReceiverFacilityLookupTableDto
                {
                    Id = facility.Id,
                    DisplayName = facility == null || facility.Name == null ? "" : facility.Name
                }).ToListAsync();
        }

        public async Task<List<ReceiverFacilityLookupTableDto>> GetAllReceiversByFacilityForTableDropdown(
            long facilityId)
        {
            //if (await FeatureChecker.IsEnabledAsync(AppFeatures.TachyonDealer))
            //{
                DisableTenancyFilters();
           // }

            return await _receiverRepository.GetAll()
                .WhereIf(!await IsTachyonDealer(), x => (x.FacilityFk.FacilityType == FacilityType.Facility && x.FacilityFk.TenantId == AbpSession.TenantId) || x.FacilityFk.FacilityType != FacilityType.Facility)
                .Where( x => x.FacilityId == facilityId )
                .Select(r => new ReceiverFacilityLookupTableDto { Id = r.Id, DisplayName = r.FullName }).ToListAsync();
        }
    }
}