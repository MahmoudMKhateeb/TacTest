namespace TACHYON.Trucks.Dtos
{
    public class GetTruckForViewOutput
    {
        public TruckDto Truck { get; set; }

        public string TrucksTypeDisplayName { get; set; }

        public string TruckStatusDisplayName { get; set; }
        public bool IsMissingDocumentFiles { get; set; }
    }
}