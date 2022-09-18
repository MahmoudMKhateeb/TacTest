using Newtonsoft.Json;
using TACHYON.Common;

namespace TACHYON.Integration.BayanIntegration.V3.Modules
{
    public class Item
    {
        public int? unitId { get; set; }
        public bool deliverToClient { get; set; }
        public bool valid { get; set; }
        public int quantity { get; set; }
        public string price { get; set; }
        public string goodTypeId { get; set; }
        public string dangerousGoodTypeId { get; set; }
        public double weight { get; set; }
        public string dimensions { get; set; }

        public string dangerousCode { get; set; }
        public string itemNumber { get; set; }
    }
}