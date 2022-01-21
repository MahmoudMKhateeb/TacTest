using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class RoutStepDto : EntityDto<long>
    {
        public string DisplayName { get; set; }
        public int Order { get; set; }

        public long ShippingRequestId { get; set; }

        public long AssignedDriverUserId { get; set; }

        public long AssignedTruckId { get; set; }

        public long? AssignedTrailerId { get; set; }

        public long? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }

        public long SourceRoutPointId { get; set; }

        public long DestinationRoutPointId { get; set; }

        public double TotalAmount { get; set; }

        public double ExistingAmount { get; set; }

        public double RemainingAmount { get; set; }
    }
}