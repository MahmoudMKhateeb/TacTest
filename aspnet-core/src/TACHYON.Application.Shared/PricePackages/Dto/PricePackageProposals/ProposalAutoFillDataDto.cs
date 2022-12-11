using System;

namespace TACHYON.PricePackages.Dto.PricePackageProposals
{
    /// <summary>
    /// this dto used for appendix creation
    /// that's can help user to auto fill data from proposal
    /// </summary>
    public class ProposalAutoFillDataDto
    {
        public DateTime AppendixDate { get; set; }

        public string Notes { get; set; }

        public string ScopeOverview { get; set; }
    }
}