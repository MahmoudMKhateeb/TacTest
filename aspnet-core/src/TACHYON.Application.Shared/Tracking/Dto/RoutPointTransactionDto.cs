using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.Tracking.Dto
{
    public class RoutPointTransactionDto
    {

        public RoutePointStatus Status { get; set; }
        public bool IsDone { get; set; }
        public string Name { get; set; }
        public DateTime? CreationTime { get; set; }
    }

    public class RoutPointTransactionArgDto
    {
        public RoutePointStatus Status { get; set; }
        public DateTime CreationTime { get; set; }
    }
}