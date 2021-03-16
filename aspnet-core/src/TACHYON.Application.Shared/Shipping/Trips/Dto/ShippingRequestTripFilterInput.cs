using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
   public class ShippingRequestTripFilterInput: PagedAndSortedResultRequestDto
    {
        public long RequestId { get; set; }
        public ShippingRequestTripStatus? Status { get; set; }
       
    }
}
