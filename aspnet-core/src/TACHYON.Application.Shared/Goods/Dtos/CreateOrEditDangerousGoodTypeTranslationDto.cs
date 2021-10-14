using Abp.Application.Services.Dto;
using Abp.Extensions;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Goods.Dtos
{
    public class CreateOrEditDangerousGoodTypeTranslationDto : EntityDto<int?>
    {
        [Required]
        [StringLength(DangerousGoodTypeConsts.MaxNameLength,
            MinimumLength = DangerousGoodTypeConsts.MinNameLength)]
        public virtual string TranslatedName { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 2)]
        public string Language { get; set; }

        [Required]
        public int CoreId { get; set; }

    }
}