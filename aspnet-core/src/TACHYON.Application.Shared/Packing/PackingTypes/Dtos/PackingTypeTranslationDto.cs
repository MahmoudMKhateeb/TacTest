using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class PackingTypeTranslationDto : EntityDto
    {
        [Required]
        [StringLength(PackingTypeConsts.MaxDisplayNameLength, MinimumLength = PackingTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(PackingTypeConsts.MaxDescriptionLength, MinimumLength = PackingTypeConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }

        [Required] public string Language { get; set; }

        public int CoreId { get; set; }
    }
}