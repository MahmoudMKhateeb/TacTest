using TACHYON.Dto;

namespace TACHYON.Cities.Dtos
{
    public class CityPolygonLookupTableDto : SelectItemDto
    {
        public string Polygon { get; set; }

        public int CountryId { get; set; }
        public bool HasPolygon { get; set; }
    }
}