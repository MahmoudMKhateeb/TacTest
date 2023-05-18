using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Reports
{
    [Table("Reports")]
    public class Report : FullAuditedEntity<Guid>
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }
}
