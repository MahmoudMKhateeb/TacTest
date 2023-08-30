using Abp.Application.Editions;
using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.Reports.ReportDefinitions;

namespace TACHYON.Reports.ReportDefinitionPermissions
{
    [Table("ReportDefinitionPermissions")]
    public class ReportDefinitionPermission : Entity<Guid>, ISoftDelete, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public int? EditionId { get; set; }

        [ForeignKey(nameof(EditionId))]
        public Edition Edition { get; set; }
        
        [Required]
        public int ReportDefinitionId { get; set; }

        [ForeignKey(nameof(ReportDefinitionId))]
        public ReportDefinition ReportDefinition { get; set; }

        public bool IsGranted { get; set; }
        public bool IsDeleted { get; set; }
    }
}
