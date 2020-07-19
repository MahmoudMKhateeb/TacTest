
using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Trailers.PayloadMaxWeight.Dtos
{
    public class PayloadMaxWeightDto : EntityDto
    {
		public string DisplayName { get; set; }

		public int MaxWeight { get; set; }



    }
}