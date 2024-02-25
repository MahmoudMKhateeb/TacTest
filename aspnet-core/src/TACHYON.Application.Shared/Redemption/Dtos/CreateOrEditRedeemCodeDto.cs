using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Redemption.Dtos
{
    public class CreateOrEditRedeemCodeDto : EntityDto<long?>
    {

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public decimal Value { get; set; }

        public string Note { get; set; }

        [Range(RedeemCodeConsts.MinpercentageValue, RedeemCodeConsts.MaxpercentageValue)]
        public int percentage { get; set; }

    }
}