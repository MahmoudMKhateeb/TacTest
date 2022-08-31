using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Cities;

namespace TACHYON.Shipping.ShippingRequests
{
    [Table("ShippingRequestDestinationCities")]
    public class ShippingRequestDestinationCity: FullAuditedEntity
    {
        public long ShippingRequestId { get; set; }
        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFK { get; set; }
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City CityFk { get; set; }
    }
}
