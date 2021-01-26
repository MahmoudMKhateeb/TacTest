
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class CreateOrEditRoutStepDto : EntityDto<long?>
    {
        [StringLength(RoutStepConsts.MaxDisplayNameLength, MinimumLength = RoutStepConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        [Range(RoutStepConsts.MinOrderValue, RoutStepConsts.MaxOrderValue)]
        public int Order { get; set; }

        public int? ShippingRequestId { get; set; }

        [Required]
        public long AssignedDriverUserId { get; set; }

        [Required]
        public long AssignedTruckId { get; set; }

        public long? AssignedTrailerId { get; set; }
        public long? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }

        //[Required]
        //public long SourceRoutPointId { get; set; }

        //[Required]
        //public long DestinationRoutPointId { get; set; }
        public double TotalAmount { get; set; }
        public double ExistingAmount { get; set; }
        public double RemainingAmount { get; set; }
        //public CreateOrEditGoodsDetailDto CreateOrEditGoodsDetailDto { get; set; }
        public CreateOrEditRoutPointInput CreateOrEditSourceRoutPointInputDto { get; set; }
        public CreateOrEditRoutPointInput CreateOrEditDestinationRoutPointInputDto { get; set; }

    }
}