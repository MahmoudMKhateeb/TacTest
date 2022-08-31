using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Localization.Sources;
using Abp.Runtime.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.DynamicInvoices.DynamicInvoiceItems;

namespace TACHYON.DynamicInvoices.Dto
{
    public class CreateOrEditDynamicInvoiceDto : EntityDto<long?>, ICustomValidate
    {
        public int? CreditTenantId { get; set; }
        
        public int? DebitTenantId { get; set; }

        public long WaybillNumber { get; set; }

        public List<CreateOrEditDynamicInvoiceItemDto> Items { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            
            
            if (!CreditTenantId.HasValue && !DebitTenantId.HasValue)
                context.Results.Add(new ValidationResult("YouMustSelectADebitOrCreditTenant"));
            
            if (Items.IsNullOrEmpty())
                context.Results.Add(new ValidationResult(("YouMustAddAtLeastOneItem")));
            
        }
    }
}