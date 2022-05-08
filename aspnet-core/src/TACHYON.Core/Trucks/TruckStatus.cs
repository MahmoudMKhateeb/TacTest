using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Trucks.TruckStatusesTranslations;

namespace TACHYON.Trucks
{
    [Table("TruckStatuses")]
    [Audited]
    public class TruckStatus : FullAuditedEntity<long>, IMultiLingualEntity<TruckStatusesTranslation>
    {
        [Required]
        [StringLength(TruckStatusConsts.MaxDisplayNameLength, MinimumLength = TruckStatusConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public ICollection<TruckStatusesTranslation> Translations { get; set; }
    }
}