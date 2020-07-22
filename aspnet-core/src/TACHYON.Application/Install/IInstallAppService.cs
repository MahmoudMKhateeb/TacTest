using Abp.Application.Services;
using System.Threading.Tasks;
using TACHYON.Install.Dto;

namespace TACHYON.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}