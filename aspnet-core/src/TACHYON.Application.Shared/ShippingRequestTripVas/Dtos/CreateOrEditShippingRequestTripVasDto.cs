using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.ShippingRequestTripVas.Dtos
{
    public class CreateOrEditShippingRequestTripVasDto : Entity<long?>
    {
        public long? ShippingRequestVasId { get; set; }
        public int? ShippingRequestTripId { get; set; }
    }
}
