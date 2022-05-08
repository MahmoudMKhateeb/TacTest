using Abp.Domain.Repositories;
using Abp.Extensions;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Mobile.Dtos;

namespace TACHYON.Mobile
{
    public class UserDeviceTokenManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<UserDeviceToken> _userDeviceTokenRepository;
        private readonly TachyonAppSession _mobileAppSession;

        public UserDeviceTokenManager(IRepository<UserDeviceToken> userDeviceTokenRepository,
            TachyonAppSession mobileAppSession)
        {
            _userDeviceTokenRepository = userDeviceTokenRepository;
            _mobileAppSession = mobileAppSession;
        }

        public async Task CreateOrEdit(UserDeviceTokenDto Input)
        {
            var token = await _userDeviceTokenRepository.FirstOrDefaultAsync(x =>
                x.UserId == Input.UserId && x.DeviceId == Input.DeviceId);
            if (token != null)
                ObjectMapper.Map(Input, token);
            else
            {
                token = ObjectMapper.Map<UserDeviceToken>(Input);
                await _userDeviceTokenRepository.InsertAsync(token);
            }
        }

        public async Task DeleteUserDeviceToken()
        {
            var deviceId = _mobileAppSession.DeviceId;
            var deviceToken = _mobileAppSession.DeviceToken;

            if (deviceId.IsNullOrEmpty() || deviceToken.IsNullOrEmpty())
                return;

            var token = await _userDeviceTokenRepository.FirstOrDefaultAsync(x =>
                x.DeviceId == deviceId && x.Token == deviceToken);
            if (token != null)
                await _userDeviceTokenRepository.DeleteAsync(token);
        }
    }
}