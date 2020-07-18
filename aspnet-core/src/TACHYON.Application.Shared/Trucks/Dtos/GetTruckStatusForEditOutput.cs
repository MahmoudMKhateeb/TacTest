using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.Dtos
{
    public class GetTruckStatusForEditOutput
    {
		public CreateOrEditTruckStatusDto TruckStatus { get; set; }


    }
}