using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.Routs.RoutSteps;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class RoutPointDto: EntityDto<long>
    {
        public string DisplayName { get; set; }

        [Required]
        public int CityId { get; set; }
        public int? PickingTypeId { get; set; }

        [Required]
        [StringLength(RoutStepConsts.MaxLatitudeLength, MinimumLength = RoutStepConsts.MinLatitudeLength)]
        public virtual string Latitude { get; set; }

        [Required]
        [StringLength(RoutStepConsts.MaxLatitudeLength, MinimumLength = RoutStepConsts.MinLatitudeLength)]
        public virtual string Longitude { get; set; }

        [Required]
        public long FacilityId { get; set; }

        //to do receiver attribute
    }
}
