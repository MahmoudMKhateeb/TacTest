using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PricePackages.Dto.PricePackageProposals
{
    public class ProposalListItemDto : EntityDto
    {
        public string ProposalName { get; set; }

        public string ShipperName { get; set; }

        public DateTime? CreationTime { get; set; }
        
        public ProposalStatus Status { get; set; }

        public string AppendixNumber { get; set; }

        public int NumberOfPricePackages { get; set; }

    }
}