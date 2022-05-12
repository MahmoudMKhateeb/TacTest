using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Receivers.Dtos
{
    public class CreateOrEditReceiverDto : EntityDto<int?>
    {
        [Required]
        [StringLength(ReceiverConsts.MaxFullNameLength, MinimumLength = ReceiverConsts.MinFullNameLength)]
        public string FullName { get; set; }

        [StringLength(ReceiverConsts.MaxEmailLength, MinimumLength = ReceiverConsts.MinEmailLength)]
        public string Email { get; set; }

        [Required]
        [StringLength(ReceiverConsts.MaxPhoneNumberLength, MinimumLength = ReceiverConsts.MinPhoneNumberLength)]
        public string PhoneNumber { get; set; }

        public long FacilityId { get; set; }
    }
}