using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trucks.TruckStatusesTranslations.Dtos
{
    public class GetAllTruckStatusesTranslationsInput 
    {

        public string LoadOptions { get; set; }

        public long CoreId { get; set; }

    }
}