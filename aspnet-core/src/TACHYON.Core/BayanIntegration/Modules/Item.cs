namespace TACHYON.BayanIntegration.Modules
{
    public class Item
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Not Required
        /// </summary>
        public int? DangerousGoodTypeId { get; set; }
        public double Weight { get; set; }
        /// <summary>
        /// Not Required
        /// </summary>
        public string Dimensions { get; set; }
        /// <summary>
        /// Not Required
        /// </summary>
        public string DangerousCode { get; set; }
        /// <summary>
        /// Not Required
        /// </summary>
        public string ItemNumber { get; set; }
    }
}