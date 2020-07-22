
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

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

        public int RouteId { get; set; }


    }
}