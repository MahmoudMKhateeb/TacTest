using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Trucks.PlateTypes
{
    [Table("PlateTypes")]
    public class PlateType : FullAuditedEntity, IMultiLingualEntity<PlateTypeTranslation>
    {

        [Required]
        [StringLength(PlateTypeConsts.MaxDisplayNameLength, MinimumLength = PlateTypeConsts.MinDisplayNameLength)]
        public virtual string Name { get; set; }
        /// <summary>
        /// This field is for Bayan Integration system
        /// </summary>
        public string BayanIntegrationId { get; set; }

        public bool IsDefault { get; set; }
        public int WaslIntegrationId { get; set; }
        public ICollection<PlateTypeTranslation> Translations { get; set; }
    }
}