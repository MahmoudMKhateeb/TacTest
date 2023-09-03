using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Reports.ReportDefinitions;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    [Table("ReportParameterDefinitions")]
    public class ReportParameterDefinition : Entity<Guid>, ISoftDelete
    {
        public int ReportDefinitionId { get; set; }

        [ForeignKey(nameof(ReportDefinitionId))]
        public ReportDefinition ReportDefinition { get; set; }
        // this name should be registered in the ReportParameterDefinitionProvider
        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}