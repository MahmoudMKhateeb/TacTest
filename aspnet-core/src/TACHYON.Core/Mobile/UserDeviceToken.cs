using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Authorization.Users;

namespace TACHYON.Mobile
{
    [Table("UserDeviceTokens")]
    public class UserDeviceToken : Entity, IHasCreationTime, IHasModificationTime
    {
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))] public User User { get; set; }
        public string DeviceId { get; set; }

        public string Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}