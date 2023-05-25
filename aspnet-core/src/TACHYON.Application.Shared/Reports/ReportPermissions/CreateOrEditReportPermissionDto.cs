using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Reports.ReportPermissions
{
    public class CreateOrEditReportPermissionDto
    {
        public int? TenantId { get; set; }
        public int? EditionId { get; set; }
    }
}
