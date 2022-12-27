using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Actors.Dtos
{
    public class GetAllActorsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string CompanyNameFilter { get; set; }

        public int? ActorTypeFilter { get; set; }

        public string MoiNumberFilter { get; set; }

        public string AddressFilter { get; set; }

        public string MobileNumberFilter { get; set; }

        public string EmailFilter { get; set; }

    }
}