using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System.Collections.Generic;

namespace TACHYON.Trucks.PlateTypes
{
    [Table("PlateTypes")]
    public class PlateType : FullAuditedEntity, IMultiLingualEntity<PlateTypeTranslation>
    {

        //[Required]
        //[StringLength(PlateTypeConsts.MaxDisplayNameLength, MinimumLength = PlateTypeConsts.MinDisplayNameLength)]
        //public virtual string DisplayName { get; set; }
        public ICollection<PlateTypeTranslation> Translations { get; set; }
    }
}