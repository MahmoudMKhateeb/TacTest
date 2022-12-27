using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.PricePackages.TmsPricePackages;

namespace TACHYON.PricePackages.PricePackageAppendices
{
    [Table("PricePackageAppendixes")]
    public class PricePackageAppendix : FullAuditedEntity, IPassivable
    {
        public string ContractName { get; set; }

        public int MajorVersion { get; set; }
        
        public int MinorVersion { get; set; }

        public DateTime AppendixDate { get; set; }
    
        public string ScopeOverview { get; set; }

        public string Notes { get; set; }
        
        public int? ProposalId { get; set; }

        [ForeignKey(nameof(ProposalId))]
        public PricePackageProposal Proposal { get; set; }
        
        public Guid? AppendixFileId { get; set; }

        public AppendixStatus Status { get; set; }

        public int? DestinationTenantId { get; set; }

        [ForeignKey(nameof(DestinationTenantId))]
        public Tenant DestinationTenant { get; set; }
        
        public List<TmsPricePackage> TmsPricePackages { get; set; }
        
        public List<NormalPricePackage> NormalPricePackages { get; set; }
        
        public bool IsActive { get; set; }
    }
}