using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Vases.Dtos
{
    public class CreateOrEditVasDto : EntityDto<int?>,ICustomValidate
    {
        [Required]
        [StringLength(VasConsts.MaxNameLength,
            MinimumLength = VasConsts.MinNameLength)]
        public string Name { get; set; }

        [StringLength(VasConsts.MaxDisplayNameLength, MinimumLength = VasConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }

        public bool HasAmount { get; set; }

        public bool HasCount { get; set; }

        public List<VasTranslationDto> TranslationDtos { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            // Create Validation
            if (!Id.HasValue && TranslationDtos.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("At Least one Vas Translation Item"));
        }
    }
}