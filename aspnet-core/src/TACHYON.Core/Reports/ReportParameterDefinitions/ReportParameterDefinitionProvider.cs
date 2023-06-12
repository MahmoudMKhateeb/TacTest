using System;
using System.Collections.Generic;
using System.Linq;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    public static class ReportParameterDefinitionProvider
    {
        private static readonly Dictionary<ReportType, List<StaticReportParameterDefinition>> ParameterDefinitions;

         static ReportParameterDefinitionProvider()
         {
             ParameterDefinitions = new Dictionary<ReportType, List<StaticReportParameterDefinition>>();
             RegisterStaticParameterDefinitions();
         }


         // Register the Parameters Definitions of your Report Type
         // by using Register(ReportType.YourReportType, new StaticReportParameterDefinition{Name = "ParameterName", Type = typeof(ParameterType)});
         private static void RegisterStaticParameterDefinitions()
         {
             // todo add constant for parameters names and localization

             #region Trip Details Report Parameters Definitions
             Register(ReportType.TripDetailsReport, new StaticReportParameterDefinition{Name = "ShipperName", Type = typeof(string)});
             Register(ReportType.TripDetailsReport, new StaticReportParameterDefinition{Name = "CarrierName", Type = typeof(string)});
             Register(ReportType.TripDetailsReport, new StaticReportParameterDefinition{Name = "DeliveryDate", Type = typeof(DateTime)});
             #endregion
             
         }
         
         public static IEnumerable<StaticReportParameterDefinition> GetParameterDefinitions(ReportType type)
         {
             return ParameterDefinitions.TryGetValue(type, out List<StaticReportParameterDefinition> definitions)
                 ? definitions
                 : Enumerable.Empty<StaticReportParameterDefinition>();
         }

         public static bool IsParameterDefined(string parameterName, ReportType reportType)
         {
             return GetParameterDefinitions(reportType).Any(parameter => parameter.Name == parameterName);
         }         
         public static bool AnyParameterNotDefined(ReportType reportType, params string[] parameterNames)
         {
             var parameters = GetParameterDefinitions(reportType);
             return !parameterNames.All(parameterName => parameters.Any(x => x.Name == parameterName));
         }
         #region Helpers

         private static void Register(ReportType type, StaticReportParameterDefinition parameterDefinition)
         {
             if (!ParameterDefinitions.ContainsKey(type))
             {
                 ParameterDefinitions[type] = new List<StaticReportParameterDefinition>();
             }
             
             ParameterDefinitions[type].Add(parameterDefinition);
         }

         #endregion
    }
}