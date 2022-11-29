﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.DedicatedDynamicInvoices.DedicatedDynamicInvoiceItems;
using TACHYON.DedicatedInvoices;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trucks;

namespace TACHYON.Shipping.Dedicated
{
    [Table("DedicatedShippingRequestTrucks")]
    public class DedicatedShippingRequestTruck: FullAuditedEntity<long>
    {
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequest { get; set; }
        public long TruckId { get; set; }
        [ForeignKey("TruckId")]
        public Truck Truck { get; set; }
        public WorkingStatus Status { get; set; }

        public ICollection<DedicatedShippingRequestTruckAttendance> DedicatedShippingRequestTruckAttendances { get; set; }
        [NotMapped]
        public ICollection<DedicatedDynamicInvoiceItem> DedicatedDynamicInvoiceItems { get; set; }
        public double? KPI { get; set; }

        #region Replacement
        public bool IsRequestedToReplace { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string ReplacementReason { get; set; }
        public ReplacementFlag ReplacementFlag { get; set; }
        public int? ReplacementIntervalInDays { get; set; }
        public long? OriginalDedicatedTruckId { get; set; }
        /// <summary>
        /// If truck is replacement, original truck will be added here
        /// </summary>
        [ForeignKey("OriginalTruckId")]
        public DedicatedShippingRequestTruck OriginalTruck { get; set; }
        #endregion
    }
}
