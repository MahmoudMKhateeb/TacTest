using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Notes.Dto
{
    public class GetNotesShippingRequestAndTripInput
    {
        public long? ShippingRequestId { get; set; }
        public long? TripId { get; set; }

    }
}