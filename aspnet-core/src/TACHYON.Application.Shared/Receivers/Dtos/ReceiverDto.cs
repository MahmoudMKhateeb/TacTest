using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Receivers.Dtos
{
    public class ReceiverDto : EntityDto
    {
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public long FacilityId { get; set; }

    }
}