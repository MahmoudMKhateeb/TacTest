using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using TACHYON.Routs.RoutPoints;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.Tracking.Dto
{
    public class TrackingRoutePointDto : EntityDto<long>
    {
        public int ShippingRequestTripId { get; set; }
        public PickingType PickingType { get; set; }
        public RoutePointStatus Status { get; set; }
        public string ReceiverFullName { get; set; }
        public string Address { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool IsResolve { get; set; }
        public bool CanGoToNextLocation { get; set; }
        public bool IsDeliveryNoteUploaded { get; set; }
        public bool IsPodUploaded { get; set; }
        public bool IsGoodPictureUploaded { get; set; }
        public decimal FacilityRate { get; set; }
        public long? WaybillNumber { get; set; }
        public List<RoutPointTransactionDto> Statues { get; set; }
        public List<PointTransactionDto> AvailableTransactions { get; set; }
    }
}