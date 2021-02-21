using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Ports;
using TACHYON.Cities;
using TACHYON.Routs.RoutTypes;

namespace TACHYON.Routs
{
    [Table("Routes")]
    [Audited]
    public class Route : FullAuditedEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [StringLength(RouteConsts.MaxDisplayNameLength, MinimumLength = RouteConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(RouteConsts.MaxDescriptionLength, MinimumLength = RouteConsts.MinDescriptionLength)]
        public virtual string Description { get; set; }


        public virtual int? RoutTypeId { get; set; }

        [ForeignKey("RoutTypeId")]
        public RoutType RoutTypeFk { get; set; }

        //Facility
        public virtual long? OriginFacilityId { get; set; }

        [ForeignKey("OriginFacilityId")]
        public Facility OriginFacilityFk { get; set; }

        public virtual long? DestinationFacilityId { get; set; }

        [ForeignKey("DestinationFacilityId")]
        public Facility DestinationFacilityFk { get; set; }

        //city
        public virtual int OriginCityId { get; set; }

        [ForeignKey("OriginCityId")]
        public City OriginCityFk { get; set; }

        public virtual int DestinationCityId { get; set; }

        [ForeignKey("DestinationCityId")]
        public City DestinationCityFk { get; set; }
        //port
        public virtual long? OriginPortId { get; set; }

        [ForeignKey("OriginPortId")]
        public Port OriginPortFk { get; set; }

        public virtual long? DestinationPortId { get; set; }

        [ForeignKey("DestinationPortId")]
        public Port DestinationPortFk { get; set; }

    }
}