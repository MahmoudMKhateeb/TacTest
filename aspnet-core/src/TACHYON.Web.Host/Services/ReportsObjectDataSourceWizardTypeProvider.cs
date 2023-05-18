using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.DataAccess.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TACHYON.Web.Services
{
    public class ReportsObjectDataSourceWizardTypeProvider : IObjectDataSourceWizardTypeProvider
    {
        public IEnumerable<Type> GetAvailableTypes(string context)
        {
            return new[] { typeof(TripDetailsDataSource) };
        }

        [DisplayName("Trip Details Data Source")]
        public class TripDetailsDataSource
        {
            public List<string> GetData()
            {
                return new List<string>{"Mosa", "Mosa" , "Mosa" , "Mosa" , "Mosa" , "Mosa" };
            }
        }
    }
}
