namespace TACHYON.AddressBook.Dtos
{
    public class GetFacilityForViewOutput
    {
		public FacilityDto Facility { get; set; }
		public string CityDisplayName { get; set;}
        public string FacilityName { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }

    }
}