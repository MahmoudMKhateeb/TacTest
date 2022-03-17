using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Host.Dto
{
    public class ListPerMonthDto
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public int Week { get; set; }
        public string Month { get; set; }
        public int Count { get; set; }

    }

}