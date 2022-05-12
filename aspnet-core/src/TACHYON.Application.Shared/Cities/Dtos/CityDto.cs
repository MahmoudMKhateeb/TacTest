using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Cities.Dtos
{
    public class CityDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Code { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool HasPolygon { get; set; }

        public int CountyId { get; set; }
        public bool IsActive { get; set; }

        public string TranslatedDisplayName { get; set; }
    }
}