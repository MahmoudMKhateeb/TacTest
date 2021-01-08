
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Goods.GoodsDetails.Dtos;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class CreateOrEditRoutStepDto : EntityDto<long?>
    {

        [StringLength(RoutStepConsts.MaxDisplayNameLength, MinimumLength = RoutStepConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        [StringLength(RoutStepConsts.MaxLatitudeLength, MinimumLength = RoutStepConsts.MinLatitudeLength)]
        public string Latitude { get; set; }


        [StringLength(RoutStepConsts.MaxLongitudeLength, MinimumLength = RoutStepConsts.MinLongitudeLength)]
        public string Longitude { get; set; }


        [Range(RoutStepConsts.MinOrderValue, RoutStepConsts.MaxOrderValue)]
        public int Order { get; set; }

        public int? OriginCityId { get; set; }

        public int? DestinationCityId { get; set; }

        public int? ShippingRequestId { get; set; }

        public long? SourceFacilityId { get; set; }

        public long? DestinationFacilityId { get; set; }

        public long? GoodsDetailId { get; set; }
        public long? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }
        public int? SourcePickingTypeId { get; set; }

        public int? DestinationPickingTypeId { get; set; }
        public long GoodsUnitOfMeaureId { get; set; }
        public double GoodsWeight { get; set; }
        public double TotalAmount { get; set; }
        public double ExistingAmount { get; set; }
        public double DroppedAmount { get; set; }
        public double RemainingAmount { get; set; }
        public int RoutStepOrder { get; set; }
        public CreateOrEditGoodsDetailDto CreateOrEditGoodsDetailDto { get; set; }






    }
}