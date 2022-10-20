using System.Collections.Generic;

namespace TACHYON.Integration.BayanIntegration.V3.Modules
{
    public class Waybill
    {
        public string waybillId { get; set; }
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public ReceivingLocation receivingLocation { get; set; }
        public DeliveryLocation deliveryLocation { get; set; }
        public List<Item> items { get; set; }
        public int fare { get; set; }
        public bool tradable { get; set; }
        public string extraCharges { get; set; }
        public int? paymentMethodId { get; set; }
        public string paymentComment { get; set; }
        public bool paidBySender { get; set; }
        public bool deliverToClient { get; set; }
    }
}