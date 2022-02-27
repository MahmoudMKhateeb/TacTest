using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Host.Dto
{
    public class GetDataByDateFilterInput
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }

}