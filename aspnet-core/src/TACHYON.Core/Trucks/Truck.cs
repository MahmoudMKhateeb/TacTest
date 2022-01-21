using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Authorization.Users;
using TACHYON.Documents.DocumentFiles;
//using TACHYON.Authorization.Users;
using TACHYON.Trucks;
using TACHYON.Trucks.PlateTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.Trucks
{
    [Table("Trucks")]
    [Audited]
    public class Truck : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [Required]
        [StringLength(TruckConsts.MaxPlateNumberLength, MinimumLength = TruckConsts.MinPlateNumberLength)]
        [RegularExpression(TruckConsts.PlateNumberRegularExpression)]
        public virtual string PlateNumber { get; set; }

        [StringLength(TruckConsts.MaxModelNameLength, MinimumLength = TruckConsts.MinModelNameLength)]
        public virtual string ModelName { get; set; }

        [StringLength(TruckConsts.MaxModelYearLength, MinimumLength = TruckConsts.MinModelYearLength)]
        [RegularExpression(TruckConsts.ModelYearRegularExpression)]
        public virtual string ModelYear { get; set; }

        public virtual string Capacity { get; set; }

        public virtual bool? IsAttachable { get; set; }

        [StringLength(TruckConsts.MaxNoteLength, MinimumLength = TruckConsts.MinNoteLength)]
        public virtual string Note { get; set; }


        public virtual long? TruckStatusId { get; set; }

        [ForeignKey("TruckStatusId")] public TruckStatus TruckStatusFk { get; set; }

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
        [ForeignKey("TransportTypeId")] public TransportType TransportTypeFk { get; set; }


        public virtual long? TrucksTypeId { get; set; }


        [ForeignKey("TrucksTypeId")] public TrucksType TrucksTypeFk { get; set; }


        public virtual int? CapacityId { get; set; }
        [ForeignKey("CapacityId")] public Capacity CapacityFk { get; set; }

        #endregion

        //Truck length (Meter) 
        public int? Length { get; set; }

        public virtual int? PlateTypeId { get; set; }

        [ForeignKey("PlateTypeId")] public PlateType PlateTypeFk { get; set; }

        public virtual ICollection<DocumentFile> DocumentFiles { get; set; }

        #region Helper

        /// <summary>
        /// Get truck display name used in DDLs 
        /// </summary>
        /// <returns></returns>
        public string GetDisplayName()
        {
            return PlateNumber + (ModelName.IsNullOrEmpty() ? "" : " _ " + ModelName)
                               + (Capacity.IsNullOrEmpty() ? "" : " _ " + Capacity);
        }

        #endregion
    }
}