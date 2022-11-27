using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.PricePackages.TmsPricePackages;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    [Table("PricePackageAppendixes")]
    public class PricePackageAppendix : FullAuditedEntity
    {
        public string ContractName { get; set; }

        public int Version { get; set; }

        public DateTime ContractDate { get; set; }
        
        public DateTime AppendixDate { get; set; }
    
        public string ScopeOverview { get; set; }

        public string Notes { get; set; }
        
        public int? ProposalId { get; set; }

        [ForeignKey(nameof(ProposalId))]
        public PricePackageProposal Proposal { get; set; }
        
        public Guid? AppendixFileId { get; set; }

        public AppendixStatus Status { get; set; }

        [NotMapped]
        public List<TmsPricePackage> TmsPricePackages { get; set; }
    }
}