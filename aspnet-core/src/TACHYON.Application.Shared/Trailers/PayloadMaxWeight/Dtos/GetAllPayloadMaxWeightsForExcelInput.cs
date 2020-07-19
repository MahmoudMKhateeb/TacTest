using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Trailers.PayloadMaxWeight.Dtos
{
    public class GetAllPayloadMaxWeightsForExcelInput
    {
		public string Filter { get; set; }

		public string DisplayNameFilter { get; set; }

		public int? MaxMaxWeightFilter { get; set; }
		public int? MinMaxWeightFilter { get; set; }



    }
}