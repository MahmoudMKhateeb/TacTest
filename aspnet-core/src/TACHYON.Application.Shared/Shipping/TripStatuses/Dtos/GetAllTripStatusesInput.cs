using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.TripStatuses.Dtos
{
    public class GetAllTripStatusesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}