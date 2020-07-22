
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Cities.Dtos
{
    public class CityDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Code { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }


        public int CountyId { get; set; }


    }
}