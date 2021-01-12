using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class GetRoutPointForViewDto
    {
        public RoutPointDto RoutPointDto { get; set; }
        public string CityName { get; set; }
        public string PickingTypeDisplayName{get; set;}
        public string FacilityName { get; set; }
    }
}
