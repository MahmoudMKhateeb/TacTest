using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using Castle.Core.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class CreateOrEditPackingTypeDto : EntityDto<int?>, ICustomValidate
    {
        [Required]
        [StringLength(PackingTypeConsts.MaxDisplayNameLength,
            MinimumLength = PackingTypeConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        [StringLength(PackingTypeConsts.MaxDescriptionLength,
            MinimumLength = PackingTypeConsts.MinDescriptionLength)]
        public string Description { get; set; }

        public List<PackingTypeTranslationDto> TranslationDtos { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!Id.HasValue && TranslationDtos.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("At Least One Translation Item"));
        }
    }
}