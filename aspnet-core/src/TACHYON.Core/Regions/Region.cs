using TACHYON.Countries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Regions
{
    [Table("Regions")]
    public class Region : FullAuditedEntity
    {

        [Required]
        public virtual string Name { get; set; }

        public virtual int BayanIntegrationId { get; set; }

        public virtual int? CountyId { get; set; }

        [ForeignKey("CountyId")]
        public County CountyFk { get; set; }

    }
}