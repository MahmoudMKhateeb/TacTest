using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.AddressBook;
using TACHYON.Authorization.Users;
using TACHYON.Cities;
using TACHYON.Goods.GoodsDetails;
using TACHYON.PickingTypes;
using TACHYON.Routs;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Trailers;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.UnitOfMeasures;

namespace TACHYON.Routs.RoutSteps
{
    [Table("RoutSteps")]
    [Audited]
    public class RoutStep : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [StringLength(RoutStepConsts.MaxDisplayNameLength, MinimumLength = RoutStepConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required]
        [Range(RoutStepConsts.MinOrderValue, RoutStepConsts.MaxOrderValue)]
        public virtual int Order { get; set; }

        [Required]
        public virtual long ShippingRequestId { get; set; }

        [ForeignKey("ShippingRequestId")]
        public ShippingRequest ShippingRequestFk { get; set; }


        /// <summary>
        /// assigned Driver
        /// </summary>
        public long? AssignedDriverUserId { get; set; }
        [ForeignKey("AssignedDriverUserId")]
        public User AssignedDriverUserFk { get; set; }
        /// <summary>
        /// assigned Truck
        /// </summary>
        public long? AssignedTruckId { get; set; }
        [ForeignKey("AssignedTruckId")]
        public Truck AssignedTruckFk { get; set; }

       

        [Required]
        public long SourceRoutPointId { get; set; }

        [ForeignKey("SourceRoutPointId")]
        public RoutPoint SourceRoutPointFk { get; set; }

        [Required]
        public long DestinationRoutPointId { get; set; }

        [ForeignKey("DestinationRoutPointId")]
        public RoutPoint DestinationRoutPointFk { get; set; }

        public int TotalAmount { get; set; }
        public int ExistingAmount { get; set; }
        public int RemainingAmount { get; set; }

    }
}