
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Vases.Dtos
{
    public class VasPriceDto : EntityDto
    {
		public double? Price { get; set; }

		public int? MaxAmount { get; set; }

		public int? MaxCount { get; set; }

		public int VasId { get; set; } 
    }
}