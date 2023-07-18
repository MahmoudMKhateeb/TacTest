using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using TACHYON.ReportParameterDefinitions;
using TACHYON.Reports;
using TACHYON.Reports.Dto;
using TACHYON.Reports.ReportDefinitionPermissions;
using TACHYON.Reports.ReportDefinitions;
using TACHYON.Reports.ReportDefinitions.Dto;
using TACHYON.Reports.ReportParameterDefinitions;

namespace TACHYON.AutoMapper.Reports {

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        ILocalizationManager localizationManager = IocManager.Instance.Resolve<ILocalizationManager>();
        ILocalizationSource localizationSource = localizationManager.GetSource(TACHYONConsts.LocalizationSourceName);
        
        CreateMap<CreateOrEditReportDefinitionDto, ReportDefinition>()
            .ForMember(x=>x.DefinitionPermissions,x=> x.Ignore())
            .ForMember(x=>x.ParameterDefinitions,x=> x.Ignore())
            .AfterMap(((dto, definition) =>
            {
                var reportPermissions = new List<ReportDefinitionPermission>();
                var grantedEditions = dto.GrantedEditionIds.Select(x => new ReportDefinitionPermission
                {
                    EditionId = x, IsGranted = true
                }).ToList();
                var excludedCompanies = dto.ExcludedTenantIds?.Select(tenantId => new ReportDefinitionPermission
                {
                    TenantId = tenantId,
                    IsGranted = false,
                }).ToList();
                reportPermissions.AddRange(grantedEditions);
                if (excludedCompanies is { Count: > 0 }) 
                    reportPermissions.AddRange(excludedCompanies);
                definition.DefinitionPermissions = reportPermissions;
            }));

        CreateMap<ReportDefinition, ReportDefinitionListItemDto>()
            .ForMember(x => x.GrantedEditions, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.EditionId.HasValue && r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.EditionId.HasValue && r.IsGranted)
                        .Select(r => localizationSource.GetString(r.Edition.DisplayName)).ToArray().JoinAsString(", ")
                    : "__"))
            .ForMember(x => x.ExcludedCompanies, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.TenantId.HasValue && !r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.TenantId.HasValue && !r.IsGranted)
                        .Select(r => r.Tenant.Name).ToArray().JoinAsString(", ")
                    : "__"))
            .ForMember(x => x.ReportType, x => x.MapFrom(i => i.Type.ToString()));

        CreateMap<StaticReportParameterDefinition, ReportParameterDefinitionItemDto>()
            .ForMember(x=> x.DisplayName, x=> x.MapFrom(i=> localizationSource.GetString(i.Name)));
        CreateMap<ReportDefinition, ClonedReportDefinitionDto>()
            .ForMember(x=> x.ParameterDefinitions, x=> x.MapFrom(i=> i.ParameterDefinitions.Select(p=> p.Name).ToList()))
            .ForMember(x => x.GrantedEditionIds, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.EditionId.HasValue && r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.EditionId.HasValue && r.IsGranted)
                        .Select(r => r.EditionId.Value).ToList()
                    : Enumerable.Empty<int>().ToList()))
            .ForMember(x => x.ExcludedTenantIds, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.TenantId.HasValue && !r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.TenantId.HasValue && !r.IsGranted)
                        .Select(r => r.TenantId.Value).ToList()
                    : Enumerable.Empty<int>().ToList()));

        CreateMap<CreateOrEditReportDto, Report>()
            .ForMember(x => x.ReportPermissions, x => x.Ignore())
            .ForMember(x => x.Parameters, x => x.Ignore());

        CreateMap<Report, ReportListItemDto>()
            .ForMember(x => x.GrantedRoles, x => x.MapFrom(i =>
                i.ReportPermissions.Any(p => p.RoleId.HasValue && p.IsGranted)
                    ? i.ReportPermissions.Where(p => p.RoleId.HasValue && p.IsGranted)
                        .Select(p => p.Role.DisplayName).ToArray().JoinAsString(", ")
                    : "__"))
            .ForMember(x => x.ExcludedUsers, x => x.MapFrom(i =>
                i.ReportPermissions.Any(p => p.UserId.HasValue && !p.IsGranted)
                    ? i.ReportPermissions.Where(p => p.UserId.HasValue && !p.IsGranted)
                        .Select(p => $"{p.User.Name} {p.User.Surname}").ToArray().JoinAsString(", ")
                    : "__"))
            .ForMember(x=> x.FormatTitle,x=> x.MapFrom(i=> i.Format.ToString()))
            .ForMember(x=> x.DefinitionName,x=> x.MapFrom(i=> i.ReportDefinition.DisplayName));
    }
}
}