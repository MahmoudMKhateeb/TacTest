using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Authorization.Roles;
using TACHYON.Dto;
using TACHYON.Reports.Dto;
using TACHYON.Reports.ReportDefinitions;

namespace TACHYON.Reports;

// todo add permission for all actions 
public class ReportAppService : TACHYONAppServiceBase
{
    private readonly RoleManager _roleManager;
    private readonly IRepository<ReportDefinition> _reportDefinitionRepository;
    private readonly IReportManager _reportManager;

    public ReportAppService(
        RoleManager roleManager,
        IRepository<ReportDefinition> reportDefinitionRepository,
        IReportManager reportManager)
    {
        _roleManager = roleManager;
        _reportDefinitionRepository = reportDefinitionRepository;
        _reportManager = reportManager;
    }

    public async Task CreateOrEdit(CreateOrEditReportDto input)
    {
        if (input.Id.HasValue)
        {
            await Create(input);
            return;
        }

        await Update(input);
    }

    protected virtual async Task Create(CreateOrEditReportDto input)
    {
        // add mapping configurations
        var createdReport = ObjectMapper.Map<Report>(input);

        await _reportManager.CreateReport(createdReport, input.GrantedRoles, input.ExcludedUsers, input.Parameters);
    }    
    protected virtual async Task Update(CreateOrEditReportDto input)
    {
        throw new UserFriendlyException(L("UpdateReportNotSupportedYet"));
    }
    
    #region Drop-Downs

    public async Task<List<SelectItemDto>> GetReportDefinitionsForDropdown()
    {
        var reportDefinitionList = await _reportDefinitionRepository.GetAll()
            .Select(x => new SelectItemDto { DisplayName = x.DisplayName, Id = x.Id.ToString() })
            .ToListAsync();
        return reportDefinitionList;
    }
    public async Task<List<SelectItemDto>> GetTenantRoles()
    {
        
        return await _roleManager.Roles
            .Select(x => new SelectItemDto { DisplayName = x.DisplayName, Id = x.Id.ToString() }).ToListAsync();
    }

    public async Task<List<SelectItemDto>> GetTenantUsers(params int[] roleIds)
    {
        return await UserManager.Users.Where(x=> x.Roles.Any(r=> roleIds.Contains(r.RoleId)) && !x.IsDriver)
            .Select(x => new SelectItemDto { DisplayName = $"{x.Name} {x.Surname}" }).ToListAsync();
    }

    #endregion
}