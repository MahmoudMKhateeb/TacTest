using Abp.AspNetZeroCore.Net;
using Abp.Dependency;
using Microsoft.Reporting.NETCore;
using System.Collections;
using System.IO;
using System.Reflection;
using TACHYON.Dto;
using TACHYON.Storage;

namespace TACHYON.DataExporting
{
    public class PdfExporterBase : TACHYONServiceBase, ITransientDependency
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;

        public PdfExporterBase(ITempFileCacheManager tempFileCacheManager)
        {
            _tempFileCacheManager = tempFileCacheManager;
        }

        private void Save(byte[] bytes, FileDto file)
        {
            _tempFileCacheManager.SetFile(file.FileToken, bytes);
        }

        public FileDto CreateRdlcPdfPackageFromList(string fileName,
            string reportPath,
            ArrayList dsNameArray,
            ArrayList DTArray)
        {
            byte[] pdf = GetRdlcPdfPackageAsBinaryData(reportPath, dsNameArray, DTArray);
            var file = new FileDto(fileName, MimeTypeNames.ApplicationPdf);
            Save(pdf, file);
            return file;
        }

        public byte[] GetRdlcPdfPackageAsBinaryData(string reportPath,
            ArrayList dsNameArray,
            ArrayList DTArray)
        {
            reportPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + reportPath;
            LocalReport report = new LocalReport();
            report.ReportPath = reportPath;

            for (int i = 0; i < DTArray.Count; i++)
                report.DataSources.Add(new ReportDataSource((string)dsNameArray[i], (IEnumerable)DTArray[i]));

            return report.Render("PDF");
        }
    }
}