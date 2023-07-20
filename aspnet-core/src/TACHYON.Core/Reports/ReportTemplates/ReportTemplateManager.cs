using Abp.Domain.Repositories;
using Abp.UI;
using DevExpress.XtraReports.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Reports.ReportDefinitions;
using TACHYON.Reports.Templates;
using TACHYON.Web.Reports;

namespace TACHYON.Reports.ReportTemplates
{
    public class ReportTemplateManager : TACHYONDomainServiceBase, IReportTemplateManager
    {
        private readonly IRepository<ReportTemplate, Guid> _reportTemplateRepository;

        public ReportTemplateManager(IRepository<ReportTemplate, Guid> reportTemplateRepository)
        {
            _reportTemplateRepository = reportTemplateRepository;
        }


        public async Task<string> CreateReportTemplate(ReportType type, string reportDefinitionName)
        {
            if (string.IsNullOrEmpty(reportDefinitionName)) 
                throw new UserFriendlyException("ReportTypeNameIsRequired");
            
            bool isUrlDuplicated = await IsUrlDuplicated(reportDefinitionName);
            if (isUrlDuplicated) 
                throw new UserFriendlyException(L("ReportTemplateUrlAlreadyUsedBefore"));
            
            XtraReport report = type switch
            {
                ReportType.TripDetailsReport => new TripDetailsReport(),
                ReportType.PodPerformanceReport => new PodPerformanceReport(),
                ReportType.FinancialReport => new FinancialReport(),
                // add your other types here if exists
                _ => throw new UserFriendlyException(L("NotSupportedReportType"))
            };
            string generatedReportUrl = GenerateReportUrl(reportDefinitionName);
            using var reportStream = new MemoryStream();
            report.SaveLayoutToXml(reportStream);
            var createdReportTemplate = new ReportTemplate
            {
                Name = reportDefinitionName, Url = generatedReportUrl, Data = reportStream.ToArray()
            };

            await _reportTemplateRepository.InsertAsync(createdReportTemplate);
            return generatedReportUrl;
        }

        public async Task<Guid> GetReportTemplateIdByUrl(string url)
        {
           var reportTemplate = await _reportTemplateRepository.GetAll().Where(x => x.Url == url)
                .Select(x => new {x.Name,x.Id}).FirstOrDefaultAsync();
            if (reportTemplate is null)
                throw new UserFriendlyException(L("ReportTemplateNotFound"));
            
            return reportTemplate.Id;
        }

        public async Task<ReportTemplate> GetReportTemplateByUrl(string url)
        {
            var reportTemplate = await _reportTemplateRepository.GetAll().Where(x => x.Url == url)
               .SingleAsync();
            
            return reportTemplate;
        }
        
        public async Task UpdateReportTemplate(string url, XtraReport report)
        {
            var reportTemplate = await GetReportTemplateByUrl(url);
            using var memoryStream = new MemoryStream();
            report.SaveLayoutToXml(memoryStream);
            reportTemplate.Data = memoryStream.ToArray();
        }

        public async Task<bool> IsUrlDuplicated(string reportDefinitionName)
        {
            string generatedUrl = GenerateReportUrl(reportDefinitionName);
            return await _reportTemplateRepository.GetAll().AnyAsync(x => x.Url == generatedUrl);
        }

        #region Helpers

        /// <summary>
        /// this method used to generate an unique url for report template
        /// </summary>
        /// <param name="reportDefinitionName"></param>
        /// <returns></returns>
        private static string GenerateReportUrl(string reportDefinitionName)
        {
            string modifiedName = reportDefinitionName.Trim().Replace(" ", "_");
            // Remove dots and special characters
            modifiedName = RemoveSpecialCharacters(modifiedName);
            return modifiedName;
        }

        /// <summary>
        /// This method used to remove special character from string except underscore
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string RemoveSpecialCharacters(string text)
        {
            StringBuilder sb = new();

            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(c);
                    continue;
                }
                sb.Append("_");

            }

            return sb.ToString();
        }

        #endregion
    }
}