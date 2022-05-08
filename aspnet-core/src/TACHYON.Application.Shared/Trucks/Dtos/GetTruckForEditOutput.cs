using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.Dtos
{
    public class GetTruckForEditOutput
    {
        public CreateOrEditTruckDto Truck { get; set; }

        public string TrucksTypeDisplayName { get; set; }

        public string TruckStatusDisplayName { get; set; }

        //public string UserName { get; set; }

        //public string UserName2 { get; set; }
    }
}