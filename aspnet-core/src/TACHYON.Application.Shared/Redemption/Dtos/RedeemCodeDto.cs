using System;
using Abp.Application.Services.Dto;

namespace TACHYON.Redemption.Dtos
{
    public class RedeemCodeDto : EntityDto<long>
    {
        public string Code { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public decimal Value { get; set; }

        public string Note { get; set; }

        public int percentage { get; set; }

    }
}