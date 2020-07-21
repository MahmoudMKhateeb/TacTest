using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Offers.Dtos
{
    public class GetAllOffersForExcelInput
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }

		public string DescriptionFilter { get; set; }

		public decimal? MaxPriceFilter { get; set; }
		public decimal? MinPriceFilter { get; set; }


		 public string TrucksTypeDisplayNameFilter { get; set; }

		 		 public string TrailerTypeDisplayNameFilter { get; set; }

		 		 public string GoodCategoryDisplayNameFilter { get; set; }

		 		 public string RouteDisplayNameFilter { get; set; }

		 
    }
}