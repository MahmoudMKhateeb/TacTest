using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class GetTrucksTypeForEditOutput
    {
		public CreateOrEditTrucksTypeDto TrucksType { get; set; }


    }
}