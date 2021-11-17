using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Routs.RoutPoints;

namespace TACHYON.Routs.Dtos
{
    public class RoutPointStatusTransitionDto : EntityDto
    {
        public long PointId { get; set; }
        public RoutePointStatus Status { get; set; }
        public DateTime CreationTime { get; set; }

    }
}