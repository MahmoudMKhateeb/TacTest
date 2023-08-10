using System;
using System.Collections.Generic;
using TACHYON.Dto;

namespace TACHYON.Reports.ReportParameterDefinitions
{
    public class ReportParameterDefinitionItem
    {
        public string DisplayName { get; set; }
        public string ParameterName { get; set; }

        public ReportParameterType ParameterType { get; set; }

        public List<SelectItemDto> ListData { get; set; }
    }
}
