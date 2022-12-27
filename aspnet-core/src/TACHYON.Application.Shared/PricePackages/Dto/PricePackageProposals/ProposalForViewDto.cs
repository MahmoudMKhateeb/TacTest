using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PricePackages.Dto.PricePackageProposals
{
    public class ProposalForViewDto : EntityDto
    {
        public string ProposalName { get; set; }

        public string ScopeOverview { get; set; }

        public string ProposalDate { get; set; }

        public string Notes { get; set; }

        public ProposalStatus Status { get; set; }
        
        public string StatusTitle { get; set; }

        public string Shipper { get; set; }

        public Guid? ProposalFileId { get; set; }
        
    }
}