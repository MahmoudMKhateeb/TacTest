using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Vases.Dtos
{
    public class GetAllVasesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public int? HasAmountFilter { get; set; }

        public int? HasCountFilter { get; set; }

    }
}