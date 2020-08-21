using TACHYON.Countries;
using TACHYON.Cities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.AddressBook
{
	[Table("Facilities")]
    public class Facility : FullAuditedEntity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		[Required]
		[StringLength(FacilityConsts.MaxNameLength, MinimumLength = FacilityConsts.MinNameLength)]
		public virtual string Name { get; set; }
		
		[Required]
		[StringLength(FacilityConsts.MaxAdressLength, MinimumLength = FacilityConsts.MinAdressLength)]
		public virtual string Adress { get; set; }
		
		public virtual decimal Longitude { get; set; }
		
		public virtual decimal Latitude { get; set; }
		

		public virtual int CountyId { get; set; }
		
        [ForeignKey("CountyId")]
		public County CountyFk { get; set; }
		
		public virtual int CityId { get; set; }
		
        [ForeignKey("CityId")]
		public City CityFk { get; set; }
		
    }
}