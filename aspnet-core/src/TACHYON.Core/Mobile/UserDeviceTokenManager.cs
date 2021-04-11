using Abp.Domain.Repositories;
using System.Threading.Tasks;
using TACHYON.Mobile.Dtos;

namespace TACHYON.Mobile
{

    public class UserDeviceTokenManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<UserDeviceToken> _userDeviceTokenRepository;
        public UserDeviceTokenManager(IRepository<UserDeviceToken> userDeviceTokenRepository)
        {
            _userDeviceTokenRepository = userDeviceTokenRepository;
        }
        public async Task CreateOrEdit(UserDeviceTokenDto Input)
        {
            var token = await _userDeviceTokenRepository.FirstOrDefaultAsync(x=> x.UserId==Input.UserId && x.DeviceId== Input.DeviceId);
            if (token != null)
                ObjectMapper.Map(Input, token);
            else
            {
                token = ObjectMapper.Map<UserDeviceToken>(Input);
                await _userDeviceTokenRepository.InsertAsync(token);
            }
        }
   
    }
}
