using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Authorization.Users;
//using TACHYON.Authorization.Users;
using TACHYON.Trucks;
using TACHYON.Trucks.TruckCategories.TransportSubtypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckSubtypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Trucks
{
    [Table("Trucks")]
    [Audited]
    public class Truck : FullAuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [Required]
        [StringLength(TruckConsts.MaxPlateNumberLength, MinimumLength = TruckConsts.MinPlateNumberLength)]
        public virtual string PlateNumber { get; set; }

        [Required]
        [StringLength(TruckConsts.MaxModelNameLength, MinimumLength = TruckConsts.MinModelNameLength)]
        public virtual string ModelName { get; set; }

        [Required]
        [StringLength(TruckConsts.MaxModelYearLength, MinimumLength = TruckConsts.MinModelYearLength)]
        public virtual string ModelYear { get; set; }

        [Required]
        public virtual string Capacity { get; set; }

        //[Required]
        //[StringLength(TruckConsts.MaxLicenseNumberLength, MinimumLength = TruckConsts.MinLicenseNumberLength)]
        //public virtual string LicenseNumber { get; set; }

        //public virtual DateTime LicenseExpirationDate { get; set; }

        public virtual bool IsAttachable { get; set; }

        [StringLength(TruckConsts.MaxNoteLength, MinimumLength = TruckConsts.MinNoteLength)]
        public virtual string Note { get; set; }


        public virtual long? TruckStatusId { get; set; }

        [ForeignKey("TruckStatusId")]
        public TruckStatus TruckStatusFk { get; set; }

       // public virtual long? Driver1UserId { get; set; }

       // [ForeignKey("Driver1UserId")]
       // public User Driver1UserFk { get; set; }

        //public virtual long? Driver2UserId { get; set; }

        //[ForeignKey("Driver2UserId")]
        //public User Driver2UserFk { get; set; }

        //public int? RentPrice { get; set; }

        //public int? RentDuration { get; set; }

        public virtual Guid? PictureId { get; set; }



        // todo make sure those are nullable

        #region Truck Categories

        public virtual int? TransportTypeId { get; set; }
        [ForeignKey("TransportTypeId")]
        public TransportType TransportTypeFk { get; set; }


        public virtual int? TransportSubtypeId { get; set; }
        [ForeignKey("TransportSubtypeId")]
        public TransportSubtype TransportSubtypeFk { get; set; }

        public virtual long? TrucksTypeId { get; set; }
        [ForeignKey("TrucksTypeId")]
        public TrucksType TrucksTypeFk { get; set; }


        public virtual int? TruckSubtypeId { get; set; }
        [ForeignKey("TruckSubtypeId")]
        public TruckSubtype TruckSubtypeFk { get; set; }


        public virtual int? CapacityId { get; set; }
        [ForeignKey("CapacityId")]
        public Capacity CapacityFk { get; set; }

        #endregion



    }
}