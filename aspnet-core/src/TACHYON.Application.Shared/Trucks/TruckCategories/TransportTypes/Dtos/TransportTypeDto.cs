
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckCategories.TransportTypes.Dtos
{
    public class TransportTypeDto : EntityDto
    {

        public string DisplayName { get; set; } 

      // Mapped from TransportTypesTranslation.DisplayName
        public string TranslatedDisplayName { get; set; }



    }
}