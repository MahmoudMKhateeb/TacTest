using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Shipping.TripStatuses.Dtos
{
    public class TripStatusDto : EntityDto
    {
        public string DisplayName { get; set; }

    }
}