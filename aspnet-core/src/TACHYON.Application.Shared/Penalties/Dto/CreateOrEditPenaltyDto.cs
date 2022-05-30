using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.PriceOffers;

namespace TACHYON.Penalties.Dto
{
    public class CreateOrEditPenaltyDto : EntityDto<long?>
    {
        [Required]
        public string PenaltyName { get; set; }
        public string PenaltyDescrption { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int DestinationTenantId { get; set; }
        public PenaltyType Type { get; set; }
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }
    }
}
