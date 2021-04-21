using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Authorization.Users;

namespace TACHYON.Mobile
{
    [Table("UserOTPs")]
    public class UserOTP : Entity, IHasCreationTime
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string OTP { get; set; } = (new Random().Next(100000, 999999)).ToString();
        public DateTime CreationTime { get; set; }

        public DateTime ExpireTime { get; set; } = Clock.Now.AddMinutes(3);

        public UserOTP(long UserId)
        {
            this.UserId = UserId;
        }

    }
}
