using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TACHYON.MultiTenancy.Dto;

namespace TACHYON.Invoices.Balances.Dto
{
  public class BalanceRechargeListDto : EntityDto, IHasCreationTime
    {
        [Required]
        public string TenantName { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
