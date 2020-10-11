using TACHYON.Trucks.TruckCategories.TransportTypes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Trucks.TruckCategories.TransportSubtypes
{
	[Table("TransportSubtypes")]
    public class TransportSubtype : FullAuditedEntity 
    {

		[Required]
		[StringLength(TransportSubtypeConsts.MaxDisplayNameLength, MinimumLength = TransportSubtypeConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		

		public virtual int? TransportTypeId { get; set; }
		
        [ForeignKey("TransportTypeId")]
		public TransportType TransportTypeFk { get; set; }
		
    }
}