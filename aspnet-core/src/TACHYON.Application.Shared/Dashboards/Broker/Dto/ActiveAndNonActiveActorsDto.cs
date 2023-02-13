namespace TACHYON.Dashboards.Broker
{
    public class ActiveAndNonActiveActorsDto
    {
        public int ActiveShipperActorsCount { get; set; }
        
        public int ActiveCarrierActorsCount { get; set; }
        
        public int NonActiveShipperActorsCount { get; set; }
        
        public int NonActiveCarrierActorsCount { get; set; }
    }
}