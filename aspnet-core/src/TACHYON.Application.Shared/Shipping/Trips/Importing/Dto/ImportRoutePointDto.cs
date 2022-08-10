using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Shipping.Trips.Importing.Dto
{
    public class ImportRoutePointDto : Entity<long?>, ICreateOrEditRoutPointDtoBase
    {
        public PickingType PickingType { get; set; }
        public string PickingTypeDisplayName { get; set; }
        /// <summary>
        /// This field is angular helper to view facility name
        /// </summary>
        public string Facility { get; set; }
        public long FacilityId { get; set; }
        /// <summary>
        /// This field is angular helper to view receiver name
        /// </summary>
        public string Receiver { get; set; }
        public int? ReceiverId { get; set; }
        public string Code { set; get; } = (new Random().Next(100000, 999999)).ToString();
        public string Exception { get; set; }
        /// <summary>
        /// This is helper field for backend
        /// </summary>
        public int ShippingRequestTripId { get; set; }
        /// <summary>
        /// This is helper field to assign workflow version for points
        /// </summary>
        public bool TripNeedsDeliveryNote { get; set; }
        public string BulkUploadReference { get; set; }
        public string TripReference { get; set; }
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }
    }
}
