using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos
{
    public class GetTransportTypesTranslationForEditOutput
    {
        public CreateOrEditTransportTypesTranslationDto TransportTypesTranslation { get; set; }

        public string TransportTypeDisplayName { get; set; }
    }
}