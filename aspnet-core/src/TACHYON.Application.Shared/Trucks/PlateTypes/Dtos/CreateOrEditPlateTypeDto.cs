using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trucks.PlateTypes.Dtos
{
    public class CreateOrEditPlateTypeDto : EntityDto<int?>
    {

        [Required]
        [StringLength(PlateTypeConsts.MaxDisplayNameLength, MinimumLength = PlateTypeConsts.MinDisplayNameLength)]
        public virtual string Name { get; set; }
        public string BayanIntegrationId { get; set; }

        public ICollection<PlateTypeTranslationDto> Translations { get; set; }
    }
}