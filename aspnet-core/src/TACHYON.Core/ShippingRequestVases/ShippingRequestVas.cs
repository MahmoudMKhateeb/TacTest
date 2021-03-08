using TACHYON.Vases;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.ShippingRequestVases
{
    [Table("ShippingRequestVases")]
    public class ShippingRequestVas : FullAuditedEntity<long>
    {
        public virtual double? DefualtPrice { get; set; }

        public virtual double? ActualPrice { get; set; }

        public virtual int RequestMaxAmount { get; set; }

        public virtual int RequestMaxCount { get; set; }

        public virtual int VasId { get; set; }

        [ForeignKey("VasId")]
        public Vas VasFk { get; set; }

        public virtual long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]  
        public ShippingRequest ShippingRequestFk { get; set; }
        //number of trips for each vas, each vas shouldn't assign to more than this number
        public int NumberOfTrips { get; set; }
    }
}