using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Host.Dto
{
    public class GoodTypeAvailableDto : EntityDto<long>
    {
        public string GoodType { get; set; }

        public int AvailableGoodTypesCount { get; set; }
    }
}