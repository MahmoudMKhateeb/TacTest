using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.IO.Extensions;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.UI;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TACHYON.Reports;

namespace TACHYON.Web.Services
{
    public class ReportStorageWebExtensionService : ReportStorageWebExtension, ITransientDependency
    {
        public const string ReportFileExtension = ".repx";
        private readonly IRepository<Report, Guid> _reportRepository;
        private readonly ILocalizationSource _localizationSource;

        public ReportStorageWebExtensionService(
            IRepository<Report, Guid> reportRepository,
            ILocalizationManager localizationManager)
        {
            _reportRepository = reportRepository;
            _localizationSource = localizationManager.GetSource(TACHYONConsts.LocalizationSourceName);
        }

        public override bool CanSetData(string url)
        {
            return true;
        }

        public override bool IsValidUrl(string url)
        {
            return true;
        }

        public override byte[] GetData(string url)
        {
            bool isReportStoredInDb = _reportRepository.GetAll().Any(r => r.Url == url);

            if (isReportStoredInDb)
                return _reportRepository.GetAll().Where(x => x.Url == url).Select(x => x.Data).FirstOrDefault();

            bool isReportStoredInFactory = ReportsFactory.Reports.Any(x=> x.Key == url);
            if (!isReportStoredInFactory) throw new EntityNotFoundException(_localizationSource.GetString("ReportNotFound"));

            using var reportStream = new MemoryStream();
            ReportsFactory.Reports[url]().SaveLayoutToXml(reportStream);
            return reportStream.ToArray();
        }

        public override Dictionary<string, string> GetUrls()
        {
            var reports = _reportRepository.GetAllList().Select(x => x.Url).ToList();
            var urls = ReportsFactory.Reports.Select(x => x.Key)
                .ToList();

            return urls.Union(reports).ToDictionary(x => x);
        }

        public override void SetData(XtraReport report, string url)
        {
            var reportData = GetReportData(report);
            var dbReport = _reportRepository.FirstOrDefault(r => r.Url == url);

            if (dbReport != null)
            {
                dbReport.Data = reportData;
                dbReport.Name = report.Name;
                dbReport.LastModificationTime = Clock.Now;
            }

            dbReport ??= new Report { Url = url, Data = reportData, Name = report.Name, CreationTime = Clock.Now };


            _reportRepository.InsertOrUpdate(dbReport);
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            var url = defaultUrl.EndsWith(ReportFileExtension) ? defaultUrl : defaultUrl + ReportFileExtension;
            SetData(report, url);
            return url;
        }

        private static byte[] GetReportData(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.SaveLayoutToXml(stream);
            return stream.ToArray();
        }

        private static Stream GetResourceStream(string resourceName)
        {
            return typeof(ReportStorageWebExtensionService).GetAssembly().GetManifestResourceStream(resourceName);
        }
    }
}