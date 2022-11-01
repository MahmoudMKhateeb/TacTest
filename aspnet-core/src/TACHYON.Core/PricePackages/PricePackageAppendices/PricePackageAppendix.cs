using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.PricePackages.PricePackageProposals;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    [Table("PricePackageAppendixes")]
    public class PricePackageAppendix : FullAuditedEntity
    {
        public string ContractName { get; set; }

        public int ContractNumber { get; set; }
        
        public int Version { get; set; }

        public DateTime? AppendixDate { get; set; }

        public string ScopeOverview { get; set; }

        public string Notes { get; set; }
        
        public int ProposalId { get; set; }

        [ForeignKey(nameof(ProposalId))]
        public PricePackageProposal Proposal { get; set; }

        public Guid? AppendixFileId { get; set; }

        public AppendixStatus Status { get; set; }
    }
}