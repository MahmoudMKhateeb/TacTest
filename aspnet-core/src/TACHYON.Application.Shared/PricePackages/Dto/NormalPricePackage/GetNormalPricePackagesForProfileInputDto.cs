using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.PricePackages.Dto.NormalPricePackage
{
    public class GetNormalPricePackagesForProfileInputDto : PagedAndSortedResultRequestDto
    {
        [Required]
        public int CarrierTenantId { get; set; }
    }
}