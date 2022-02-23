using Newtonsoft.Json;

namespace TACHYON.Polygons
{
    public class CityPolygon
    {
        [JsonProperty("type")] 
        public string Type { get; set; }
        [JsonProperty("properties")] 
        public CityPolygonProperties Properties { get; set; }
        [JsonProperty("geometry")] 
        public object Geometry { get; set; }
    }

}