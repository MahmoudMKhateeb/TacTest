using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Trailers.Dtos
{
    public class GetTrailerForEditOutput
    {
        public CreateOrEditTrailerDto Trailer { get; set; }

        public string TrailerStatusDisplayName { get; set; }

        public string TrailerTypeDisplayName { get; set; }

        public string PayloadMaxWeightDisplayName { get; set; }

        public string TruckPlateNumber { get; set; }
    }
}