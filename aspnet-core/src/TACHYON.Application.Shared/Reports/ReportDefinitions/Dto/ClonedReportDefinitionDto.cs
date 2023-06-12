using System.Collections.Generic;

namespace TACHYON.Reports.ReportDefinitions.Dto
{
    public class ClonedReportDefinitionDto
    {
        public string DisplayName { get; set; }

        public ReportType Type { get; set; }
        
        public string ReportTemplateUrl { get; set; }

        public List<int> GrantedEditionIds { get; set; }

        public List<int> ExcludedTenantIds { get; set; }

        public List<string> ParameterDefinitions { get; set; }
    }
}