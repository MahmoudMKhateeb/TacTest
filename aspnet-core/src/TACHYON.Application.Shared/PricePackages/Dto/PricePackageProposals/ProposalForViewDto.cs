using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PricePackages.Dto.PricePackageProposals
{
    public class ProposalForViewDto : EntityDto
    {
        public string ProposalName { get; set; }

        public string ScopeOverview { get; set; }

        public DateTime? ProposalDate { get; set; }

        public string Notes { get; set; }

        public ProposalStatus Status { get; set; }
    }
}