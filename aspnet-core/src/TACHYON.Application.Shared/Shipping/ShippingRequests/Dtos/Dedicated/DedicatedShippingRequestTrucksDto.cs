using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Shipping.Dedicated;

namespace TACHYON.Shipping.ShippingRequests.Dtos.Dedicated
{
    public class DedicatedShippingRequestTrucksDto: EntityDto<long>
    {
        public string TruckType { get; set; }
        public string Status { get; set; }
        public string PlateNumber { get; set; }
        public string Capacity { get; set; }
        public string ShippingRequestReference { get; set; }
        public string CarrierName { get; set; }
        public string Duration { get; set; }
        public double? KPI { get; set; }
        public int NumberOfTrips { get; set; }
        public bool IsRequestedToReplace { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string ReplacementReason { get; set; }
        public ReplacementFlag ReplacementFlag { get; set; }
        public int ReplacementIntervalInDays { get; set; }
        public long? OriginalDedicatedTruckId { get; set; }
        public string OriginalDedicatedTruckName { get; set; }
        public long? InvoiceId { get; set; }
        public long? SubmitInvoiceId { get; set; }

    }
}
