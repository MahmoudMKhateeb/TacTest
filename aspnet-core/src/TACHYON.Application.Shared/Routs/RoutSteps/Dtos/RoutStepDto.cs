
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class RoutStepDto : EntityDto<long>
    {
        [StringLength(RoutStepConsts.MaxDisplayNameLength, MinimumLength = RoutStepConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }
        [Required]
        public int Order { get; set; }

        [Required]
        public  long ShippingRequestId { get; set; }

        [Required]
        public long AssignedDriverUserId { get; set; }

        [Required]
        public Guid AssignedTruckId { get; set; }

        public long? AssignedTrailerId { get; set; }

        public long? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }

        [Required]
        public long SourceRoutPointId { get; set; }

        [Required]
        public long DestinationRoutPointId { get; set; }

        public double TotalAmount { get; set; }

        public double ExistingAmount { get; set; }

        public double RemainingAmount { get; set; }
    }
}