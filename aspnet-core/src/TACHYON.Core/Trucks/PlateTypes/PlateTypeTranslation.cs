using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TACHYON.Trucks.PlateTypes
{
    [Table("PlateTypeTranslations")]
    public class PlateTypeTranslation : Entity, IEntityTranslation<PlateType>
    {
        public PlateType Core { get; set; }
        public int CoreId { get; set; }
        public string Language { get; set; }

        [Required]
        [StringLength(PlateTypeConsts.MaxDisplayNameLength, MinimumLength = PlateTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }
    }
}