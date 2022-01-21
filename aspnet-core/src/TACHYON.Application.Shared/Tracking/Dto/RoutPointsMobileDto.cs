using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Trips;
using TACHYON.Tracking.Dto.WorkFlow;

namespace TACHYON.Tracking
{
    public class RoutPointsMobileDto : EntityDto<long>
    {
        public int ShippingRequestTripId { get; set; }
        public RoutePointStatus Status { get; set; }
        public PickingType PickingType { get; set; }
        public string StatusTitle { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool IsResolve { get; set; }
        public bool IsPodUploaded { get; set; }
        public bool CanGoToNextLocation { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public List<PointTransactionDto> AvailableTransactions { get; set; }
    }
}