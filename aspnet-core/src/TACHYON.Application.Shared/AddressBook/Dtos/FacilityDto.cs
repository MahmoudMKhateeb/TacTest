using System;
using Abp.Application.Services.Dto;
using NetTopologySuite.Geometries;

namespace TACHYON.AddressBook.Dtos
{
    public class FacilityDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Address { get; set; }

        //public Point Location { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public int CityId { get; set; }
        public string FacilityWorkingHours { get; set; }
    }
}