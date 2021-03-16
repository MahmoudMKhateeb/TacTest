using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.Shipping.Accidents
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestCauseAccidents)]

    public class ShippingRequestCauseAccidentAppService : TACHYONAppServiceBase, IShippingRequestCauseAccidentAppService
    {
        private readonly IRepository<ShippingRequestCauseAccident> _ShippingRequestCauseAccidentRepository;

        public ShippingRequestCauseAccidentAppService(IRepository<ShippingRequestCauseAccident> ShippingRequestCauseAccidentRepository)
        {
            _ShippingRequestCauseAccidentRepository = ShippingRequestCauseAccidentRepository;
        }
        public async Task<PagedResultDto<ShippingRequestCauseAccidentListDto>> GetAll(GetAllForShippingRequestCauseAccidentFilterInput Input)
        {
            var query = _ShippingRequestCauseAccidentRepository
                .GetAll()
                .AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(Input.Filter), e => e.DisplayName.Contains(Input.Filter.Trim()))
                .OrderBy(Input.Sorting ?? "id asc")
                .PageBy(Input);

            var totalCount = await query.CountAsync();
            return new PagedResultDto<ShippingRequestCauseAccidentListDto>(
                totalCount,
                ObjectMapper.Map<List<ShippingRequestCauseAccidentListDto>>(query)

            );
        }
        public async Task<CreateOrEditShippingRequestCauseAccidentDto> GetForEdit(EntityDto input)
        {
            return ObjectMapper.Map<CreateOrEditShippingRequestCauseAccidentDto>(await _ShippingRequestCauseAccidentRepository.FirstOrDefaultAsync(e => e.Id == input.Id));
        }
        public async Task CreateOrEdit(CreateOrEditShippingRequestCauseAccidentDto input)
        {
            if (input.Id == 0)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestCauseAccidents_Create)]

        private async Task Create(CreateOrEditShippingRequestCauseAccidentDto input)
        {
            var CauseAccident = ObjectMapper.Map<ShippingRequestCauseAccident>(input);

            await _ShippingRequestCauseAccidentRepository.InsertAsync(CauseAccident);
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestCauseAccidents_Edit)]

        private async Task Update(CreateOrEditShippingRequestCauseAccidentDto input)
        {
            var CauseAccident = await _ShippingRequestCauseAccidentRepository.SingleAsync(e => e.Id == input.Id);
            ObjectMapper.Map(input, CauseAccident);

        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestCauseAccidents_Delete)]

        public async Task Delete(EntityDto input)
        {
            await _ShippingRequestCauseAccidentRepository.DeleteAsync(input.Id);
        }


    }
}
