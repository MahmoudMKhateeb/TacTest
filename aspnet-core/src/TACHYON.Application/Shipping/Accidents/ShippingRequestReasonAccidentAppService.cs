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
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestResoneAccidents)]

    public class ShippingRequestReasonAccidentAppService : TACHYONAppServiceBase, IShippingRequestReasonAccidentAppService
    {
        private readonly IRepository<ShippingRequestReasonAccident> _ShippingRequestReasonAccidentRepository;

        public ShippingRequestReasonAccidentAppService(IRepository<ShippingRequestReasonAccident> ShippingRequestCauseAccidentRepository)
        {
            _ShippingRequestReasonAccidentRepository = ShippingRequestCauseAccidentRepository;
        }
        public async Task<ListResultDto<ShippingRequestReasonAccidentListDto>> GetAll(GetAllForShippingRequestReasonAccidentFilterInput Input)
        {
            var query = _ShippingRequestReasonAccidentRepository
                .GetAllIncluding(x=>x.Translations)
                .AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(Input.Filter), e => e.Translations.Any(x=> x.Name.Contains(Input.Filter.Trim())))
                .OrderBy(Input.Sorting ?? "id asc");

            var totalCount = await query.CountAsync();
            return new ListResultDto<ShippingRequestReasonAccidentListDto>(
                ObjectMapper.Map<List<ShippingRequestReasonAccidentListDto>>(query)

            );
        }
        public async Task<CreateOrEditShippingRequestReasonAccidentDto> GetForEdit(EntityDto input)
        {
            return ObjectMapper.Map<CreateOrEditShippingRequestReasonAccidentDto>(await 
                _ShippingRequestReasonAccidentRepository.GetAllIncluding(x=>x.Translations)
                .FirstOrDefaultAsync(e => e.Id == input.Id));
        }
        public async Task CreateOrEdit(CreateOrEditShippingRequestReasonAccidentDto input)
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

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestResoneAccidents_Create)]

        private async Task Create(CreateOrEditShippingRequestReasonAccidentDto input)
        {
            var CauseAccident = ObjectMapper.Map<ShippingRequestReasonAccident>(input);

            await _ShippingRequestReasonAccidentRepository.InsertAsync(CauseAccident);
        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestResoneAccidents_Edit)]

        private async Task Update(CreateOrEditShippingRequestReasonAccidentDto input)
        {
            var ReasonAccident = await _ShippingRequestReasonAccidentRepository.GetAllIncluding(x=>x.Translations).SingleAsync(e => e.Id == input.Id);
            ReasonAccident.Translations.Clear();
            ObjectMapper.Map(input, ReasonAccident);

        }
        [AbpAuthorize(AppPermissions.Pages_ShippingRequestResoneAccidents_Delete)]

        public async Task Delete(EntityDto input)
        {
            await _ShippingRequestReasonAccidentRepository.DeleteAsync(input.Id);
        }


    }
}
