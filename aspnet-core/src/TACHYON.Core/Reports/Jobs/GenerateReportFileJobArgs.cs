using System;

namespace TACHYON.Reports.Jobs
{
    public class GenerateReportFileJobArgs
    {
        public Guid ReportId { get; set; }

        public string DataSourceAuthKey { get; set; }
    }
}