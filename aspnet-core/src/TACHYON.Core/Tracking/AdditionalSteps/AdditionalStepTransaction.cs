using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TACHYON.Routs.RoutPoints;
using TACHYON.WorkFlows;

namespace TACHYON.Tracking.AdditionalSteps
{
    public class AdditionalStepTransaction<TArgs,TEnum> : IWorkflowTransaction where TEnum : Enum
    {
        public string Action { get; set; }
        public string Name { get; set; }
        public TEnum AdditionalStepType { get; set; }
        public RoutePointDocumentType? RoutePointDocumentType { get; set; }
        public Func<TArgs, Task<string>> Func { get; set; }

        /// <summary>
        /// if a transaction is required then the trip will not be delivered until this transaction is completed
        /// </summary>
        public bool IsRequired { get; set; }
    }
}