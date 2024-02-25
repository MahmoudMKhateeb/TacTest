using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Redemption.Dtos
{
    public class RedemptionCodeDto : EntityDto<long>
    {
        public DateTime RedemptionDate { get; set; }

        public long RedemptionTenantId { get; set; }

        public long? RedeemCodeId { get; set; }

    }
}