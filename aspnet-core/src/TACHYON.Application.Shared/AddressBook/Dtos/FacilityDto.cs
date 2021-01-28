
using System;
using Abp.Application.Services.Dto;
using NetTopologySuite.Geometries;

namespace TACHYON.AddressBook.Dtos
{
    public class FacilityDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Adress { get; set; }

        //public Point Location { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }

        public int CityId { get; set; }


    }
}