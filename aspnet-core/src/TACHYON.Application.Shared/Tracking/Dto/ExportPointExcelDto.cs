using Abp.Application.Services.Dto;
using System.Collections.Generic;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Tracking.Dto
{
    public class ExportPointExcelDto : EntityDto<long>
    {
        public int WorkFlowVersion { get; set; }

        public PickingType PickingType { get; set; }

        public List<string> Transactions { get; set; }
    }
}