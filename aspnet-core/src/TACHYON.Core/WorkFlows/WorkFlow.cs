using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.WorkFlows
{
    public class WorkFlow<TTransaction> where TTransaction : IWorkflowTransaction
    {
        public int Version { get; set; }
        public List<TTransaction> Transactions { get; set; }
    }
}