using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Routs.Dtos
{
    public class GetRouteForEditOutput
    {
        public CreateOrEditRouteDto Route { get; set; }

        public string RoutTypeDisplayName { get; set; }


    }
}