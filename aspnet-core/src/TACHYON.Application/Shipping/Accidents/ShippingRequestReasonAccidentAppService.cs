using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Common.Dto;
using TACHYON.Dto;
using TACHYON.Exporting;
using TACHYON.Extension;
using TACHYON.Shipping.Accidents.Dto;

namespace TACHYON.Shipping.Accidents
{
    [AbpAuthorize(AppPermissions.Pages_ShippingRequestResoneAccidents)]

    public class ShippingRequestReasonAccidentAppService : TACHYONAppServiceBase, IShippingRequestReasonAccidentAppService
    {
        private readonly IRepository<ShippingRequestReasonAccident> _ShippingRequestReasonAccidentRepository;
        private readonly IExcelExporterManager<ShippingRequestReasonAccidentListDto> _excelExporterManager;

        public ShippingRequestReasonAccidentAppService(IRepository<ShippingRequestReasonAccident> ShippingRequestCauseAccidentRepository,
            IExcelExporterManager<ShippingRequestReasonAccidentListDto> excelExporterManager)
        {
            _ShippingRequestReasonAccidentRepository = ShippingRequestCauseAccidentRepository;
            _excelExporterManager = excelExporterManager;
        }
        public ListResultDto<ShippingRequestReasonAccidentListDto> GetAll(FilterInput Input)
        {
            var query = GetReason(Input);

            return new ListResultDto<ShippingRequestReasonAccidentListDto>(
                ObjectMapper.Map<List<ShippingRequestReasonAccidentListDto>>(query)

            );
        }
        public async Task<CreateOrEditShippingRequestReasonAccidentDto> GetForEdit(EntityDto input)
        {
            return ObjectMapper.Map<CreateOrEditShippingRequestReasonAccidentDto>(await
                _ShippingRequestReasonAccidentRepository.GetAllIncluding(x => x.Translations)
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
            var ReasonAccident = await _ShippingRequestReasonAccidentRepository.GetAllIncluding(x => x.Translations).SingleAsync(e => e.Id == input.Id);
            ReasonAccident.Translations.Clear();
            ObjectMapper.Map(input, ReasonAccident);

        }

        public FileDto Exports(FilterInput Input)
        {
            string[] HeaderText;
            Func<ShippingRequestReasonAccidentListDto, object>[] propertySelectors;
            HeaderText = new string[] { "Id", "Name" };
            propertySelectors = new Func<ShippingRequestReasonAccidentListDto, object>[] { _ => _.Id, _ => _.Name };
            var ReasonListDto = ObjectMapper.Map<List<ShippingRequestReasonAccidentListDto>>(GetReason(Input));
            return _excelExporterManager.ExportToFile(ReasonListDto, "Reasons", HeaderText, propertySelectors);

        }

        [AbpAuthorize(AppPermissions.Pages_ShippingRequestResoneAccidents_Delete)]

        public async Task Delete(EntityDto input)
        {
            var reasonAccident = await _ShippingRequestReasonAccidentRepository
                .SingleAsync(x => x.Id == input.Id);

            if (reasonAccident.Key.ToLower().Contains(TACHYONConsts.OthersDisplayName))
                throw new UserFriendlyException(L("OtherReasonAccidentNotRemovable"));

            await _ShippingRequestReasonAccidentRepository.DeleteAsync(reasonAccident);
        }

        #region Helper
        private IQueryable<ShippingRequestReasonAccident> GetReason(FilterInput Input)
        {
            return _ShippingRequestReasonAccidentRepository
                .GetAllIncluding(x => x.Translations)
                .AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(Input.Filter), e => e.Translations.Any(x => x.Name.Contains(Input.Filter.Trim())))
                .OrderBy(!string.IsNullOrEmpty(Input.Sorting) ? Input.Sorting : "id asc");
        }
        #endregion
    }
}