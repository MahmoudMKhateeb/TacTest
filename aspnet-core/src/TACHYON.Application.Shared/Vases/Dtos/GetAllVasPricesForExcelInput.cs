using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Vases.Dtos
{
    public class GetAllVasPricesForExcelInput
    {
		public string Filter { get; set; }

		public double? MaxPriceFilter { get; set; }
		public double? MinPriceFilter { get; set; }

		public int? MaxMaxAmountFilter { get; set; }
		public int? MinMaxAmountFilter { get; set; }

		public int? MaxMaxCountFilter { get; set; }
		public int? MinMaxCountFilter { get; set; }


		 public string VasNameFilter { get; set; }

		 
    }
}