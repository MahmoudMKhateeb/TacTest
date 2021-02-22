using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Shipping.ShippingTypes.Dtos
{
    public class GetAllShippingTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }

        public string DescriptionFilter { get; set; }

    }
}