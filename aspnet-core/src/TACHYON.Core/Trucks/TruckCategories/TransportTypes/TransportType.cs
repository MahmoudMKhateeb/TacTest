using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Common;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;

namespace TACHYON.Trucks.TruckCategories.TransportTypes
{
    [Table("TransportTypes")]
    public class TransportType : FullAuditedEntity, IMultiLingualEntity<TransportTypesTranslation>, IHasKey
    {

        public virtual string DisplayName { get; set; }

        [Required]
        [StringLength(TransportTypeConsts.MaxDisplayNameLength, MinimumLength = TransportTypeConsts.MinDisplayNameLength)]
        public virtual string Key { get; set; }

        public ICollection<TransportTypesTranslation> Translations { get; set; }
    }
}