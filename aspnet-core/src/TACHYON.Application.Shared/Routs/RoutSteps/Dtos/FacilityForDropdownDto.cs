﻿using TACHYON.AddressBook;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class FacilityForDropdownDto
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        public int CityId { get; set; }
        public FacilityType FacilityType { get; set; }
    }
}