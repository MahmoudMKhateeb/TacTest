using Newtonsoft.Json;

namespace TACHYON.Actors
{
    public enum ActorTypesEnum : byte
    {
        Shipper = 1 ,
        Carrier = 2,
        [JsonIgnore]
        MySelf = 3
    }
}
