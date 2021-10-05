
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.GoodCategories.Dtos
{
    public class CreateOrEditGoodCategoryDto : EntityDto<int?>
    {
        [Required]
        [StringLength(GoodCategoryConsts.MaxDisplayNameLength, MinimumLength = GoodCategoryConsts.MinDisplayNameLength)]
        public string Name { get; set; }

        public int? FatherId { get; set; }

        public ICollection<GoodCategoryTranslationDto> Translations { get; set; }
        public bool IsActive { get; set; } = true;

        public string BayanIntegrationId { get; set; }


    }
}