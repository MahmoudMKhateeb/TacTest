using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Localization.Sources;
using Abp.Runtime.Validation;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;

namespace TACHYON.DynamicInvoices.Dto
{
    public class CreateOrEditDynamicInvoiceDto : EntityDto<long?>, ICustomValidate
    {
        public int? CreditTenantId { get; set; }
        
        public int? DebitTenantId { get; set; }
        
        public string Notes { get; set; }

        public List<CreateOrEditDynamicInvoiceItemDto> Items { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!Id.HasValue && Items.Any(x => x.Id.HasValue))
                context.Results.Add(new ValidationResult("YouCanNotAddAnAlreadyExistItemIntoNewDynamicInvoice"));
            
            if (!CreditTenantId.HasValue && !DebitTenantId.HasValue)
                context.Results.Add(new ValidationResult("YouMustSelectADebitOrCreditTenant"));
            
            if (Items.IsNullOrEmpty())
                context.Results.Add(new ValidationResult(("YouMustAddAtLeastOneItem")));
            
        }
    }
}