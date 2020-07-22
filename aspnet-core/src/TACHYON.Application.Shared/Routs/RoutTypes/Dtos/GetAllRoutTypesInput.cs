using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Routs.RoutTypes.Dtos
{
    public class GetAllRoutTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }



    }
}