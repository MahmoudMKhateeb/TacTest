using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PricePackages.Dto.PricePackageAppendices
{
    public class CreateOrEditAppendixDto : EntityDto<int?>, ICustomValidate
    {
        public string ContractName { get; set; }

        public DateTime AppendixDate { get; set; }
        
        public DateTime ContractDate { get; set; }

        public string EmailAddress { get; set; }

        public string ScopeOverview { get; set; }

        public string Notes { get; set; }
        
        public int? ProposalId { get; set; }

        public int? DestinationCompanyId { get; set; }
        
        public List<int> TmsPricePackages { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            // Note: for shipper appendix => the appendix must have proposal
            // Note: for carrier appendix => the appendix must have tms price packages
            
            if (ProposalId.HasValue && !TmsPricePackages.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("You can't create appendix with proposal and price packages at the same time"));
            
            else if (!ProposalId.HasValue && TmsPricePackages.IsNullOrEmpty())
                context.Results.Add(new ValidationResult("you must add proposal or price packages to create appendix"));
        }
    }
}