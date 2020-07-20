using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

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
        [StringLength(TruckConsts.MaxLicenseNumberLength, MinimumLength = TruckConsts.MinLicenseNumberLength)]
        public virtual string LicenseNumber { get; set; }

        public virtual DateTime LicenseExpirationDate { get; set; }

        public virtual bool IsAttachable { get; set; }

        [StringLength(TruckConsts.MaxNoteLength, MinimumLength = TruckConsts.MinNoteLength)]
        public virtual string Note { get; set; }


        public virtual Guid TrucksTypeId { get; set; }

        [ForeignKey("TrucksTypeId")]
        public TrucksType TrucksTypeFk { get; set; }

        public virtual Guid TruckStatusId { get; set; }

        [ForeignKey("TruckStatusId")]
        public TruckStatus TruckStatusFk { get; set; }

        public virtual long? Driver1UserId { get; set; }

        [ForeignKey("Driver1UserId")]
        public User Driver1UserFk { get; set; }

        public virtual long? Driver2UserId { get; set; }

        [ForeignKey("Driver2UserId")]
        public User Driver2UserFk { get; set; }

        public int? RentPrice { get; set; }

        public int? RentDuration  { get; set; }

        public virtual Guid? PictureId { get; set; }



    }
}