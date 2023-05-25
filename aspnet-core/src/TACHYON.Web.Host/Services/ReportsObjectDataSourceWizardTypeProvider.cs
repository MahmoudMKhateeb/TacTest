using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.DataAccess.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TACHYON.Reports.ReportDataSources;

namespace TACHYON.Web.Services
{
    public class ReportsObjectDataSourceWizardTypeProvider : IObjectDataSourceWizardTypeProvider
    {
        public IEnumerable<Type> GetAvailableTypes(string context)
        {
            
            return new[] { typeof(TripDetailsReportDataSourceAppService) };
        }

    }
}
