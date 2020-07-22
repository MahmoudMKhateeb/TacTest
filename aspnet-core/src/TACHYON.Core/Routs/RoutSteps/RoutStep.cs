using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Cities;
using TACHYON.Cities;
using TACHYON.Routs;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.Routs.RoutSteps
{
    [Table("RoutSteps")]
    [Audited]
    public class RoutStep : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [StringLength(RoutStepConsts.MaxDisplayNameLength, MinimumLength = RoutStepConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [StringLength(RoutStepConsts.MaxLatitudeLength, MinimumLength = RoutStepConsts.MinLatitudeLength)]
        public virtual string Latitude { get; set; }

        [StringLength(RoutStepConsts.MaxLongitudeLength, MinimumLength = RoutStepConsts.MinLongitudeLength)]
        public virtual string Longitude { get; set; }

        [Range(RoutStepConsts.MinOrderValue, RoutStepConsts.MaxOrderValue)]
        public virtual int Order { get; set; }


        public virtual int? OriginCityId { get; set; }

        [ForeignKey("OriginCityId")]
        public City OriginCityFk { get; set; }

        public virtual int? DestinationCityId { get; set; }

        [ForeignKey("DestinationCityId")]
        public City DestinationCityFk { get; set; }

        public virtual int? RouteId { get; set; }

        [ForeignKey("RouteId")]
        public Route RouteFk { get; set; }

        public virtual long? ShippingRequestId { get; set; }

        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }


    }
}