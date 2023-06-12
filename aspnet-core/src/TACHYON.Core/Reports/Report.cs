using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.Reports.ReportDefinitions;
using TACHYON.Reports.ReportParameters;
using TACHYON.Reports.ReportPermissions;

namespace TACHYON.Reports
{
    [Table("Reports")]
    public class Report : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public string DisplayName { get; set; }
        
        public ReportFormat Format { get; set; }
        public int ReportDefinitionId { get; set; }

        [ForeignKey(nameof(ReportDefinitionId))]
        public ReportDefinition ReportDefinition { get; set; }

        public int TenantId { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        
        public ICollection<ReportParameter> Parameters { get; set; }

        public ICollection<ReportPermission> ReportPermissions { get; set; }
    }
}