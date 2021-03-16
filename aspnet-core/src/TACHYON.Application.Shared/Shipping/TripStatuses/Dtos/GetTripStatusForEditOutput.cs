using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Shipping.TripStatuses.Dtos
{
    public class GetTripStatusForEditOutput
    {
        public CreateOrEditTripStatusDto TripStatus { get; set; }

    }
}