using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;

namespace TACHYON.StatusLogs
{
    public class StatusLog : Entity<long>, IMayHaveTenant, ICreationAudited, IHasCreationTime
    {
        public int? TenantId { get; set; }
        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public StatusLogChannel Channel { get; set; }
        public byte? FromStatus { get; set; }

        public byte ToStatus { get; set; }

        public string Description { get; set; }
    }
}