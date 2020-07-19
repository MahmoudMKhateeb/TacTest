using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trailers.PayloadMaxWeight.Dtos
{
    public class GetPayloadMaxWeightForEditOutput
    {
		public CreateOrEditPayloadMaxWeightDto PayloadMaxWeight { get; set; }


    }
}