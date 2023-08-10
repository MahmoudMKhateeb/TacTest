using System.IO;
using System.Linq;
using TACHYON.EntityFrameworkCore;
using TACHYON.Reports;
using TACHYON.Reports.ReportTemplates;
using TACHYON.Web.Reports;

namespace TACHYON.Migrations.Seed.Host
{
    public class ReportTemplatesCreator
    {
        private readonly TACHYONDbContext _dbContext;

        public ReportTemplatesCreator(TACHYONDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void InitializeReportTemplates()
        {
            if (_dbContext.ReportTemplates.Any(x => x.Url == ReportsNames.TripDetailsReport))
            {
                return;
            }

            // initialize trip details report
            using var stream = new MemoryStream();
            new TripDetailsReport().SaveLayoutToXml(stream);
            var reportTemplate = new ReportTemplate()
            {
                Name = "Trip Details Report Template", Url = ReportsNames.TripDetailsReport, Data = stream.ToArray()
            };
            _dbContext.ReportTemplates.Add(reportTemplate);
            _dbContext.SaveChanges();
        }
    }
}