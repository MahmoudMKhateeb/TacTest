using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Redemption.Dtos
{
    public class CreateOrEditRedemptionCodeDto : EntityDto<long?>
    {

        public long? RedeemCodeId { get; set; }

    }
}