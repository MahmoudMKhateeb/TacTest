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
        public string ReferenceNumber { get; set; }
        public string PenaltyDescrption { get; set; }
        /// <summary>
        /// Total price for items
        /// </summary>
        public decimal ItmePrice { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        public int TenantId { get; set; }
        public int? DestinationTenantId { get; set; }
        public PenaltyType Type { get; set; }
        public PriceOfferCommissionType CommissionType { get; set; }
        public decimal CommissionPercentageOrAddValue { get; set; }
        public PenaltyStatus Status { get; set; }
        public int? ShippingRequestTripId { get; set; }
        public List<PenaltyItemDto> PenaltyItems { get; set; }
    }
}
