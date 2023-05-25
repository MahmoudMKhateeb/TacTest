using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Reports.ReportPermissions;

namespace TACHYON.Reports.ReportDefinitions
{
    [Table("ReportDefinitions")]
    public class ReportDefinition : FullAuditedEntity
    {
        public string DisplayName { get; set; }

        public ReportType Type { get; set; }

        [Required]
        public Guid ReportTemplateId { get; set; }

        [ForeignKey(nameof(ReportTemplateId))]
        public Report ReportTemplate { get; set; }

        public ICollection<ReportPermission> ReportPermissions { get; set; }
    }
}
