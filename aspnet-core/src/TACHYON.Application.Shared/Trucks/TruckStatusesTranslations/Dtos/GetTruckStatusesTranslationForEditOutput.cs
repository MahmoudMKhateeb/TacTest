using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.TruckStatusesTranslations.Dtos
{
    public class GetTruckStatusesTranslationForEditOutput
    {
        public CreateOrEditTruckStatusesTranslationDto TruckStatusesTranslation { get; set; }

        public string TruckStatusDisplayName { get; set; }

    }
}