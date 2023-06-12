using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Localization.Sources;
using Abp.Threading;
using Abp.Timing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Reports.ReportTemplates;
using TACHYON.Web.Services.DataSourceServices;

namespace TACHYON.Web.Services.StorageServices
{
    public class ReportStorageWebExtensionService : ReportStorageWebExtension, ITransientDependency
    {
        private const string ReportFileExtension = ".repx";
        private readonly IRepository<ReportTemplate, Guid> _reportRepository;
        private readonly ILocalizationSource _localizationSource;

        public ReportStorageWebExtensionService(
            IRepository<ReportTemplate, Guid> reportRepository,
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
            return _reportRepository.GetAll().Any(x=> x.Url == url);
        }

        public override byte[] GetData(string url)
        {
            return AsyncHelper.RunSync(() => GetDataAsync(url));
        }

        public override async Task<byte[]> GetDataAsync(string url)
        {
            bool isReportStoredInDb = await _reportRepository.GetAll().AnyAsync(r => r.Url == url);
            if (!isReportStoredInDb) 
                throw new EntityNotFoundException(_localizationSource.GetString("ReportNotFound"));
            
            var reportData = await _reportRepository.GetAll().Where(x => x.Url == url).Select(x => x.Data).FirstOrDefaultAsync();
            if (reportData is null ) 
                throw new EntityNotFoundException(_localizationSource.GetString("EmptyReportFile"));
            
            using var reportStream = new MemoryStream(reportData);
            var report = new XtraReport();
            report.LoadLayoutFromXml(reportStream);
            report.ReplaceService(new JsonSourceCustomizationService());
            return GetReportData(report);
        }

        public override Dictionary<string, string> GetUrls()
        {
            return AsyncHelper.RunSync(GetUrlsAsync);
        }

        public override async Task<Dictionary<string, string>> GetUrlsAsync()
        {
            var reports = await _reportRepository.GetAll().Select(x => x.Url).ToListAsync();
            var urls = ReportsFactory.Reports.Select(x => x.Key)
                .ToList();

            return urls.Union(reports).ToDictionary(x => x);
        }

        public override async Task SetDataAsync(XtraReport report, string url)
        {
            var reportData = GetReportData(report);
            var dbReport = await _reportRepository.FirstOrDefaultAsync(r => r.Url == url);

            if (dbReport != null)
            {
                dbReport.Data = reportData;
                dbReport.Name = report.Name;
                dbReport.LastModificationTime = Clock.Now;
            }

            dbReport ??= new ReportTemplate { Url = Path.GetFileNameWithoutExtension(url), Data = reportData, Name = report.Name, CreationTime = Clock.Now };


           await _reportRepository.InsertOrUpdateAsync(dbReport);
        }

        public override void SetData(XtraReport report, string url)
        {
            AsyncHelper.RunSync(() => SetDataAsync(report,url));
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            var url = defaultUrl.EndsWith(ReportFileExtension) ? defaultUrl : defaultUrl + ReportFileExtension;
            SetData(report, url);
            return url;
        }

        public override async Task<string> SetNewDataAsync(XtraReport report, string defaultUrl)
        {
            var url = defaultUrl.EndsWith(ReportFileExtension) ? defaultUrl : defaultUrl + ReportFileExtension;
            await SetDataAsync(report, url);
            return url;
        }

        private static byte[] GetReportData(XtraReport report)
        {
            using var stream = new MemoryStream();
            report.SaveLayoutToXml(stream);
            return stream.ToArray();
        }
    }
}