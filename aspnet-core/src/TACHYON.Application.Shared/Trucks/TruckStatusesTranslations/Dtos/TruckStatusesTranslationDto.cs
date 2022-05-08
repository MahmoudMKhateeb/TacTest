using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trucks.TruckStatusesTranslations.Dtos
{
    public class TruckStatusesTranslationDto : EntityDto
    {
        public string TranslatedDisplayName { get; set; }

        public string Language { get; set; }

        public long CoreId { get; set; }
    }
}