using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Redemption.Dtos
{
    public class GetAllRedemptionCodesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public DateTime? MaxRedemptionDateFilter { get; set; }
        public DateTime? MinRedemptionDateFilter { get; set; }

        public long? MaxRedemptionTenantIdFilter { get; set; }
        public long? MinRedemptionTenantIdFilter { get; set; }

        public string RedeemCodeCodeFilter { get; set; }

    }
}