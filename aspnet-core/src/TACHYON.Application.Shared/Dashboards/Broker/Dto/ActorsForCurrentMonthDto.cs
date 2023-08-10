namespace TACHYON.Dashboards.Broker
{
    public class ActorsForCurrentMonthDto
    {
        public int TotalActorsForCurrentMonth { get; set; }
        
        public int GrowthChangePercentage { get; set; }

        public decimal CarrierActorsPercentage { get; set; }
        
        public decimal ShipperActorsPercentage { get; set; }
    }
}