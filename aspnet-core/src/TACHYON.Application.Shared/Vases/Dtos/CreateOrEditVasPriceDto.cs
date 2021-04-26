
using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Validation;

namespace TACHYON.Vases.Dtos
{
    public class CreateOrEditVasPriceDto : EntityDto<int?>, ICustomValidate
    {

		public double? Price { get; set; }
		
		
		public int? MaxAmount { get; set; }
		
		
		public int? MaxCount { get; set; }
		
		
		 public int VasId { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (Price.HasValue && Price.Value<0)
            {
                context.Results.Add(new ValidationResult("ThePriceSholudBeNotLessThanZero"));
            }
            if (MaxAmount.HasValue && MaxAmount.Value < 1)
            {
                context.Results.Add(new ValidationResult("TheMaxAmountSholudBeNotLessThanOne"));
            }
            if (MaxCount.HasValue && MaxCount.Value < 1)
            {
                context.Results.Add(new ValidationResult("TheMaxCountSholudBeNotLessThanOne"));
            }
        }
    }
}