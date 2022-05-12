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

        [ForeignKey(nameof(UserId))] public User User { get; set; }
        public string OTP { get; set; } = (new Random().Next(1000, 9999)).ToString();

        public DateTime CreationTime { get; set; }

        //changed from 3 minutes to 1 minute based on TAC-2122
        public DateTime ExpireTime { get; set; } = Clock.Now.AddMinutes(1);

        public UserOTP(long UserId)
        {
            this.UserId = UserId;
        }
    }
}