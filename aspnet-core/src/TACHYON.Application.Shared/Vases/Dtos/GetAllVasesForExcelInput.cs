using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Vases.Dtos
{
    public class GetAllVasesForExcelInput
    {
        public string Filter { get; set; }

        public int? HasAmountFilter { get; set; }

        public int? HasCountFilter { get; set; }

    }
}