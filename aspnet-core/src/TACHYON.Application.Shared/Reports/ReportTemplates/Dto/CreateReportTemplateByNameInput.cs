namespace TACHYON.Reports.ReportTemplates.Dto
{
    public class CreateReportTemplateByNameInput
    {
        public ReportType ReportDefinitionType { get; set; }

        public string ReportDefinitionName { get; set; }
    }
}