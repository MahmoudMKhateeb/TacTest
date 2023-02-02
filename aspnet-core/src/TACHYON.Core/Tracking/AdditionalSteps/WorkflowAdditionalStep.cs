using System;

namespace TACHYON.Tracking.AdditionalSteps
{
    public class WorkflowAdditionalStep
    {
        public AdditionalStepType StepType { get; set; }

        public string DisplayName { get; set; }
        
        public bool IsRequiredToDeliver { get; set; }

       // public Func StepFunc { get; set; } TODO
        
        
    }
}