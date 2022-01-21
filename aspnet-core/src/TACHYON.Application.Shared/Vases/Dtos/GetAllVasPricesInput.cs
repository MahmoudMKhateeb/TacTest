using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Vases.Dtos
{
    public class GetAllVasPricesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public double? MaxPriceFilter { get; set; }
        public double? MinPriceFilter { get; set; }

        public int? MaxAmountFilter { get; set; }
        public int? MinAmountFilter { get; set; }

        public int? MaxCountFilter { get; set; }
        public int? MinCountFilter { get; set; }

        public string VasNameFilter { get; set; }
    }
}