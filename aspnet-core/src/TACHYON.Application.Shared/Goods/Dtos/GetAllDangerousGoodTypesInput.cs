using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Goods.Dtos
{
    public class GetAllDangerousGoodTypesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }
    }
}