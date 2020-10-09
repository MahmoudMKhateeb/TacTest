using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TruckSubtypes.Dtos
{
    public class GetTruckSubtypeForEditOutput
    {
		public CreateOrEditTruckSubtypeDto TruckSubtype { get; set; }

		public string TrucksTypeDisplayName { get; set;}


    }
}