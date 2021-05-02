using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TACHYON.Invoices.Balances.Dto
{

    public  class CreateBalanceRechargeInput: ICustomValidate
    {
        [Required]
        public int? TenantId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string ReferenceNo { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!TenantId.HasValue || TenantId.Value<=0)
            {
                context.Results.Add(new ValidationResult("YouMustSelectTheShipperFromAutoCompleteText"));
            }

            if (Amount <= 0)
            {
                context.Results.Add(new ValidationResult("TheAmountMustBeGreaterThanZero"));
            }
        }
    }
}
