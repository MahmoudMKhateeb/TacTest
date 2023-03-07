using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Routs.RoutPoints;
using TACHYON.Tracking.AdditionalSteps;

namespace TACHYON.Tracking.Dto.WorkFlow
{
    public class AdditionalStepTransitionDto
    {
        public AdditionalStepType AdditionalStepType { get; set; }
        public RoutePointDocumentType RoutePointDocumentType { get; set; }
        public string AdditionalStepTypeTitle { get; set; }
        public string RoutePointDocumentTypeTitle { get; set; }
        public string FileContentType { get; set; }
        public bool IsReset { get; set; }
    }
}
