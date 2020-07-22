using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Trailers.TrailerTypes
{
    [Table("TrailerTypes")]
    [Audited]
    public class TrailerType : FullAuditedEntity
    {

        [Required]
        [StringLength(TrailerTypeConsts.MaxDisplayNameLength, MinimumLength = TrailerTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }


    }
}