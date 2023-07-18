using Abp;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.UI;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Users;
using TACHYON.Reports.Jobs;
using TACHYON.Reports.ReportDefinitions;
using TACHYON.Reports.ReportParameterDefinitions;
using TACHYON.Reports.ReportParameters;
using TACHYON.Reports.ReportPermissions;
using TACHYON.Reports.ReportTemplates;
using TACHYON.Storage;

namespace TACHYON.Reports
{
    public class ReportManager : TACHYONDomainServiceBase, IReportManager
    {
        private readonly IRepository<Report, Guid> _reportRepository;
        private readonly IRepository<ReportParameterDefinition,Guid> _parameterDefinitionRepository;
        private readonly RoleManager _roleManager;
        private readonly IReportParameterDefinitionProvider _definitionProvider;
        private readonly IRepository<BinaryObject,Guid> _binaryObjectRepository;
        private readonly IBackgroundJobManager _jobManager;
        private readonly UserManager _userManager;
        private readonly IRepository<ReportPermission, Guid> _reportPermissionRepository;
        private readonly IReportDefinitionManager _definitionManager;

        public ReportManager(
            IRepository<Report, Guid> reportRepository,
            IRepository<ReportParameterDefinition, Guid> parameterDefinitionRepository,
            RoleManager roleManager,
            IReportParameterDefinitionProvider definitionProvider, 
            IRepository<BinaryObject, Guid> binaryObjectRepository,
            IBackgroundJobManager jobManager,
            UserManager userManager,
            IRepository<ReportPermission, Guid> reportPermissionRepository,
            IReportDefinitionManager definitionManager)
        {
            _reportRepository = reportRepository;
            _parameterDefinitionRepository = parameterDefinitionRepository;
            _roleManager = roleManager;
            _definitionProvider = definitionProvider;
            _binaryObjectRepository = binaryObjectRepository;
            _jobManager = jobManager;
            _userManager = userManager;
            _reportPermissionRepository = reportPermissionRepository;
            _definitionManager = definitionManager;
        }


        public IQueryable<Report> GetAll(long userId)
        {
            return from report in _reportRepository.GetAll() where report.IsPublished
                from roleId in _userManager.Users.Where(x => x.Id == userId)
                    .SelectMany(x => x.Roles).Select(x => x.RoleId)
                where _reportPermissionRepository.GetAll().Where(x => x.ReportId == report.Id)
                    .Any(x => x.RoleId == roleId && x.IsGranted)
                where _reportPermissionRepository.GetAll().Where(x => x.ReportId == report.Id)
                    .All(x => x.UserId != userId || x.IsGranted)
                select report;
        }

        public async Task<Guid> CreateReport(Report report, List<int> grantedRoles, List<long> excludedUsers, List<ReportParameterDto> parameters)
        {
            var roles = await _roleManager.Roles.Select(x => x.Id).ToListAsync();
            bool anyInvalidRole = grantedRoles.Except(roles).Any();
            if (anyInvalidRole) throw new UserFriendlyException(L("YouMustSelectAValidRole"));

            var reportPermissionByRole = grantedRoles
                .Select(roleId => new ReportPermission { RoleId = roleId, IsGranted = true }).ToList();
            var reportPermissionByUser = excludedUsers?
                .Select(userId => new ReportPermission { UserId = userId, IsGranted = false }).ToList();
            report.ReportPermissions = reportPermissionByUser != null ? reportPermissionByRole.Union(reportPermissionByUser).ToList() : reportPermissionByRole;
            var selectedParameterNames = parameters.Select(x => x.Name).ToArray();
            var reportDefinitionType = await _definitionManager.GetReportDefinitionType(report.ReportDefinitionId);
            if (_definitionProvider.AnyParameterNotDefined(reportDefinitionType,selectedParameterNames))
                throw new UserFriendlyException(L("NotValidParameterIsSelected"));

            report.Parameters = (from selectedParameter in parameters
                from paraDefinition in _parameterDefinitionRepository.GetAll()
                where paraDefinition.Name == selectedParameter.Name && paraDefinition.ReportDefinitionId == report.ReportDefinitionId
                select new ReportParameter
                {
                    ReportParameterDefinitionId = paraDefinition.Id, Value = selectedParameter.Value
                }).ToList();
            
            return await _reportRepository.InsertAndGetIdAsync(report);
        }

