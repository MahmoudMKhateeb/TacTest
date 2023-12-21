using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Redemption.Dtos
{
    public class GetRedeemCodeForEditOutput
    {
        public CreateOrEditRedeemCodeDto RedeemCode { get; set; }

    }
}