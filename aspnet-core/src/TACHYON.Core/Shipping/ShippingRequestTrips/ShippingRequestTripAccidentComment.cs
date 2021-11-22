using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;
using TACHYON.MultiTenancy;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Accidents;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripAccidentComments")]
    public class ShippingRequestTripAccidentComment : FullAuditedEntity, IMustHaveTenant
    {
        public string Comment { get; set; }
        public int AccidentId { get; set; }

        [ForeignKey("AccidentId")]
        public ShippingRequestTripAccident AccidentFK { get; set; }
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant TenantFK { get; set; }

    }

}