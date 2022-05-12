using Abp.Application.Services.Dto;
using System;

namespace TACHYON.ShippingRequestVases.Dtos
{
    public class GetAllShippingRequestVasesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string VasNameFilter { get; set; }
    }
}