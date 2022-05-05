using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;

namespace TACHYON.Dashboards.Shipper.Dto
{
    public class RequestsInMarketpalceDto
    {
        public string RequestReference { get; set; }
        public DateTime? BiddingEndDate { get; set; }
        public int NumberOfOffers { get; set; }
    }
}