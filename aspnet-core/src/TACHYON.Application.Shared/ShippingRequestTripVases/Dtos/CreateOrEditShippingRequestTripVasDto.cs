using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.ShippingRequestTripVases.Dtos
{
    public class CreateOrEditShippingRequestTripVasDto : Entity<long?>
    {
        public long? ShippingRequestVasId { get; set; }
        public int? ShippingRequestTripId { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// This field in helper for front to hide specific vases "appointment, clearance" from user
        /// </summary>
        public bool ShouldHide { get; set; }
    }
}