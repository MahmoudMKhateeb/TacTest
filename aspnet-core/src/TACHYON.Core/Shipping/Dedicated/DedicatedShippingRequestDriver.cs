using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Authorization.Users;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.Dedicated
{
    [Table("DedicatedShippingRequestDrivers")]
    public class DedicatedShippingRequestDriver :FullAuditedEntity<long>
    {
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequest { get; set; }
        public long DriverUserId { get; set;}
        [ForeignKey("DriverUserId")]
        public User DriverUser { get; set; }
        public WorkingStatus Status { get; set; }
        #region Replacement
        public bool IsRequestedToReplace { get; set; }
        public DateTime? ReplacementDate { get; set; }
        public string ReplacementReason { get; set; }
        public ReplacementFlag ReplacementFlag { get; set; }
        public int ReplacementIntervalInDays { get; set; }
        public long? OriginalDedicatedDriverId { get; set; }
        /// <summary>
        /// If truck is replacement, original truck will be added here
        /// </summary>
        [ForeignKey("OriginalDedicatedDriverId")]
        public User OriginalDriver { get; set; }
        #endregion


    }
}
