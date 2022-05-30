using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class GetAllNormalPricePackagesInput
    {
        public string LoadOptions { get; set; }
    }

    public class GetAllPricePackagesForRequestInput : PagedAndSortedResultRequestDto
    {
        public int? CarrierId { get; set; }
        public int ShippingRequestId { get; set; }

    }
}