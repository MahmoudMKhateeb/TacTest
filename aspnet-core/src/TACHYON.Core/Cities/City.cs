using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities.CitiesTranslations;
using TACHYON.Countries;

namespace TACHYON.Cities
{
    [Table("Cities")]
    [Audited]
    public class City : FullAuditedEntity, IMultiLingualEntity<CitiesTranslation>
    {

        [Required]
        [StringLength(CityConsts.MaxDisplayNameLength, MinimumLength = CityConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(CityConsts.MaxCodeLength, MinimumLength = CityConsts.MinCodeLength)]
        public virtual string Code { get; set; }

        public string Polygon { get; set; }
        public Point Location { get; set; }
        public virtual int CountyId { get; set; }

        [ForeignKey("CountyId")]
        public County CountyFk { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<CitiesTranslation> Translations { get; set; }


    }
}