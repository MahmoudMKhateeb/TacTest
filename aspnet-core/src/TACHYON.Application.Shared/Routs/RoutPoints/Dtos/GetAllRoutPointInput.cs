using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Routs.RoutPoints.Dtos
{
    public class GetAllRoutPointInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public PickingType? PickingType { get; set; }
    }
}
