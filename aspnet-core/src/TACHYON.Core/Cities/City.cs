using TACHYON.Countries;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Cities
{
	[Table("Cities")]
    [Audited]
    public class City : FullAuditedEntity 
    {

		[Required]
		[StringLength(CityConsts.MaxDisplayNameLength, MinimumLength = CityConsts.MinDisplayNameLength)]
		public virtual string DisplayName { get; set; }
		
		[StringLength(CityConsts.MaxCodeLength, MinimumLength = CityConsts.MinCodeLength)]
		public virtual string Code { get; set; }
		
		[StringLength(CityConsts.MaxLatitudeLength, MinimumLength = CityConsts.MinLatitudeLength)]
		public virtual string Latitude { get; set; }
		
		[StringLength(CityConsts.MaxLongitudeLength, MinimumLength = CityConsts.MinLongitudeLength)]
		public virtual string Longitude { get; set; }
		

		public virtual int CountyId { get; set; }
		
        [ForeignKey("CountyId")]
		public County CountyFk { get; set; }
		
    }
}