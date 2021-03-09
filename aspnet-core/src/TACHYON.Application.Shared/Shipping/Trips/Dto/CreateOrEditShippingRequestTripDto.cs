using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.ShippingRequestTripVases.Dtos;

namespace TACHYON.Shipping.Trips.Dto
{
    public class CreateOrEditShippingRequestTripDto : FullAuditedEntityDto<long?>
    {
        public DateTime StartTripDate { get; set; }
        public DateTime EndTripDate { get; set; }
        public DateTime? StartWorking { get; set; }
        public DateTime? EndWorking { get; set; }
        public byte StatusId { get; set; }
        //public ShippingRequestStatus Status { get; set; }

        public long? AssignedDriverUserId { get; set; }
        public long? AssignedTruckId { get; set; }
        public long ShippingRequestId { get; set; }

        public string Code { get; set; }

        //Facility
        public virtual long? OriginFacilityId { get; set; }

        public virtual long? DestinationFacilityId { get; set; }

        public List<CreateOrEditRoutPointDto> RoutPoints { get; set; }
        public List<CreateOrEditShippingRequestTripVasDto> ShippingRequestTripVases { get; set; }
    }
}
