
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class RoutStepDto : EntityDto<long>
    {
        public string DisplayName { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public int Order { get; set; }


        public int? OriginCityId { get; set; }

        public int? DestinationCityId { get; set; }

        public int? SourcePickingTypeId { get; set; }

        public int? DestinationPickingTypeId { get; set; }

        public long? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }

        public long? GoodsDetailId { get; set; }
        public long GoodsUnitOfMeaureId { get; set; }
        public double GoodsWeight { get; set; }
        public double TotalAmount { get; set; }
        public double ExistingAmount { get; set; }
        public double DroppedAmount { get; set; }
        public double RemainingAmount { get; set; }
        public int RoutStepOrder { get; set; }



    }
}