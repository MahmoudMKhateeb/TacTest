using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TransportSubtypes.Dtos
{
    public class GetTransportSubtypeForEditOutput
    {
		public CreateOrEditTransportSubtypeDto TransportSubtype { get; set; }

		public string TransportTypeDisplayName { get; set;}


    }
}