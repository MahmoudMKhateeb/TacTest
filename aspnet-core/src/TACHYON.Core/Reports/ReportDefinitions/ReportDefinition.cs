using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Reports.ReportDefinitionPermissions;
using TACHYON.Reports.ReportParameterDefinitions;
using TACHYON.Reports.ReportPermissions;
using TACHYON.Reports.ReportTemplates;

namespace TACHYON.Reports.ReportDefinitions
{
    [Table("ReportDefinitions")]
    public class ReportDefinition : FullAuditedEntity, IPassivable
    {
        public string DisplayName { get; set; }

        public ReportType Type { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public Guid ReportTemplateId { get; set; }

        [ForeignKey(nameof(ReportTemplateId))]
        public ReportTemplate ReportTemplate { get; set; }

        public ICollection<ReportParameterDefinition> ParameterDefinitions { get; set; }
        public ICollection<ReportDefinitionPermission> DefinitionPermissions { get; set; }
    }
}
