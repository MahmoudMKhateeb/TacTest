using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.Dtos
{
    public class GetTransportTypeForEditOutput
    {
        public CreateOrEditTransportTypeDto TransportType { get; set; }
    }
}