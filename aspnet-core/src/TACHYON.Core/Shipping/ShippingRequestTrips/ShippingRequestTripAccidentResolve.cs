using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;

namespace TACHYON.Shipping.ShippingRequestTrips
{
    [Table("ShippingRequestTripAccidentResolves")]
    public class ShippingRequestTripAccidentResolve : FullAuditedEntity, IHasDocument
    {
        public int AccidentId { get; set; }
        [ForeignKey("AccidentId")]
        public ShippingRequestTripAccident AccidentFK { get; set; }
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
    }
}
