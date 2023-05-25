using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Reports.ReportDefinitions;

namespace TACHYON.Reports.ReportPermissions
{
    [Table("ReportPermissions")]
    public class ReportPermission : FullAuditedEntity<Guid>
    {
        public int? TenantId { get; set; }
        public int? EditionId { get; set; }

        [Required]
        public int ReportDefinitionId { get; set; }

        [ForeignKey(nameof(ReportDefinitionId))]
        public ReportDefinition ReportDefinition { get; set; }

        public bool IsGranted { get; set; }
    }
}
