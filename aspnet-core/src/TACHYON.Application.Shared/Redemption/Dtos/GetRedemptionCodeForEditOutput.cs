using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Redemption.Dtos
{
    public class GetRedemptionCodeForEditOutput
    {
        public CreateOrEditRedemptionCodeDto RedemptionCode { get; set; }

        public string RedeemCodeCode { get; set; }

    }
}