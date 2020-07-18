using TACHYON.Trucks.TrucksTypes;
					using System.Collections.Generic;
using TACHYON.Trucks;
					using System.Collections.Generic;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using TACHYON.Trucks.Exporting;
using TACHYON.Trucks.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using TACHYON.Features;

namespace TACHYON.Trucks
{
	[AbpAuthorize(AppPermissions.Pages_Trucks)]
    [RequiresFeature(AppFeatures.Shipper)]
    public class TrucksAppService : TACHYONAppServiceBase, ITrucksAppService
    {
		 private readonly IRepository<Truck, Guid> _truckRepository;
		 private readonly ITrucksExcelExporter _trucksExcelExporter;
		 private readonly IRepository<TrucksType,Guid> _lookup_trucksTypeRepository;
		 private readonly IRepository<TruckStatus,Guid> _lookup_truckStatusRepository;
		 private readonly IRepository<User,long> _lookup_userRepository;
		 

		  public TrucksAppService(IRepository<Truck, Guid> truckRepository, ITrucksExcelExporter trucksExcelExporter , IRepository<TrucksType, Guid> lookup_trucksTypeRepository, IRepository<TruckStatus, Guid> lookup_truckStatusRepository, IRepository<User, long> lookup_userRepository) 
		  {
			_truckRepository = truckRepository;
			_trucksExcelExporter = trucksExcelExporter;
			_lookup_trucksTypeRepository = lookup_trucksTypeRepository;
		_lookup_truckStatusRepository = lookup_truckStatusRepository;
		_lookup_userRepository = lookup_userRepository;
		
		  }

		 public async Task<PagedResultDto<GetTruckForViewDto>> GetAll(GetAllTrucksInput input)
         {
			
			var filteredTrucks = _truckRepository.GetAll()
						.Include( e => e.TrucksTypeFk)
						.Include( e => e.TruckStatusFk)
						.Include( e => e.Driver1UserFk)
						.Include( e => e.Driver2UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.PlateNumber.Contains(input.Filter) || e.ModelName.Contains(input.Filter) || e.ModelYear.Contains(input.Filter) || e.LicenseNumber.Contains(input.Filter) || e.Note.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter),  e => e.PlateNumber == input.PlateNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ModelNameFilter),  e => e.ModelName == input.ModelNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ModelYearFilter),  e => e.ModelYear == input.ModelYearFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.LicenseNumberFilter),  e => e.LicenseNumber == input.LicenseNumberFilter)
						.WhereIf(input.MinLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate >= input.MinLicenseExpirationDateFilter)
						.WhereIf(input.MaxLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate <= input.MaxLicenseExpirationDateFilter)
						.WhereIf(input.IsAttachableFilter > -1,  e => (input.IsAttachableFilter == 1 && e.IsAttachable) || (input.IsAttachableFilter == 0 && !e.IsAttachable) )
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
                         
                         select new GetTruckForViewDto() {
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
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((Guid)output.Truck.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

		    if (output.Truck.TruckStatusId != null)
            {
                var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync((Guid)output.Truck.TruckStatusId);
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
           
		    var output = new GetTruckForEditOutput {Truck = ObjectMapper.Map<CreateOrEditTruckDto>(truck)};

		    if (output.Truck.TrucksTypeId != null)
            {
                var _lookupTrucksType = await _lookup_trucksTypeRepository.FirstOrDefaultAsync((Guid)output.Truck.TrucksTypeId);
                output.TrucksTypeDisplayName = _lookupTrucksType?.DisplayName?.ToString();
            }

		    if (output.Truck.TruckStatusId != null)
            {
                var _lookupTruckStatus = await _lookup_truckStatusRepository.FirstOrDefaultAsync((Guid)output.Truck.TruckStatusId);
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
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Trucks_Create)]
		 protected virtual async Task Create(CreateOrEditTruckDto input)
         {
            var truck = ObjectMapper.Map<Truck>(input);

			
			if (AbpSession.TenantId != null)
			{
				truck.TenantId = (int) AbpSession.TenantId;
			}
		

            await _truckRepository.InsertAsync(truck);
         }

		 [AbpAuthorize(AppPermissions.Pages_Trucks_Edit)]
		 protected virtual async Task Update(CreateOrEditTruckDto input)
         {
            var truck = await _truckRepository.FirstOrDefaultAsync((Guid)input.Id);
             ObjectMapper.Map(input, truck);
         }

		 [AbpAuthorize(AppPermissions.Pages_Trucks_Delete)]
         public async Task Delete(EntityDto<Guid> input)
         {
            await _truckRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetTrucksToExcel(GetAllTrucksForExcelInput input)
         {
			
			var filteredTrucks = _truckRepository.GetAll()
						.Include( e => e.TrucksTypeFk)
						.Include( e => e.TruckStatusFk)
						.Include( e => e.Driver1UserFk)
						.Include( e => e.Driver2UserFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.PlateNumber.Contains(input.Filter) || e.ModelName.Contains(input.Filter) || e.ModelYear.Contains(input.Filter) || e.LicenseNumber.Contains(input.Filter) || e.Note.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.PlateNumberFilter),  e => e.PlateNumber == input.PlateNumberFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ModelNameFilter),  e => e.ModelName == input.ModelNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.ModelYearFilter),  e => e.ModelYear == input.ModelYearFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.LicenseNumberFilter),  e => e.LicenseNumber == input.LicenseNumberFilter)
						.WhereIf(input.MinLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate >= input.MinLicenseExpirationDateFilter)
						.WhereIf(input.MaxLicenseExpirationDateFilter != null, e => e.LicenseExpirationDate <= input.MaxLicenseExpirationDateFilter)
						.WhereIf(input.IsAttachableFilter > -1,  e => (input.IsAttachableFilter == 1 && e.IsAttachable) || (input.IsAttachableFilter == 0 && !e.IsAttachable) )
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
                         
                         select new GetTruckForViewDto() { 
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
				return await _lookup_trucksTypeRepository.GetAll()
					.Select(trucksType => new TruckTrucksTypeLookupTableDto
					{
						Id = trucksType.Id.ToString(),
						DisplayName = trucksType == null || trucksType.DisplayName == null ? "" : trucksType.DisplayName.ToString()
					}).ToListAsync();
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
                   e=> e.Name != null && e.Name.Contains(input.Filter)
                );

            var totalCount = await query.CountAsync();

            var userList = await query
                .PageBy(input)
                .ToListAsync();

			var lookupTableDtoList = new List<TruckUserLookupTableDto>();
			foreach(var user in userList){
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
    }
}