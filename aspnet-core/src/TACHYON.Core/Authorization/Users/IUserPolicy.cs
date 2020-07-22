using Abp.Domain.Policies;
using System.Threading.Tasks;

namespace TACHYON.Authorization.Users
{
    public interface IUserPolicy : IPolicy
    {
        Task CheckMaxUserCountAsync(int tenantId);
    }
}