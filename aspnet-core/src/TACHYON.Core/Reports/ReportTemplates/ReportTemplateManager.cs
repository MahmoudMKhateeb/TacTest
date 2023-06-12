using Abp.Domain.Repositories;
using Abp.UI;
using DevExpress.XtraReports.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            
            XtraReport report = type switch
            {
                ReportType.TripDetailsReport => new TripDetailsReport(),
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

        #region Helpers

        private string GenerateReportUrl(string reportDefinitionName)
        {
            string modifiedName = reportDefinitionName.Replace(" ", "_");
            // Remove dots and special characters
            modifiedName = RemoveSpecialCharacters(modifiedName);
            return modifiedName;
        }

        private string RemoveSpecialCharacters(string input)
        {
            StringBuilder sb = new();

            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append("_");
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}