using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Dashboards
{
    public class DashboardDomainService: TACHYONDomainServiceBase
    {
         public DashboardDomainService() { }
        public List<DateTime> GetYearMonthsEndWithCurrent()
        {
            DateTime endDate = new DateTime(Clock.Now.Year, Clock.Now.Month, 1);
            DateTime begDate = Clock.Now.Month == 12 ? new DateTime(Clock.Now.Year, Clock.Now.AddMonths(-11).Month, 1) : new DateTime(Clock.Now.AddYears(-1).Year, Clock.Now.AddMonths(-11).Month, 1);

            var monthsList = new List<DateTime>();
            foreach (DateTime date in MonthsInRange(begDate, endDate))
            {
                monthsList.Add(date);
            }

            return monthsList;
        }

        public IEnumerable<DateTime> MonthsInRange(DateTime start, DateTime end)
        {
            for (DateTime date = start; date <= end; date = date.AddMonths(1))
            {
                yield return date;
            }
        }
    }
}
