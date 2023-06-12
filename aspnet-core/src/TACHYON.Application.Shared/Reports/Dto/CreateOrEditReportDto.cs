using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using TACHYON.Reports.ReportParameters;

namespace TACHYON.Reports.Dto
{
    public class CreateOrEditReportDto : EntityDto<Guid?>, IShouldNormalize
    {
        public string DisplayName { get; set; }
        
        public int ReportDefinitionId { get; set; }

        public ReportFormat Format { get; set; }
        
        public List<int> GrantedRoles { get; set; }
        public List<long> ExcludedUsers { get; set; }


        public List<ReportParameterDto> Parameters { get; set; }
        public void Normalize()
        {
            Parameters.ForEach(x=> x.Name = x.Name.Trim());
        }
    }
}