using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Receivers.Dtos
{
    public class GetAllReceiversInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string FullNameFilter { get; set; }

        public string EmailFilter { get; set; }

        public string PhoneNumberFilter { get; set; }

        public string FacilityNameFilter { get; set; }
        public long? FacilityId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }
}