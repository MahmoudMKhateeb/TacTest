using System.Collections.Generic;

namespace TACHYON.BayanIntegration.Modules
{
    public class RootForCreate
    {
        public Sender Sender { get; set; }
        public Recipient Recipient { get; set; }
        public PickUpLocation PickUpLocation { get; set; }
        public DropOffLocation DropOffLocation { get; set; }
        public List<Item> Items { get; set; }
        public Vehicle Vehicle { get; set; }
        public Driver Driver { get; set; }
        public int TotalFare { get; set; }
        public string PickUpDate { get; set; }
        public string PickUpAddress { get; set; }
        public string DropOffDate { get; set; }
        public string DropOffAddress { get; set; }

        /// <summary>
        /// always will be False
        /// </summary>
        public bool Tradable { get; set; } = false;
        public string ExtraCharges { get; set; }
        /// <summary>
        /// Not Required
        /// </summary>
        public string PaymentMethod { get; set; }
        /// <summary>
        /// Not Required
        /// </summary>
        public string PaymentComment { get; set; }

        /// <summary>k
        /// paidBy either sender or recipient
        /// always will be the Sender in Tachyon 
        /// </summary>
        public string PaidBy { get; set; } = "Sender";
    }
}