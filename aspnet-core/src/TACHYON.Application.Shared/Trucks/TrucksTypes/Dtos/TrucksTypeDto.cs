
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Trucks.TrucksTypes.Dtos
{
    public class TrucksTypeDto : EntityDto<long>
    {
        public string DisplayName { get; set; }

        public int? TransportTypeId { get; set; }

        public string TranslatedDisplayName { get; set; }
    }
}