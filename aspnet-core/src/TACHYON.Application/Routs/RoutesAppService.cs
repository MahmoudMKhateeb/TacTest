using TACHYON.Routs.RoutTypes;
					using System.Collections.Generic;


using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Routs.Exporting;
using TACHYON.Routs.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Routs
{
	[AbpAuthorize(AppPermissions.Pages_Routes)]
    public class RoutesAppService : TACHYONAppServiceBase, IRoutesAppService
    {
		 private readonly IRepository<Route> _routeRepository;
		 private readonly IRoutesExcelExporter _routesExcelExporter;
		 private readonly IRepository<RoutType,int> _lookup_routTypeRepository;
		 

		  public RoutesAppService(IRepository<Route> routeRepository, IRoutesExcelExporter routesExcelExporter , IRepository<RoutType, int> lookup_routTypeRepository) 
		  {
			_routeRepository = routeRepository;
			_routesExcelExporter = routesExcelExporter;
			_lookup_routTypeRepository = lookup_routTypeRepository;
		
		  }

		 public async Task<PagedResultDto<GetRouteForViewDto>> GetAll(GetAllRoutesInput input)
         {
			
			var filteredRoutes = _routeRepository.GetAll()
						.Include( e => e.RoutTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.RoutTypeDisplayNameFilter), e => e.RoutTypeFk != null && e.RoutTypeFk.DisplayName == input.RoutTypeDisplayNameFilter);

			var pagedAndFilteredRoutes = filteredRoutes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

			var routes = from o in pagedAndFilteredRoutes
                         join o1 in _lookup_routTypeRepository.GetAll() on o.RoutTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetRouteForViewDto() {
							Route = new RouteDto
							{
                                DisplayName = o.DisplayName,
                                Description = o.Description,
                                Id = o.Id
							},
                         	RoutTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
						};

            var totalCount = await filteredRoutes.CountAsync();

            return new PagedResultDto<GetRouteForViewDto>(
                totalCount,
                await routes.ToListAsync()
            );
         }
		 
		 public async Task<GetRouteForViewDto> GetRouteForView(int id)
         {
            var route = await _routeRepository.GetAsync(id);

            var output = new GetRouteForViewDto { Route = ObjectMapper.Map<RouteDto>(route) };

		    if (output.Route.RoutTypeId != null)
            {
                var _lookupRoutType = await _lookup_routTypeRepository.FirstOrDefaultAsync((int)output.Route.RoutTypeId);
                output.RoutTypeDisplayName = _lookupRoutType?.DisplayName?.ToString();
            }
			
            return output;
         }
		 
		 [AbpAuthorize(AppPermissions.Pages_Routes_Edit)]
		 public async Task<GetRouteForEditOutput> GetRouteForEdit(EntityDto input)
         {
            var route = await _routeRepository.FirstOrDefaultAsync(input.Id);
           
		    var output = new GetRouteForEditOutput {Route = ObjectMapper.Map<CreateOrEditRouteDto>(route)};

		    if (output.Route.RoutTypeId != null)
            {
                var _lookupRoutType = await _lookup_routTypeRepository.FirstOrDefaultAsync((int)output.Route.RoutTypeId);
                output.RoutTypeDisplayName = _lookupRoutType?.DisplayName?.ToString();
            }
			
            return output;
         }

		 public async Task CreateOrEdit(CreateOrEditRouteDto input)
         {
            if(input.Id == null){
				await Create(input);
			}
			else{
				await Update(input);
			}
         }

		 [AbpAuthorize(AppPermissions.Pages_Routes_Create)]
		 protected virtual async Task Create(CreateOrEditRouteDto input)
         {
            var route = ObjectMapper.Map<Route>(input);

			
			if (AbpSession.TenantId != null)
			{
				route.TenantId = (int) AbpSession.TenantId;
			}
		

            await _routeRepository.InsertAsync(route);
         }

		 [AbpAuthorize(AppPermissions.Pages_Routes_Edit)]
		 protected virtual async Task Update(CreateOrEditRouteDto input)
         {
            var route = await _routeRepository.FirstOrDefaultAsync((int)input.Id);
             ObjectMapper.Map(input, route);
         }

		 [AbpAuthorize(AppPermissions.Pages_Routes_Delete)]
         public async Task Delete(EntityDto input)
         {
            await _routeRepository.DeleteAsync(input.Id);
         } 

		public async Task<FileDto> GetRoutesToExcel(GetAllRoutesForExcelInput input)
         {
			
			var filteredRoutes = _routeRepository.GetAll()
						.Include( e => e.RoutTypeFk)
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.DisplayName.Contains(input.Filter) || e.Description.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter),  e => e.DisplayName == input.DisplayNameFilter)
						.WhereIf(!string.IsNullOrWhiteSpace(input.RoutTypeDisplayNameFilter), e => e.RoutTypeFk != null && e.RoutTypeFk.DisplayName == input.RoutTypeDisplayNameFilter);

			var query = (from o in filteredRoutes
                         join o1 in _lookup_routTypeRepository.GetAll() on o.RoutTypeId equals o1.Id into j1
                         from s1 in j1.DefaultIfEmpty()
                         
                         select new GetRouteForViewDto() { 
							Route = new RouteDto
							{
                                DisplayName = o.DisplayName,
                                Description = o.Description,
                                Id = o.Id
							},
                         	RoutTypeDisplayName = s1 == null || s1.DisplayName == null ? "" : s1.DisplayName.ToString()
						 });


            var routeListDtos = await query.ToListAsync();

            return _routesExcelExporter.ExportToFile(routeListDtos);
         }


			[AbpAuthorize(AppPermissions.Pages_Routes)]
			public async Task<List<RouteRoutTypeLookupTableDto>> GetAllRoutTypeForTableDropdown()
			{
				return await _lookup_routTypeRepository.GetAll()
					.Select(routType => new RouteRoutTypeLookupTableDto
					{
						Id = routType.Id,
						DisplayName = routType == null || routType.DisplayName == null ? "" : routType.DisplayName.ToString()
					}).ToListAsync();
			}
							
    }
}