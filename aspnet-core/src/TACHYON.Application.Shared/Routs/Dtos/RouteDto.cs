
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.Dtos
{
    public class RouteDto : EntityDto
    {
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int? RoutTypeId { get; set; }
        public virtual int OriginCityId { get; set; }
        public virtual int DestinationCityId { get; set; }
    }
}