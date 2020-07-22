using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trailers.TrailerTypes.Dtos
{
    public class GetTrailerTypeForEditOutput
    {
        public CreateOrEditTrailerTypeDto TrailerType { get; set; }


    }
}