﻿using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PriceOffers.Dto
{
    public class CreateOrEditPriceOfferInput : ICustomValidate
    {
        public long ShippingRequestId { get; set; }
        public decimal ItemPrice { get; set; }
        [JsonIgnore]
        public PriceOfferChannel Channel { get; set; }
        [JsonIgnore]
        public PriceOfferType PriceType { get; set; } = PriceOfferType.Trip;
        public List<PriceOfferDetailDto> ItemDetails;
        [JsonIgnore]
        public decimal? CommissionPercentageOrAddValue { get; set; }
        [JsonIgnore]
        public PriceOfferCommissionType CommissionType { get; set; }
        [JsonIgnore]
        public long? SourceId { get; set; }
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (ItemPrice <= 0)
            {
                context.Results.Add(new ValidationResult("ThePriceMustBeGreaterThanZero"));
            }
        }
    }

}
