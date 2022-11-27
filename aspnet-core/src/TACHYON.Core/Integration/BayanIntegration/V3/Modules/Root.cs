using System.Collections.Generic;

namespace TACHYON.Integration.BayanIntegration.V3.Modules
{
    public class Root
    {
        public Vehicle vehicle { get; set; }
        public Driver driver { get; set; }
       // public ExtraDriver extraDriver { get; set; }
        public Carrier carrier { get; set; }
        public string receivedDate { get; set; }
        public string expectedDeliveryDate { get; set; }
        public string notes { get; set; }
        public List<Waybill> waybills { get; set; }
    }
}