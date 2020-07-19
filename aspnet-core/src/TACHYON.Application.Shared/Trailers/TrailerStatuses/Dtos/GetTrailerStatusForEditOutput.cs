using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trailers.TrailerStatuses.Dtos
{
    public class GetTrailerStatusForEditOutput
    {
		public CreateOrEditTrailerStatusDto TrailerStatus { get; set; }


    }
}