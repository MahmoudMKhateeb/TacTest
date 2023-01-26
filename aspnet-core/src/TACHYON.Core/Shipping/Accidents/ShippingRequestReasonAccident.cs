using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.Common;

namespace TACHYON.Shipping.Accidents
{
    [Table("ShippingRequestReasonAccidents")]
    public class ShippingRequestReasonAccident : FullAuditedEntity,
        IMultiLingualEntity<ShippingRequestReasonAccidentTranslation>, IHasKey
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Key { get; set; }

        /// <summary>
        /// if <para>IsTripImpactEnabled</para> enabled
        /// this will take an effect for trip `Can not continue trip`
        /// and the active point will stuck on `RoutPointStatus.Issue`
        /// </summary>
        public bool IsTripImpactEnabled { get; set; }
        public ICollection<ShippingRequestReasonAccidentTranslation> Translations { get; set; }
    }
}