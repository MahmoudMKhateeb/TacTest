using Abp.Application.Services.Dto;
using TACHYON.Actors;

namespace TACHYON.Sessions.Dto
{
    public class UserLoginInfoDto : EntityDto<long> , IMayHaveShipperActor
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string ProfilePictureId { get; set; }
        public int? ShipperActorId { get; set; }
    }
}