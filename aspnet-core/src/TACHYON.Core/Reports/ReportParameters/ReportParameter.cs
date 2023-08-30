using Abp.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Reports.ReportParameterDefinitions;

namespace TACHYON.Reports.ReportParameters
{
    [Table("ReportParameters")]
    public class ReportParameter : Entity<Guid>, ISoftDelete
    {
        public Guid ReportParameterDefinitionId { get; set; }

        [ForeignKey(nameof(ReportParameterDefinitionId))]
        public ReportParameterDefinition ParameterDefinition { get; set; }

        public string Value { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}