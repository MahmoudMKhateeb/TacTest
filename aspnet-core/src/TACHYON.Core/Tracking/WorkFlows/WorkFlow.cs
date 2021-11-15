using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Tracking.WorkFlows
{
    public class WorkFlow
    {
        public int Version { get; set; }
        public List<PointTransaction> Transactions { get; set; }
    }
}