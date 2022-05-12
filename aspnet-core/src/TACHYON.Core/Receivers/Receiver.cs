using TACHYON.AddressBook;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using Abp.Auditing;

namespace TACHYON.Receivers
{
    [Table("Receivers")]
    [Audited]
    public class Receiver : FullAuditedEntity, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(ReceiverConsts.MaxFullNameLength, MinimumLength = ReceiverConsts.MinFullNameLength)]
        public virtual string FullName { get; set; }

        [StringLength(ReceiverConsts.MaxEmailLength, MinimumLength = ReceiverConsts.MinEmailLength)]
        public virtual string Email { get; set; }

        [Required]
        [StringLength(ReceiverConsts.MaxPhoneNumberLength, MinimumLength = ReceiverConsts.MinPhoneNumberLength)]
        public virtual string PhoneNumber { get; set; }

        public virtual long FacilityId { get; set; }

        [ForeignKey("FacilityId")] public Facility FacilityFk { get; set; }
    }
}