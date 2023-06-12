using Abp.Collections.Extensions;
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
using TACHYON.Reports.ReportPermissions;

namespace TACHYON.AutoMapper.Reports {

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        // todo add map configuration for this mapping 
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
                var excludedCompanies = dto.ExcludedTenantIds.Select(tenantId => new ReportDefinitionPermission
                {
                    TenantId = tenantId,
                    IsGranted = false,
                }).ToList();
                reportPermissions.AddRange(grantedEditions);
                reportPermissions.AddRange(excludedCompanies);
                definition.DefinitionPermissions = reportPermissions;
            }));

        CreateMap<ReportDefinition, ReportDefinitionListItemDto>()
            .ForMember(x => x.GrantedEditions, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.EditionId.HasValue && r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.EditionId.HasValue && r.IsGranted)
                        .Select(x => x.Edition.DisplayName).ToArray().JoinAsString(",")
                    : "__"))
            .ForMember(x => x.ExcludedCompanies, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.TenantId.HasValue && !r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.TenantId.HasValue && !r.IsGranted)
                        .Select(r => r.Tenant.Name).ToArray().JoinAsString(",")
                    : "__"))
            .ForMember(x => x.ReportType, x => x.MapFrom(i => i.Type.ToString()));

        CreateMap<StaticReportParameterDefinition, ReportParameterDefinitionItemDto>();
        CreateMap<ReportDefinition, ClonedReportDefinitionDto>()
            .ForMember(x=> x.ParameterDefinitions, x=> x.MapFrom(i=> i.ParameterDefinitions.Select(p=> p.Name).ToList()))
            .ForMember(x => x.GrantedEditionIds, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.EditionId.HasValue && r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.EditionId.HasValue && r.IsGranted)
                        .Select(x => x.EditionId.Value).ToList()
                    : Enumerable.Empty<int>().ToList()))
            .ForMember(x => x.ExcludedTenantIds, x => x.MapFrom(i =>
                i.DefinitionPermissions.Any(r => r.TenantId.HasValue && !r.IsGranted)
                    ? i.DefinitionPermissions.Where(r => r.TenantId.HasValue && !r.IsGranted)
                        .Select(r => r.TenantId.Value).ToList()
                    : Enumerable.Empty<int>().ToList()));

        CreateMap<CreateOrEditReportDto, Report>()
            .ForMember(x => x.ReportPermissions, x => x.Ignore())
            .ForMember(x => x.Parameters, x => x.Ignore());
    }
}
}