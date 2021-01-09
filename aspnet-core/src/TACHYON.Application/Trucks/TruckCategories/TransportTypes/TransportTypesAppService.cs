

using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Dto;
using Abp.Application.Services.Dto;
using TACHYON.Authorization;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TACHYON.Trucks.TruckCategories.TransportTypes
{
    [AbpAuthorize(AppPermissions.Pages_TransportTypes)]
    public class TransportTypesAppService : TACHYONAppServiceBase, ITransportTypesAppService
    {
        private readonly IRepository<TransportType> _transportTypeRepository;


        public TransportTypesAppService(IRepository<TransportType> transportTypeRepository)
        {
            _transportTypeRepository = transportTypeRepository;

        }

        public async Task<PagedResultDto<GetTransportTypeForViewDto>> GetAll(GetAllTransportTypesInput input)
        {

            var filteredTransportTypes = _transportTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.DisplayName.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DisplayNameFilter), e => e.DisplayName == input.DisplayNameFilter);

            var pagedAndFilteredTransportTypes = filteredTransportTypes
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var transportTypes = from o in pagedAndFilteredTransportTypes
                                 select new GetTransportTypeForViewDto()
                                 {
                                     TransportType = new TransportTypeDto
                                     {
                                         DisplayName = o.DisplayName,
                                         Id = o.Id
                                     }
                                 };

            var totalCount = await filteredTransportTypes.CountAsync();

            return new PagedResultDto<GetTransportTypeForViewDto>(
                totalCount,
                await transportTypes.ToListAsync()
            );
        }

        public async Task<GetTransportTypeForViewDto> GetTransportTypeForView(int id)
        {
            var transportType = await _transportTypeRepository
                .GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(x => x.Id == id);

            var output = new GetTransportTypeForViewDto { TransportType = ObjectMapper.Map<TransportTypeDto>(transportType) };

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Edit)]
        public async Task<GetTransportTypeForEditOutput> GetTransportTypeForEdit(EntityDto input)
        {
            var transportType = await _transportTypeRepository.FirstOrDefaultAsync(input.Id);

            var output = new GetTransportTypeForEditOutput { TransportType = ObjectMapper.Map<CreateOrEditTransportTypeDto>(transportType) };

            return output;
        }

        public async Task CreateOrEdit(CreateOrEditTransportTypeDto input)
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

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Create)]
        protected virtual async Task Create(CreateOrEditTransportTypeDto input)
        {
            var transportType = ObjectMapper.Map<TransportType>(input);



            await _transportTypeRepository.InsertAsync(transportType);
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Edit)]
        protected virtual async Task Update(CreateOrEditTransportTypeDto input)
        {
            var transportType = await _transportTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, transportType);
        }

        [AbpAuthorize(AppPermissions.Pages_TransportTypes_Delete)]
        public async Task Delete(EntityDto input)
        {
            await _transportTypeRepository.DeleteAsync(input.Id);
        }
    }
}