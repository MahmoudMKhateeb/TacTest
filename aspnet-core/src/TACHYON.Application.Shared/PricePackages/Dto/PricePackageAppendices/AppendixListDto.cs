using Abp.Application.Services.Dto;
using System;

namespace TACHYON.PricePackages.Dto.PricePackageAppendices
{
    public class AppendixListDto : EntityDto
    {
        public string Shipper { get; set; }
        
        public string ContractName { get; set; }

        public int ContractNumber { get; set; }
        
        public DateTime? AppendixDate { get; set; }

        public AppendixStatus Status { get; set; }
    }
}