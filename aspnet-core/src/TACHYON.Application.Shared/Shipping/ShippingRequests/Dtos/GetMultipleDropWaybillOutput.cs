using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetMultipleDropWaybillOutput
    {
        //Base Info
        public long MasterWaybillNo { get; set; }
        public long SubWaybillNo { get; set; }
        public string Date { get; set; }
        public string ShippingRequestStatus { get; set; }
        public int InvoiceNumber { get; set; }
        public string ShipperReference { get; set; }
        public string StartTripDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string CarrierName { get; set; }
        public string ClientName { get; set; }
        public double TotalWeight { get; set; }
        public string GoodsCategoryDisplayName { get; set; }


        //Sender Details Info
        public string SenderCompanyName { get; set; }
        public string SenderContactName { get; set; }
        public string SenderMobile { get; set; }
        // Receiver Details info
        public string ReceiverCompanyName { get; set; }
        public string ReceiverContactName { get; set; }
        public string ReceiverMobile { get; set; }

        //Driver Details
        public string DriverName { get; set; }
        public string DriverIqamaNo { get; set; }

        //Truck Info
        public string TruckTypeDisplayName { get; set; }
        public string PlateNumber { get; set; }

        //Drops Details
        public string PackingTypeDisplayName { get; set; }
        public int NumberOfPacking { get; set; }

        //Delivery Location
        public string DroppFacilityName { get; set; }
        public string DroppCountryName { get; set; }
        public string DroppCityName { get; set; }
        public string DroppArea { get; set; }
    }
}
