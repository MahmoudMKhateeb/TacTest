using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Goods
{
    [Table("DangerousGoodTypes")]
    public class DangerousGoodType : FullAuditedEntity, IMultiLingualEntity<DangerousGoodTypeTranslation>
    {

        [Required]
        [StringLength(DangerousGoodTypeConsts.MaxNameLength, MinimumLength = DangerousGoodTypeConsts.MinNameLength)]
        public virtual string Name { get; set; }
        /// <summary>
        /// for Bayan integration mapping Dangerous Good Types
        /// </summary>
		public virtual int? BayanIntegrationId { get; set; }

        public ICollection<DangerousGoodTypeTranslation> Translations { get; set; }
    }
}