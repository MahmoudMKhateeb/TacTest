using System;
using System.Collections.Generic;

namespace TACHYON.AddressBook.Dtos
{
    public class GetFacilityForViewOutput
    {
        public FacilityDto Facility { get; set; }
        public string CityDisplayName { get; set; }
        public string Country { get; set; }
        public string FacilityName { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public DateTime CreationTime { get; set; }
        public List<FacilityWorkingHourDto> FacilityWorkingHours { get; set; }
    }
}