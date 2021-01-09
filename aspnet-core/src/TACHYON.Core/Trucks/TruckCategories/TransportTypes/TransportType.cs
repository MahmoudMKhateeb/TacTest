using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;

namespace TACHYON.Trucks.TruckCategories.TransportTypes
{
    [Table("TransportTypes")]
    public class TransportType : FullAuditedEntity, IMultiLingualEntity<TransportTypesTranslation>
    {

        [Required]
        [StringLength(TransportTypeConsts.MaxDisplayNameLength, MinimumLength = TransportTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }


        public ICollection<TransportTypesTranslation> Translations { get; set; }
    }
}