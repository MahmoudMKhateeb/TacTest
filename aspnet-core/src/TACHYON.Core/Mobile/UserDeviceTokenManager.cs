using Abp.Domain.Repositories;
using System.Threading.Tasks;

namespace TACHYON.Mobile
{

    public class UserDeviceTokenManager: TACHYONDomainServiceBase
    {
        private readonly IRepository<UserDeviceToken> _userDeviceToken;
        public UserDeviceTokenManager(IRepository<UserDeviceToken> userDeviceToken)
        {
            _userDeviceToken = userDeviceToken;
        }

   
    }
}
