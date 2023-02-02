using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Tracking.AdditionalSteps
{
    [Table("AdditionalStepTransitions")]
    public class AdditionalStepTransition : CreationAuditedEntity<long>
    {
        public long RoutePointId { get; set; }
        
        [ForeignKey(nameof(RoutePointId))] 
        public RoutPoint RoutePoint { get; set; }
        
        public AdditionalStepType AdditionalStepType { get; set; }
        
        public bool IsReset { get; set; }
    }
}