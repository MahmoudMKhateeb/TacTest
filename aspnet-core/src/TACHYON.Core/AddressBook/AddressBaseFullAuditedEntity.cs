using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Abp.Domain.Entities.Auditing;
using NetTopologySuite.Geometries;
using TACHYON.Cities;
using TACHYON.Countries;

namespace TACHYON.AddressBook
{
    public class AddressBaseFullAuditedEntity : FullAuditedEntity<long>
    {
        [Required]
        [StringLength(FacilityConsts.MaxNameLength, MinimumLength = FacilityConsts.MinNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(FacilityConsts.MaxAdressLength, MinimumLength = FacilityConsts.MinAdressLength)]
        public virtual string Adress { get; set; }

        public virtual int CityId { get; set; }

        [ForeignKey("CityId")]
        public City CityFk { get; set; }

        public Point Location { get; set; }
    }
}
