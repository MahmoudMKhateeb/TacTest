using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Packing.PackingTypes.Dtos
{
    public class GetAllPackingTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

    }
}