using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System;
using TACHYON.Web.Reports;
using TACHYON.Reports;

namespace TACHYON.Web.Services.StorageServices
{
    public static class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            [ReportsNames.TripDetailsReport] = () => new TripDetailsReport(),
        };
    }
}
