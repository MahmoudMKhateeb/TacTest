using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;

namespace TACHYON.PricePackages.Dto.PricePackageAppendices
{
    public class AppendixListDto : EntityDto
    {
        public string CompanyName { get; set; }

        public string EditionName { get; set; }
        
        [JsonProperty("subject")]
        public string ContractName { get; set; }

        public string AppendixNumber { get; set; }

        public bool IsActive { get; set; }
        public string ContractNumber { get; set; }
        
        public DateTime CreationTime { get; set; }

        public AppendixStatus Status { get; set; }

        public int NumberOfPricePackages { get; set; }
    }
}