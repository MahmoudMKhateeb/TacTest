using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ImportTripDto : ICreateOrEditTripDtoBase
    {
        [Required]
        public string BulkUploadReference { get; set; }

        [Required]

        /// <summary>
        /// Can be set when reading data from excel or when importing truck
        /// </summary>
        public string Exception { get; set; }

        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }

        public long ShippingRequestId { get; set; }
        public DateTime? StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }
        public string Note { get; set; }
        public string TotalValue { get; set; }
    }
}
