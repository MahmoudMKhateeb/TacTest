using Abp.Application.Features;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TACHYON.Features;
using TACHYON.Reports.ReportParameterDefinitions;

namespace TACHYON.Reports.ReportParameters
{
    public class ReportParameterManager : TACHYONDomainServiceBase,IReportParameterManager
    {
        
        private readonly IRepository<Report,Guid> _reportRepository;
        private readonly IReportParameterDefinitionProvider _definitionProvider;
        private readonly IFeatureChecker _featureChecker;

        public ReportParameterManager(
            IRepository<Report, Guid> reportRepository,
            IReportParameterDefinitionProvider definitionProvider,
            IFeatureChecker featureChecker)
        {
            _reportRepository = reportRepository;
            _definitionProvider = definitionProvider;
            _featureChecker = featureChecker;
        }

        public async Task<List<ReportParameterItem>> GetReportParameters(Guid reportId)
        {
            bool isCarrier = await _featureChecker.IsEnabledAsync(AppFeatures.Carrier);
            bool isShipper = await _featureChecker.IsEnabledAsync(AppFeatures.Shipper);
            
            var list = await (from report in _reportRepository.GetAll()
                    where report.Id == reportId
                    from parameter in report.Parameters
                    select new 
                        {
                            parameter.ParameterDefinition.Name,
                            ReportType = report.ReportDefinition.Type,
                            parameter.Value
                        }).ToListAsync();

            return (from parameter in list
                    from definitionItem in _definitionProvider.GetParameterDefinitions(parameter.ReportType)
                    where definitionItem.Name == parameter.Name
                    select new ReportParameterItem
                    {
                        Name = parameter.Name,
                        Value = parameter.Value,
                        Type = _definitionProvider.IsParameterDefined(parameter.Name, parameter.ReportType)
                            ? _definitionProvider.GetParameterDefinition(parameter.ReportType, parameter.Name).Type
                            : null,
                        Expression = definitionItem.ExpressionCallback(new ReportParameterExpressionCallbackArgs{ParameterValue = parameter.Value, IsCarrier = isCarrier, IsShipper = isCarrier})
                    }
                ).ToList();
        }
        
    }
}
