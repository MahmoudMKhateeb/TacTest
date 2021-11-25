using Abp.Application.Services.Dto;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Tracking.Dto;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.Shipping.Drivers.Dto
{
    public class ShippingRequestTripDriverRoutePointDto : EntityDto<long>
    {

        public int ShippingRequestTripId { get; set; }
        public PickingType PickingType { get; set; }

        public RoutePointStatus Status { get; set; }
        public RoutePointCompletedStatus CompletedStatus { get; set; }
        public string ReceiverFullName { get; set; }
        public long FacilityId { get; set; }
        public string Facility { get; set; }
        public decimal FacilityRating { get; set; }
        public int FacilityRatingNumber { get; set; }
        public bool IsFacilityRated { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public int WorkFlowVersion { get; set; }

        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool IsResolve { get; set; }
        public bool CanGoToNextLocation { get; set; }
        public bool IsDeliveryNoteUploaded { get; set; }
        public List<GoodsDetailDto> GoodsDetails { get; set; }
        [JsonIgnore]
        public List<RoutPointStatusTransitionDto> RoutPointStatusTransitions { get; set; }
        public List<PointTransactionDto> AvailableTransactions { get; set; }
        public List<RoutPointTransactionDto> Statues { get; set; }
        public bool IsShow
        {
            get
            {

                if (PickingType == PickingType.Pickup)
                {
                    if (Status != RoutePointStatus.FinishLoading) return true;
                }
                else if (PickingType == PickingType.Dropoff)
                {
                    if (Status != RoutePointStatus.StandBy && Status != RoutePointStatus.DeliveryConfirmation) return true;
                }
                return false;

            }
            set { }
        }


    }
}