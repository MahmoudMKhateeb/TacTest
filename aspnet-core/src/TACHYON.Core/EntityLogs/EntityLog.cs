using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.EntityLogs
{
    [Table("EntityHistoryLog")]
    public class EntityLog : FullAuditedEntity<Guid>, IMayHaveTenant
    {
        public string CoreId { get; set; }

        public string Core { get; set; }
        public EntityLogTransaction LogTransaction { get; set; }

        // Any Data Stored As Json
        public string Data { get; set; }

        public DateTime ChangeTime { get; set; }

        public int? TenantId { get; set; }
    }
}