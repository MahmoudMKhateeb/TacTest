using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ShippingRequestTripFilterInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public long RequestId { get; set; }
        public ShippingRequestTripStatus? Status { get; set; }

        public void Normalize()
        {
            if (Sorting.Contains("TripReferenceID"))
            {
                Sorting=Sorting.Replace("TripReferenceID","BulkUploadRef");
            }
            else if (Sorting.Contains("notesCount"))
                Sorting = "";
        }
    }
}