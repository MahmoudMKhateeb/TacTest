using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Routs.RoutPoints.Dtos;

namespace TACHYON.Routs.RoutSteps.Dtos
{
    public class GetRoutStepForEditOutput
    {
        public CreateOrEditRoutStepDto RoutStep { get; set; }

        //public string CityDisplayName { get; set; }

        //public string CityDisplayName2 { get; set; }

        public string RouteDisplayName { get; set; }


        public string TrucksTypeDisplayName { get; set; }

        public string TrailerTypeDisplayName { get; set; }

        //public string GoodsDetailName { get; set; }
        public GetRoutPointForViewDto SourceRoutPointDto { get; set; }
        public GetRoutPointForViewDto DestinationRoutPointDto { get; set; }


    }
}