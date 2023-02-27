namespace TACHYON.Dashboards.Broker
{
    public class ActorsForCurrentMonthDto
    {
        public int TotalActorsForCurrentMonth { get; set; }
        
        public int GrowthChangePercentage { get; set; }

        public int CarrierActorsPercentage { get; set; }
        
        public int ShipperActorsPercentage { get; set; }
    }
}