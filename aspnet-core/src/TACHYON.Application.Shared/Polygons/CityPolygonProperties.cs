using Newtonsoft.Json;

namespace TACHYON.Polygons
{
    
    public class CityPolygonProperties
    {
        [JsonProperty("name")] 
        public string CityId { get; set; }
    }
}