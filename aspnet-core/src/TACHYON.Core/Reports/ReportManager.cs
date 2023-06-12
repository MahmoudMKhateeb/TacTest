using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Roles;
using TACHYON.Reports.ReportDefinitions;
using TACHYON.Reports.ReportParameterDefinitions;
using TACHYON.Reports.ReportParameters;
using TACHYON.Reports.ReportPermissions;

namespace TACHYON.Reports
{
    public class ReportManager : TACHYONDomainServiceBase, IReportManager
    {
        private readonly IRepository<Report, Guid> _reportRepository;
        private readonly IRepository<ReportDefinition> _reportDefinitionRepository;
        private readonly IRepository<ReportParameterDefinition,Guid> _reportParameterDefinitionRepository;
        private readonly RoleManager _roleManager;

        public ReportManager(
            IRepository<Report, Guid> reportRepository,
            IRepository<ReportDefinition> reportDefinitionRepository,
            IRepository<ReportParameterDefinition, Guid> reportParameterDefinitionRepository,
            RoleManager roleManager)
        {
            _reportRepository = reportRepository;
            _reportDefinitionRepository = reportDefinitionRepository;
            _reportParameterDefinitionRepository = reportParameterDefinitionRepository;
            _roleManager = roleManager;
        }


        public async Task CreateReport(Report report, List<int> grantedRoles, List<long> excludedUsers, List<ReportParameterDto> parameters)
        {
            var roles = await _roleManager.Roles.Select(x => x.Id).ToListAsync();
            bool anyInvalidRole = grantedRoles.Except(roles).Any();
            if (anyInvalidRole) throw new UserFriendlyException(L("YouMustSelectAValidRole"));

            var reportPermissionByRole = grantedRoles
                .Select(roleId => new ReportPermission { RoleId = roleId, IsGranted = true }).ToList();
            var reportPermissionByUser = excludedUsers
                .Select(userId => new ReportPermission { UserId = userId, IsGranted = false }).ToList();
            report.ReportPermissions = reportPermissionByRole.Union(reportPermissionByUser).ToList();
            var selectedParameterNames = parameters.Select(x => x.Name).ToArray();
            if (ReportParameterDefinitionProvider.AnyParameterNotDefined(ReportType.TripDetailsReport,selectedParameterNames))
                throw new UserFriendlyException(L("NotValidParameterIsSelected"));

            report.Parameters = (from selectedParameter in parameters
                from paraDefinition in _reportParameterDefinitionRepository.GetAll()
                where paraDefinition.Name == selectedParameter.Name
                select new ReportParameter
                {
                    ReportParameterDefinitionId = paraDefinition.Id, Value = selectedParameter.Value
                }).ToList();
            
           await _reportRepository.InsertAsync(report);
        }
    }
}