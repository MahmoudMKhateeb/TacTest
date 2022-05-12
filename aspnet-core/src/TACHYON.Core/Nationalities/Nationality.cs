using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Nationalities
{
    [Table("Nationalities")]
    public class Nationality : FullAuditedEntity
    {
        [Required]
        [StringLength(NationalityConsts.MaxNameLength, MinimumLength = NationalityConsts.MinNameLength)]
        public virtual string Name { get; set; }
    }
}