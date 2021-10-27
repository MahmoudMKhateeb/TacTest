using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Accidents.Comments.Dto
{
    public class GetAllForShippingRequestTripAccidentCommentFilterInput
    {
        public long AccidentId { get; set; }
        public string Sorting { get; set; }

    }
}