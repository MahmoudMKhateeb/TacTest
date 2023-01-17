using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class NeedsActionTripDto
    {
        public string Origin { get; set; }
        
        public List<string> Destinations { get; set; }

        public long? WaybillNumber { get; set; }

        public bool NeedsReceiverCode { get; set; }
        
        public bool NeedsPod { get; set; }
    }
}