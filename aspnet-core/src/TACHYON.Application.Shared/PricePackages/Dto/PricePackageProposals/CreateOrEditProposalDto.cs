using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.PricePackages.Dto.PricePackageProposals
{
    public class CreateOrEditProposalDto : EntityDto<int?>, IShouldNormalize
    {
        [Required]
        [StringLength(125,MinimumLength = 3)]
        public string ProposalName { get; set; }
        
        [StringLength(500,MinimumLength = 3)]
        public string ScopeOverview { get; set; }

        [Required]
        public DateTime ProposalDate { get; set; }
        
        [Required]
        public int ShipperId { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public List<int> TmsPricePackages { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public void Normalize()
        {
            ProposalName = ProposalName.Trim();
        }
    }
}