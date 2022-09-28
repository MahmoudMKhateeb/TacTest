using System.Collections.Generic;
using TACHYON.Dto;

namespace TACHYON.ServiceAreas
{
    public class ServiceAreaListItemDto
    {
        public string CountryName { get; set; }

        public List<SelectItemDto> Cities { get; set; }
    }
}