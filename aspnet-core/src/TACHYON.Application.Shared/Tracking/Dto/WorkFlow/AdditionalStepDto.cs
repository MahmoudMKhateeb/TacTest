using TACHYON.Tracking.AdditionalSteps;

namespace TACHYON.Tracking.Dto.WorkFlow
{
    public class AdditionalStepDto
    {
        public string Action { get; set; }
        
        public string Name { get; set; }
        
        public AdditionalStepType StepType { get; set; }
    }
}