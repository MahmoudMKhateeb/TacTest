
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Routs.Dtos
{
    public class CreateOrEditRouteDto : EntityDto<int?>
    {

        [StringLength(RouteConsts.MaxDisplayNameLength, MinimumLength = RouteConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        [StringLength(RouteConsts.MaxDescriptionLength, MinimumLength = RouteConsts.MinDescriptionLength)]
        public string Description { get; set; }


        public int? RoutTypeId { get; set; }

        public virtual int? OriginFacilityId { get; set; }

        public virtual int? DestinationFacilityId { get; set; }

        public virtual long? OriginPortId { get; set; }


        public virtual long? DestinationPortId { get; set; }
    }
}