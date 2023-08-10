using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Reports.Dto
{
    public class ReportListItemDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }

        public string FormatTitle { get; set; }
        public ReportFormat Format { get; set; }

        public string DefinitionName { get; set; }

        public Guid? GeneratedFileId { get; set; }

        public string GrantedRoles { get; set; }

        public string ExcludedUsers { get; set; }

        public DateTime CreationTime { get; set; }
    }
}