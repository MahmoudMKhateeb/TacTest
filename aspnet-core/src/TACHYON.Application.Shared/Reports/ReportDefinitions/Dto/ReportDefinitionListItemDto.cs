using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Reports.ReportDefinitions.Dto
{
    public class ReportDefinitionListItemDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string ReportType { get; set; }

        public string GrantedEditions { get; set; }
        
        public string ExcludedCompanies { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsActive { get; set; }
    }
}