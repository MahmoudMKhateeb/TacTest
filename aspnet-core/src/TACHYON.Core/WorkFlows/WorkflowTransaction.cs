using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TACHYON.WorkFlows
{
    public class WorkflowTransaction<TArgs, TEnum> : IWorkflowTransaction where TEnum : Enum
    {
        public string Action { get; set; }
        public string Name { get; set; }
        public TEnum FromStatus { get; set; }
        public TEnum ToStatus { get; set; }
        public Func<TArgs, Task<string>> Func { get; set; }
        public List<string> Permissions { get; set; }
        public List<string> Features { get; set; }
    }
}