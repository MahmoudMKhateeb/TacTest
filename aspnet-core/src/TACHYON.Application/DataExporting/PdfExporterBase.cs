using Abp.AspNetZeroCore.Net;
using Abp.Dependency;
using AspNetCore.Reporting;
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

        public FileDto CreateRdlcPdfPackageFromList(string fileName, string reportPath, ArrayList dsNameArray, ArrayList DTArray)
        {
            reportPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + reportPath;

            LocalReport localReport = new LocalReport(reportPath);

            for (int i = 0; i < dsNameArray.Count; i++)
            {
                localReport.AddDataSource((string)dsNameArray[i], (IEnumerable)DTArray[i]);
            }


            byte[] bytes = localReport.Execute(RenderType.Pdf).MainStream;

            var file = new FileDto(fileName, MimeTypeNames.ApplicationPdf);

            Save(bytes, file);


            return file;
        }


    }
}
