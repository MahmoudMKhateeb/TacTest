namespace TACHYON.Trucks.Dtos
{
    public class GetTruckForViewDto
    {
        public TruckDto Truck { get; set; }

        public string TrucksTypeDisplayName { get; set; }

        public string TruckStatusDisplayName { get; set; }

       // public string UserName { get; set; }

        public bool IsMissingDocumentFiles { get; set; }

        //public string UserName2 { get; set; }
    }
}