using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos
{
    public class GetCapacityForEditOutput
    {
        public CreateOrEditCapacityDto Capacity { get; set; }

        public string TruckTypeDisplayName { get; set; }
    }
}