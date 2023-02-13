using TACHYON.Dashboards.Host.Dto;

namespace TACHYON.Dashboards.Broker
{
    public class NextDueDateDto
    {
        public string CompanyName { get; set; }

        public int PeriodAmount { get; set; }

        public FilterDatePeriod PeriodType { get; set; } 
    }
}