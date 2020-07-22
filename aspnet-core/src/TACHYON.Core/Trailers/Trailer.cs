using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.Trailers.PayloadMaxWeights;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trucks;

namespace TACHYON.Trailers
{
    [Table("Trailers")]
    [Audited]
    public class Trailer : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }


        [Required]
        [StringLength(TrailerConsts.MaxTrailerCodeLength, MinimumLength = TrailerConsts.MinTrailerCodeLength)]
        public virtual string TrailerCode { get; set; }

        [Required]
        [StringLength(TrailerConsts.MaxPlateNumberLength, MinimumLength = TrailerConsts.MinPlateNumberLength)]
        public virtual string PlateNumber { get; set; }

        [Required]
        [StringLength(TrailerConsts.MaxModelLength, MinimumLength = TrailerConsts.MinModelLength)]
        public virtual string Model { get; set; }

        [Required]
        [StringLength(TrailerConsts.MaxYearLength, MinimumLength = TrailerConsts.MinYearLength)]
        public virtual string Year { get; set; }

        public virtual int Width { get; set; }

        public virtual int Height { get; set; }

        public virtual int Length { get; set; }

        public virtual bool IsLiftgate { get; set; }

        public virtual bool IsReefer { get; set; }

        public virtual bool IsVented { get; set; }

        public virtual bool IsRollDoor { get; set; }


        public virtual int TrailerStatusId { get; set; }

        [ForeignKey("TrailerStatusId")]
        public TrailerStatus TrailerStatusFk { get; set; }

        public virtual int TrailerTypeId { get; set; }

        [ForeignKey("TrailerTypeId")]
        public TrailerType TrailerTypeFk { get; set; }

        public virtual int PayloadMaxWeightId { get; set; }

        [ForeignKey("PayloadMaxWeightId")]
        public PayloadMaxWeight PayloadMaxWeightFk { get; set; }

        public virtual Guid? HookedTruckId { get; set; }

        [ForeignKey("HookedTruckId")]
        public Truck HookedTruckFk { get; set; }

    }
}