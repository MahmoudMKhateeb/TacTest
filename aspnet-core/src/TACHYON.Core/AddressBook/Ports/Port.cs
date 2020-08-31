using TACHYON.Cities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.AddressBook.Ports
{
	[Table("Ports")]
    public class Port : FullAuditedEntity<long> 
    {

		[Required]
		[StringLength(PortConsts.MaxNameLength, MinimumLength = PortConsts.MinNameLength)]
		public virtual string Name { get; set; }
		
		[StringLength(PortConsts.MaxAdressLength, MinimumLength = PortConsts.MinAdressLength)]
		public virtual string Adress { get; set; }
		
		public virtual decimal Longitude { get; set; }
		
		public virtual decimal Latitude { get; set; }
		

		public virtual int CityId { get; set; }
		
        [ForeignKey("CityId")]
		public City CityFk { get; set; }
		
    }
}