using Abp.Application.Services;
using System.Threading.Tasks;
using TACHYON.Sessions.Dto;

namespace TACHYON.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();

        Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken();
    }
}