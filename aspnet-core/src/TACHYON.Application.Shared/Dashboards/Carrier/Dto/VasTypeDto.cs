using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Carrier.Dto
{
    public class VasTypeDto
    {
        public string VasType { get; set; }

        public int AvailableVasTypeCount { get; set; }

    }

}