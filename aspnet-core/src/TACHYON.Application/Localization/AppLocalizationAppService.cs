using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Threading;
using Abp.UI;
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
        private readonly IRepository<TerminologieEdition> _terminologieEditionRepository;
        private readonly IRepository<TerminologiePage> _terminologiePageRepository;

        private readonly IExcelExporterManager<AppLocalizationListDto> _excelExporterManager;
        private readonly AppLocalizationManager _appLocalizationManager;


        public AppLocalizationAppService(
            IRepository<AppLocalization> AppLocalizationRepository,
            IRepository<TerminologieEdition> terminologieEditionRepository,
            IRepository<TerminologiePage> terminologiePageRepository,
            IExcelExporterManager<AppLocalizationListDto> excelExporterManager,
            AppLocalizationManager appLocalizationManager)
        {
            _appLocalizationRepository = AppLocalizationRepository;
            _excelExporterManager = excelExporterManager;
            _appLocalizationManager = appLocalizationManager;
            _terminologieEditionRepository = terminologieEditionRepository;
            _terminologiePageRepository = terminologiePageRepository;
        }
        public async Task<PagedResultDto<AppLocalizationListDto>> GetAll(AppLocalizationFilterInput Input)
        {
            var query = GetLocalization(Input);
            var ResultPage = await query.PageBy(Input).ToListAsync();
            var ResultPageDto = ObjectMapper.Map<List<AppLocalizationListDto>>(ResultPage);

            var totalCount = await query.CountAsync();
            return new PagedResultDto<AppLocalizationListDto>(
                totalCount,
                ResultPageDto
            );
        }
        public async Task<AppLocalizationForViewDto> GetForView(EntityDto input)
        {
            var query = await
                _appLocalizationRepository
                .GetAllIncluding(x => x.Translations, x => x.TerminologiePages)
                .Include(x => x.TerminologieEditions)
                    .ThenInclude(e => e.Edition)
                .FirstOrDefaultAsync(e => e.Id == input.Id);
            return ObjectMapper.Map<AppLocalizationForViewDto>(query);
        }
        public async Task<CreateOrEditAppLocalizationDto> GetForEdit(EntityDto input)
        {
            return ObjectMapper.Map<CreateOrEditAppLocalizationDto>(await
                _appLocalizationRepository.GetAllIncluding(x => x.Translations)
                .FirstOrDefaultAsync(e => e.Id == input.Id));
        }
        public async Task CreateOrEdit(CreateOrEditAppLocalizationDto input)
        {
            await CheckKeyIsExists(input);
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
            HeaderText = new string[] { "Id", "MasterKey", "MasterValue", "CurrentLanguage" };
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

        [AbpAllowAnonymous]
        public async Task CreateOrUpdateKeyLog(TerminologieMonitorInput input)
        {
            var Terminologie = await _appLocalizationRepository.FirstOrDefaultAsync(x => x.MasterKey.ToLower() == input.Key.ToLower());
            if (Terminologie == null)
            {
                await CreateKeyLog(input);
            }
            else
            {
                await UpdateKeyLog(input, Terminologie);
            }
        }


        #region Heleper
        [AbpAuthorize(AppPermissions.Pages_AppLocalization_Create)]

        private async Task Create(CreateOrEditAppLocalizationDto input)
        {
            input.MasterKey = FirstCharToUpper(input.MasterKey);
            var key = ObjectMapper.Map<AppLocalization>(input);

            await _appLocalizationRepository.InsertAsync(key);
        }
        [AbpAuthorize(AppPermissions.Pages_AppLocalization_Edit)]

        private async Task Update(CreateOrEditAppLocalizationDto input)
        {
            var key = await _appLocalizationRepository.GetAllIncluding(x => x.Translations).SingleAsync(e => e.Id == input.Id);
            key.Translations.Clear();
            ObjectMapper.Map(input, key);

        }
        private async Task CheckKeyIsExists(CreateOrEditAppLocalizationDto input)
        {
            if (await _appLocalizationRepository.GetAll().AnyAsync(x => x.Id != input.Id && x.MasterKey.ToLower() == input.MasterKey.ToLower()))
            {
                throw new UserFriendlyException(L("TheKeyAlreadyExists"));

            }
        }
        private IQueryable<AppLocalization> GetLocalization(AppLocalizationFilterInput Input)
        {
            if (!string.IsNullOrEmpty(Input.Filter)) Input.Filter = Input.Filter.Trim().ToLower();

            return _appLocalizationRepository
             .GetAllIncluding(x => x.Translations)
              .AsNoTracking()
              .WhereIf(!string.IsNullOrWhiteSpace(Input.Filter), e => e.MasterKey.ToLower().Contains(Input.Filter) || e.MasterValue.ToLower().Contains(Input.Filter) || e.Translations.Any(x => x.Value.ToLower().Contains(Input.Filter)))
              .WhereIf(Input.EditionId.HasValue, e => e.TerminologieEditions.Any(x=>x.EditionId== Input.EditionId.Value))
              .WhereIf(!string.IsNullOrWhiteSpace(Input.Page), e => e.TerminologiePages.Any(x => x.PageUrl.ToLower().Contains(Input.Page.ToLower())))
              .OrderBy(!string.IsNullOrEmpty(Input.Sorting) ? Input.Sorting : "id asc");
        }

        private async Task CreateKeyLog(TerminologieMonitorInput input)
        {
            AppLocalization Terminologie = new AppLocalization();
            Terminologie.MasterKey = FirstCharToUpper(input.Key);
            Terminologie.MasterValue = Terminologie.MasterKey.ToSentenceCase();
            if (AbpSession.TenantId.HasValue) Terminologie.TerminologieEditions.Add(new TerminologieEdition { EditionId = GetCurrentTenant().EditionId.Value });
            Terminologie.TerminologiePages.Add(new TerminologiePage { PageUrl = input.PageUrl });

            await _appLocalizationRepository.InsertAsync(Terminologie);
        }

        private async Task UpdateKeyLog(TerminologieMonitorInput input, AppLocalization terminologie)
        {
            if (AbpSession.TenantId.HasValue)
            {
                int? EditionId = GetCurrentTenant().EditionId;
                if (!await _terminologieEditionRepository.GetAll().AnyAsync(x => x.EditionId == EditionId && x.TerminologieId == terminologie.Id))
                {
                    await _terminologieEditionRepository.InsertAsync(new TerminologieEdition { EditionId = EditionId.Value, TerminologieId = terminologie.Id });
                }
            }

            if (!await _terminologiePageRepository.GetAll().AnyAsync(x => x.PageUrl == input.PageUrl && x.TerminologieId == terminologie.Id))
            {
                await _terminologiePageRepository.InsertAsync(new TerminologiePage { PageUrl = input.PageUrl, TerminologieId = terminologie.Id });
            }
        }

        public string FirstCharToUpper(string input) => input.First().ToString().ToUpper() + input.Substring(1);

        #endregion
    }

}
