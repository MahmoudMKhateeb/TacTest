using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Host.Dto
{
    public class ListUsersHaveMostRequests
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfRequests { get; set; }
        public decimal Rating { get; set; }

    }

}