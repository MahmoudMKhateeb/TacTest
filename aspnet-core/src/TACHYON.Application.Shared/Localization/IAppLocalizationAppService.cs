using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using TACHYON.Dto;
using TACHYON.Localization.Dto;

namespace TACHYON.Localization
{
    public interface IAppLocalizationAppService: IApplicationService
    {
        Task<PagedResultDto<AppLocalizationListDto>> GetAll(AppLocalizationFilterInput Input);
        Task<AppLocalizationForViewDto> GetForView(EntityDto input);

        Task<CreateOrEditAppLocalizationDto> GetForEdit(EntityDto input);
        Task CreateOrEdit(CreateOrEditAppLocalizationDto input);
        FileDto Exports(AppLocalizationFilterInput Input);
        Task Delete(EntityDto input);
        Task Restore();
        Task Generate();
        Task CreateOrUpdateKeyLog(TerminologieMonitorInput input);

    }
}
