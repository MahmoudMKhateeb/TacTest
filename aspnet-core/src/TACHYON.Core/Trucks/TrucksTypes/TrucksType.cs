using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Trucks.TruckCategories.TransportSubtypes;

namespace TACHYON.Trucks.TrucksTypes
{
    [Table("TrucksTypes")]
    [Audited]
    public class TrucksType : FullAuditedEntity<long>
    {


        [Required]
        [StringLength(TrucksTypeConsts.MaxDisplayNameLength, MinimumLength = TrucksTypeConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public int? TransportSubtypeId { get; set; }
        
        [ForeignKey("TransportSubtypeId")]
        public TransportSubtype TransportSubtypeFk { get; set; }


    }
}