using Newtonsoft.Json;
using System.Collections.Generic;

namespace TACHYON.Polygons
{
    public class PolygonListRoot
    {
        [JsonProperty("type")] 
        public string Type { get; set; }

        [JsonProperty("features")] 
        public List<CityPolygon> Features { get; set; }
    }
}