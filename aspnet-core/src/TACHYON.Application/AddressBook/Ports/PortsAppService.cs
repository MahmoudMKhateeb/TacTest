using TACHYON.Cities;
					using System.Collections.Generic;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.AddressBook.Ports.Exporting;
using TACHYON.AddressBook.Ports.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.AddressBook.Ports
{
	[AbpAuthorize(AppPermissions.Pages_Ports)]
    public class PortsAppService : TACHYONAppServiceBase, IPortsAppService
    {
		 private readonly IRepository<Port, long> _portRepository;
		 private readonly IPortsExcelExporter _portsExcelExporter;
		 private readonly IRepository<City,int> _lookup_cityRepository;
		 

		  public PortsAppService(IRepository<Port, long> portRepository, IPortsExcelExporter portsExcelExporter , IRepository<City, int> lookup_cityRepository) 
		  {
			_portRepository = portRepository;
			_portsExcelExporter = portsExcelExporter;
			_lookup_cityRepository = lookup_cityRepository;
		
		  }

		 public async Task<PagedResultDto<GetPortForViewDto>> GetAll(GetAllPortsInput input)
         {
			
			var filteredPorts = _portRepository.GetAll()
						.Include( e => e.CityFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Address.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AdressFilter),  e => e.Address == input.AdressFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter), e => e.CityFk != null && e.CityFk.DisplayName == input.CityDisplayNameFilter);

			var pagedAndFilteredPorts = filteredPorts
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var ports = from o in pagedAndFilteredPorts
                         join o1 in _lookup_cityRepository.GetAll() on o.CityId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetPortForViewDto() {
							Port = new PortDto
							{
                                Name = o.Name,
                                Address = o.Address,
                                Location=o.Location,
                                Id = o.Id
							},
                         	CityDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
						};

            var totalCount = await filteredPorts.CountAsync();

            return new PagedResultDto<GetPortForViewDto>(
                totalCount,
                await ports.ToListAsync()
            );
         }
		 
		 public async Task<GetPortForViewDto> GetPortForView(long id)
         {
            var port = await _portRepository.GetAsync(id);

            var output = new GetPortForViewDto { Port = ObjectMapper.Map<PortDto>(port) };

		    if (output.Port.CityId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.Port.CityId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Ports_Edit)]
		 public async Task<GetPortForEditOutput> GetPortForEdit(EntityDto<long> input)
         {
            var port = await _portRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetPortForEditOutput {Port = ObjectMapper.Map<CreateOrEditPortDto>(port)};

		    if (output.Port.CityId != null)
            {
                var _lookupCity = await _lookup_cityRepository.FirstOrDefaultAsync((int)output.Port.CityId);
                output.CityDisplayName = _lookupCity?.DisplayName?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditPortDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Ports_Create)]
		 protected virtual async Task Create(CreateOrEditPortDto input)
         {
            var port = ObjectMapper.Map<Port>(input);

			

            await _portRepository.InsertAsync(port);
         }

		 [AbpAuthorize(AppPermissions.Pages_Ports_Edit)]
		 protected virtual async Task Update(CreateOrEditPortDto input)
         {
            var port = await _portRepository.FirstOrDefaultAsync((long)input.Id);
             ObjectMapper.Map(input, port);
         }

		 [AbpAuthorize(AppPermissions.Pages_Ports_Delete)]
         public async Task Delete(EntityDto<long> input)
         {
            await _portRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetPortsToExcel(GetAllPortsForExcelInput input)
         {
			
			var filteredPorts = _portRepository.GetAll()
						.Include( e => e.CityFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Address.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name == input.NameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.AdressFilter),  e => e.Address == input.AdressFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.CityDisplayNameFilter), e => e.CityFk != null && e.CityFk.DisplayName == input.CityDisplayNameFilter);

			var query = (from o in filteredPorts
                         join o1 in _lookup_cityRepository.GetAll() on o.CityId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetPortForViewDto() { 
							Port = new PortDto
							{
                                Name = o.Name,
                                Address = o.Address,
                                Location=o.Location,
                                Id = o.Id
							},
                         	CityDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
						 });


            var portListDtos = await query.ToListAsync();

            return _portsExcelExporter.ExportToFile(portListDtos);
         }


			[AbpAuthorize(AppPermissions.Pages_Ports)]
			public async Task<List<PortCityLookupTableDto>> GetAllCityForTableDropdown()
			{
				return await _lookup_cityRepository.GetAll()
					.Select(city => new PortCityLookupTableDto
					{
						Id = city.Id,
						DisplayName = city == null || city.DisplayName == null ? "" : city.DisplayName.ToString()
					}).ToListAsync();
			}
							
    }
}