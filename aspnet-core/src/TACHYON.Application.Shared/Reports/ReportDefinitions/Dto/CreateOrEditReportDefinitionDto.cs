using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Reports.ReportDefinitions.Dto
{
    public class CreateOrEditReportDefinitionDto : EntityDto<int?>, ICustomValidate
    {
        [StringLength(120,MinimumLength = 3)]
        public string DisplayName { get; set; }

        public ReportType Type { get; set; }

        [Required]
        [StringLength(int.MaxValue,MinimumLength = 1)]
        public string ReportTemplateUrl { get; set; }

        public List<int> GrantedEditionIds { get; set; }

        public List<int> ExcludedTenantIds { get; set; }

        public List<string> ParameterDefinitions { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (GrantedEditionIds.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("You must select at lease one edition"));
            if (ParameterDefinitions.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("You must select at lease one filter"));
        }
    } 
}
