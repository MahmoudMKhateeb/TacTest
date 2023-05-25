using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Reports.ReportDefinitions
{
    public class CreateOrEditReportDefinitionDto : EntityDto<int?>
    {
        [StringLength(120,MinimumLength = 3)]
        public string DisplayName { get; set; }

        public ReportType Type { get; set; }

        public string ReportTemplateUrl { get; set; }

        public List<int> GrantedEditionIds { get; set; }

        public List<int> ExcludedTenantIds { get; set; }
    } 
}
