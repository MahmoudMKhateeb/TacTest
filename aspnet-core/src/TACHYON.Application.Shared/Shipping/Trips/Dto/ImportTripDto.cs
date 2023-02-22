using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Shipping.Trips.Dto
{
    public class ImportTripDto : ICreateOrEditTripDtoBase
    {
        public int Id { get; set; }
        public string BulkUploadRef { get; set; }

        /// <summary>
        /// Can be set when reading data from excel or when importing trip
        /// </summary>
        public string Exception { get; set; }

        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }

        public long? ShippingRequestId { get; set; }
        public DateTime? StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public bool HasAttachment { get; set; }
        public bool NeedsDeliveryNote { get; set; }
        public string Note { get; set; }
        public string TotalValue { get; set; }
        /// <summary>
        /// This field is for single drop pickup point facility
        /// </summary>
        public string OriginalFacility { get; set; }
        public long? OriginFacilityId { get; set; }
        /// <summary>
        /// This field is for single drop, drop point facility
        /// </summary>
        public string DestinationFacility { get; set; }
        public long? DestinationFacilityId { get; set; }
        /// <summary>
        /// This field is for single drop pickup point sender
        /// </summary>
        public string sender { get; set; }
        public int? SenderId { get; set; }
        /// <summary>
        /// This field is for single drop, drop point receiver
        /// </summary>
        public string Receiver { get; set; }
        public int? ReceiverId { get; set; }
        #region dedicated
        public string RouteTypeTitle { get; set; }
        public ShippingRequestRouteType RouteType { get; set; }
        public int NumberOfDrops { get; set; }
        public string Driver { get; set; }
        public string Truck { get; set; }

        public long? TruckId { set; get; }
        public long? DriverUserId { set; get; }
        #endregion
    }
}
