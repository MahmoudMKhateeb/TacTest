using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trailers.Dtos
{
    public class GetAllTrailersForExcelInput
    {
		public string Filter { get; set; }

		public string TrailerCodeFilter { get; set; }

		public string PlateNumberFilter { get; set; }

		public string ModelFilter { get; set; }

		public string YearFilter { get; set; }

		public int? MaxWidthFilter { get; set; }
		public int? MinWidthFilter { get; set; }

		public int? MaxHeightFilter { get; set; }
		public int? MinHeightFilter { get; set; }

		public int? MaxLengthFilter { get; set; }
		public int? MinLengthFilter { get; set; }

		public int IsLiftgateFilter { get; set; }

		public int IsReeferFilter { get; set; }

		public int IsVentedFilter { get; set; }

		public int IsRollDoorFilter { get; set; }


		 public string TrailerStatusDisplayNameFilter { get; set; }

		 		 public string TrailerTypeDisplayNameFilter { get; set; }

		 		 public string PayloadMaxWeightDisplayNameFilter { get; set; }

		 		 public string TruckPlateNumberFilter { get; set; }

		 
    }
}