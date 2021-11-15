using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Tracking.Dto.WorkFlow
{
    public class PointTransactionDto
    {
        public string Action { get; set; }
        public string Name { get; set; }
        public RoutePointStatus FromStatus { get; set; }
        public RoutePointStatus ToStatus { get; set; }
        public bool IsOptional { get; set; }
        public bool IsResolved { get; set; }
        public PickingType PickingType { get; set; }
    }
}