﻿using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;
using TACHYON.PricePackages.PricePackageAppendices;
using TACHYON.PricePackages.TmsPricePackages;

namespace TACHYON.PricePackages.PricePackageProposals
{
    [Table("PricePackageProposals")]
    public class PricePackageProposal : FullAuditedEntity
    {
        public string ProposalName { get; set; }

        public string ScopeOverview { get; set; }

        public DateTime? ProposalDate { get; set; }
        
        public int ShipperId { get; set; }

        [ForeignKey(nameof(ShipperId))]
        public Tenant Shipper { get; set; }
        
        public string Notes { get; set; }

        public ProposalStatus Status { get; set; }

        public Guid? ProposalFileId { get; set; }

        public int? AppendixId { get; set; }

        [ForeignKey(nameof(AppendixId))]
        public PricePackageAppendix Appendix { get; set; }
        public List<TmsPricePackage> TmsPricePackages { get; set; }
    }
}