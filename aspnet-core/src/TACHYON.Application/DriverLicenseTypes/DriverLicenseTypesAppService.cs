

using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.DriverLicenseTypes.Dtos;
using TACHYON.Dto;

namespace TACHYON.DriverLicenseTypes
{
    public class DriverLicenseTypesAppService : TACHYONAppServiceBase, IDriverLicenseTypesAppService
    {
        private readonly IRepository<DriverLicenseType> _driverLicenseTypeRepository;


        public DriverLicenseTypesAppService(IRepository<DriverLicenseType> driverLicenseTypeRepository)
        {
            _driverLicenseTypeRepository = driverLicenseTypeRepository;

        }

        public async Task<PagedResultDto<GetDriverLicenseTypeForViewDto>> GetAll(GetAllDriverLicenseTypesInput input)
        {

            var filteredDriverLicenseTypes = _driverLicenseTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter));

            var pagedAndFilteredDriverLicenseTypes = filteredDriverLicenseTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var driverLicenseTypes = from o in pagedAndFilteredDriverLicenseTypes
                                     select new GetDriverLicenseTypeForViewDto()
                                     {
                                         DriverLicenseType = new DriverLicenseTypeDto
                                         {
                                             Name = o.Name,
                                             WasIIntegrationId = o.WasIIntegrationId,
                                             ApplicableforWaslRegistration = o.ApplicableforWaslRegistration,
                                             Id = o.Id
                                         }
                                     };

            var totalCount = await filteredDriverLicenseTypes.CountAsync();

            return new PagedResultDto<GetDriverLicenseTypeForViewDto>(
                totalCount,
                await driverLicenseTypes.ToListAsync()
            );
        }

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Edit)]
        public async Task<GetDriverLicenseTypeForEditOutput> GetDriverLicenseTypeForEdit(EntityDto input)
        {
            var driverLicenseType = await _driverLicenseTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetDriverLicenseTypeForEditOutput { DriverLicenseType = ObjectMapper.Map<CreateOrEditDriverLicenseTypeDto>(driverLicenseType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditDriverLicenseTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Create)]
        protected virtual async Task Create(CreateOrEditDriverLicenseTypeDto input)
        {
            var driverLicenseType = ObjectMapper.Map<DriverLicenseType>(input);



            await _driverLicenseTypeRepository.InsertAsync(driverLicenseType);
        }

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Edit)]
        protected virtual async Task Update(CreateOrEditDriverLicenseTypeDto input)
        {
            var driverLicenseType = await _driverLicenseTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, driverLicenseType);
        }

        [AbpAuthorize(AppPermissions.Pages_DriverLicenseTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _driverLicenseTypeRepository.DeleteAsync(input.Id);
        }


        public async Task<List<SelectItemDto>> GetForDropDownList()
        {
            DisableTenancyFilters();
            return await _driverLicenseTypeRepository
                .GetAll()
                .Select(x => new SelectItemDto(x.Id.ToString(), x.Name,false))
                .ToListAsync();
        }
    }
}