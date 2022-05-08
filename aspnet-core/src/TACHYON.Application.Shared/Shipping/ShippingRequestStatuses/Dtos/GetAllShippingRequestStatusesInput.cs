using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingRequestStatuses.Dtos
{
    public class GetAllShippingRequestStatusesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }
    }
}