        public async Task GenerateReportFile(Guid reportId)
        {
            var report = await _reportRepository.GetAll().Where(x=> x.Id == reportId).Select(x=> new
            {
                ReportData = x.ReportDefinition.ReportTemplate.Data,
                x.DisplayName,
                x.Format,x.TenantId
            }).SingleAsync();
            var generatedReport = new XtraReport();
            using var reportStream = new MemoryStream(report.ReportData);
            generatedReport.LoadLayoutFromXml(reportStream);
            byte[] generatedFileBytes = report.Format switch
            {
                ReportFormat.Pdf => await ExportToPdf(generatedReport,report.DisplayName),
                ReportFormat.Excel => await ExportToExcel(generatedReport,report.DisplayName),
                ReportFormat.Word => await ExportToWord(generatedReport,report.DisplayName),
                ReportFormat.Html => await ExportToHtml(generatedReport,report.DisplayName),
                ReportFormat.Image => await ExportToImage(generatedReport),
                _ => throw new ArgumentOutOfRangeException()
            };
            var binaryObject = new BinaryObject(report.TenantId, generatedFileBytes);
            var binaryObjectId = await _binaryObjectRepository.InsertAndGetIdAsync(binaryObject);
            _reportRepository.Update(reportId, x => x.GeneratedFileId = binaryObjectId);
        }

        public async Task Publish(Guid reportId)
        {
            bool isExists = await _reportRepository.GetAll().AnyAsync(x=> x.Id == reportId);
            if (!isExists) throw new UserFriendlyException(L("ReportNotFound"));

            await _jobManager.EnqueueAsync<GenerateReportFileJob, Guid>(reportId);
            _reportRepository.Update(reportId, x => x.IsPublished = true);
        }

        public async Task Delete(Guid reportId)
        {
            bool isExists = await _reportRepository.GetAll().AnyAsync(x => x.Id == reportId);

            if (!isExists)
            {
                throw new UserFriendlyException(L("ReportNotFound"));
            }

            await _reportRepository.DeleteAsync(reportId);
        }

        #region Helpers

        private async Task<byte[]> ExportToPdf(XtraReport generatedReport,string fileTitle)
        {
            var pdfExportOptions = generatedReport.ExportOptions.Pdf;
            pdfExportOptions.DocumentOptions.Application = "TACHYON";
            pdfExportOptions.DocumentOptions.Author = "TACHYON Company";
            pdfExportOptions.DocumentOptions.Title = fileTitle ;
            using var exportedFileStream = new MemoryStream();
            await generatedReport.ExportToPdfAsync(exportedFileStream, pdfExportOptions);
            return exportedFileStream.ToArray();
        }        
        private async Task<byte[]> ExportToExcel(XtraReport generatedReport,string fileTitle)
        {
            var excelExportOptions = generatedReport.ExportOptions.Xlsx;
            excelExportOptions.DocumentOptions.Application = "TACHYON";
            excelExportOptions.DocumentOptions.Author = "TACHYON Company";
            excelExportOptions.DocumentOptions.Title = fileTitle ;
            using var exportedFileStream = new MemoryStream();
            await generatedReport.ExportToXlsxAsync(exportedFileStream, excelExportOptions);
            return exportedFileStream.ToArray();
        }        
        private async Task<byte[]> ExportToWord(XtraReport generatedReport,string fileTitle)
        {
            var docxOptions = generatedReport.ExportOptions.Docx;
            docxOptions.DocumentOptions.Author = "TACHYON Company";
            docxOptions.DocumentOptions.Title = fileTitle ;
            using var exportedFileStream = new MemoryStream();
            await generatedReport.ExportToDocxAsync(exportedFileStream, docxOptions);
            return exportedFileStream.ToArray();
        }        
        private async Task<byte[]> ExportToHtml(XtraReport generatedReport,string fileTitle)
        {
            var htmlOptions = generatedReport.ExportOptions.Html;
            htmlOptions.Title = fileTitle;
            using var exportedFileStream = new MemoryStream();
            await generatedReport.ExportToHtmlAsync(exportedFileStream, htmlOptions);
            return exportedFileStream.ToArray();
        }        
        private async Task<byte[]> ExportToImage(XtraReport generatedReport)
        {
            using var exportedFileStream = new MemoryStream();
            await generatedReport.ExportToHtmlAsync(exportedFileStream);
            return exportedFileStream.ToArray();
        }

        #endregion

    }
}