using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PricePackages.Dto.PricePackageAppendices
{
    public class CreateOrEditAppendixDto : EntityDto<int?>
    {
        public string ContractName { get; set; }

        public DateTime AppendixDate { get; set; }
        
        public DateTime ContractDate { get; set; }

        public string EmailAddress { get; set; }

        public string ScopeOverview { get; set; }

        public string Notes { get; set; }
        
        public int ProposalId { get; set; }

        public int? ShipperId { get; set; }
        
    }
}