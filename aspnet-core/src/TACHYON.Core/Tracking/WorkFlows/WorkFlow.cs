using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Tracking.WorkFlows
{
    public class WorkFlow<TArgs, TEnum> where TEnum : Enum
    {
        public int Version { get; set; }
        public List<WorkflowTransaction<TArgs, TEnum>> Transactions { get; set; }
    }
}