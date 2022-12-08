using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PricePackages.Dto.PricePackageAppendices
{
    public class AppendixForViewDto : EntityDto
    {
        public string ContractName { get; set; }

        public int ContractNumber { get; set; }
            
        public string Version { get; set; }

        public DateTime? AppendixDate { get; set; }

        public string ScopeOverview { get; set; }

        public string Notes { get; set; }
        
        public string ProposalName { get; set; }

        public Guid? AppendixFileId { get; set; }

        public string StatusTitle { get; set; }

        public AppendixStatus Status { get; set; }
    }
}