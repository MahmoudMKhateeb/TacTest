using AutoMapper;
using TACHYON.Mobile;
using TACHYON.Mobile.Dtos;

namespace TACHYON.AutoMapper.Mobile
{
    public class UserDeviceTokenProfile : Profile
    {
        public UserDeviceTokenProfile()
        {
            CreateMap<UserDeviceTokenDto, UserDeviceToken>();
        }
    }
}