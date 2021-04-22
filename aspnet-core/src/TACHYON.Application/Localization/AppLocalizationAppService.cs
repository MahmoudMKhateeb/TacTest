using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Threading;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using TACHYON.Authorization;
using TACHYON.Dto;
using TACHYON.Exporting;
using TACHYON.Localization.Dto;

namespace TACHYON.Localization
{
    [AbpAuthorize(AppPermissions.Pages_AppLocalization)]
    public class AppLocalizationAppService : TACHYONAppServiceBase, IAppLocalizationAppService
    {
        private readonly IRepository<AppLocalization> _appLocalizationRepository;
        private readonly IExcelExporterManager<AppLocalizationListDto> _excelExporterManager;
        private readonly AppLocalizationManager _appLocalizationManager;

        public AppLocalizationAppService(IRepository<AppLocalization> AppLocalizationRepository,
            IExcelExporterManager<AppLocalizationListDto> excelExporterManager,
            AppLocalizationManager appLocalizationManager)
        {
            _appLocalizationRepository = AppLocalizationRepository;
            _excelExporterManager = excelExporterManager;
            _appLocalizationManager = appLocalizationManager;
        }
        public async Task<PagedResultDto<AppLocalizationListDto>> GetAll(AppLocalizationFilterInput Input)
        {
            var query = GetLocalization(Input);
            var ResultPage = await query.PageBy(Input).ToListAsync();

            var totalCount = await query.CountAsync();
            return new PagedResultDto<AppLocalizationListDto>(
                totalCount,
                ObjectMapper.Map<List<AppLocalizationListDto>>(ResultPage)
            );
        }

        public async Task<CreateOrEditAppLocalizationDto> GetForEdit(EntityDto input)
        {
            return ObjectMapper.Map<CreateOrEditAppLocalizationDto>(await
                _appLocalizationRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(e => e.Id == input.Id));
        }
        public async Task CreateOrEdit(CreateOrEditAppLocalizationDto input)
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

        [AbpAuthorize(AppPermissions.Pages_AppLocalization_Delete)]

        public async Task Delete(EntityDto input)
        {
            await _appLocalizationRepository.DeleteAsync(input.Id);
        }

        public FileDto Exports(AppLocalizationFilterInput Input)
        {
            string[] HeaderText;
            Func<AppLocalizationListDto, object>[] propertySelectors;
            HeaderText = new string[] { "Id", "MasterKey", "MasterValue","CurrentLanguage" };
            propertySelectors = new Func<AppLocalizationListDto, object>[] { _ => _.Id, _ => _.MasterKey, _ => _.MasterValue, _ => _.Value };

            var LanguageListDto = ObjectMapper.Map<List<AppLocalizationListDto>>(GetLocalization(Input));
            return _excelExporterManager.ExportToFile(LanguageListDto, "Language", HeaderText, propertySelectors);

        }
        [AbpAuthorize(AppPermissions.Pages_AppLocalization_Restore)]

        public Task Restore()
        {
            _appLocalizationManager.Restore();
            return Task.FromResult(0);
        }
        [AbpAuthorize(AppPermissions.Pages_AppLocalization_Generate)]
        public async Task Generate()
        {
            await _appLocalizationManager.Generate();
        }
        #region Heleper
        [AbpAuthorize(AppPermissions.Pages_AppLocalization_Create)]

        private async Task Create(CreateOrEditAppLocalizationDto input)
        {
            var Reason = ObjectMapper.Map<AppLocalization>(input);

            await _appLocalizationRepository.InsertAsync(Reason);
        }
        [AbpAuthorize(AppPermissions.Pages_AppLocalization_Edit)]

        private async Task Update(CreateOrEditAppLocalizationDto input)
        {
            var Reason = await _appLocalizationRepository.GetAllIncluding(x => x.Translations).SingleAsync(e => e.Id == input.Id);
            Reason.Translations.Clear();
            ObjectMapper.Map(input, Reason);

        }

        private IQueryable<AppLocalization> GetLocalization(AppLocalizationFilterInput Input)
        {
            if (!string.IsNullOrEmpty(Input.Filter)) Input.Filter = Input.Filter.Trim().ToLower();

            return _appLocalizationRepository
             .GetAllIncluding(x => x.Translations)
              .AsNoTracking()
              .WhereIf(!string.IsNullOrWhiteSpace(Input.Filter), e => e.MasterKey.ToLower().Contains(Input.Filter) || e.MasterValue.ToLower().Contains(Input.Filter) || e.Translations.Any(x => x.Value.ToLower().Contains(Input.Filter)))
              .OrderBy(!string.IsNullOrEmpty(Input.Sorting) ? Input.Sorting : "id asc");
        }




        #endregion
    }

}
