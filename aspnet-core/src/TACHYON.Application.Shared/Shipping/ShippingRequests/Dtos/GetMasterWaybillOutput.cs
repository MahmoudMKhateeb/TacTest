using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.MultiTenancy.Payments.Dto;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetMasterWaybillOutput
    {
        //Base Info
        public long MasterWaybillNo { get; set; }
        public string Date { get; set; }
        public string ShippingRequestStatus { get; set; }

        /// <summary>
        /// Shipper Invoice No
        /// </summary>
        public string InvoiceNumber { get; set; }

        public string ShipperReference { get; set; }
        public double TotalWeight { get; set; }
        public string ShipperNotes { get; set; }

        //Sender Details Info
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Mobile { get; set; }

        //Driver Details
        public string DriverName { get; set; }
        public string DriverIqamaNo { get; set; }

        //Truck Info
        public string TruckTypeDisplayName { get; set; }
        public string PlateNumber { get; set; }

        //Drops Details
        public bool IsMultipDrops { get; set; }
        public int TotalDrops { get; set; }
        public string PackingTypeDisplayName { get; set; }
        public int NumberOfPacking { get; set; }

        //Pickup Loction
        public string FacilityName { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string Area { get; set; }
        public string StartTripDate { get; set; }

        //carrier name
        public string CarrierName { get; set; }

        //client name
        public string ClientName { get; set; }
    }
}