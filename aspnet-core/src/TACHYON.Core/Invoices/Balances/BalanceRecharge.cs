using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TACHYON.MultiTenancy;

namespace TACHYON.Invoices.Balances
{
    [Table("BalanceRecharges")]
    public class BalanceRecharge: Entity<int>, IMustHaveTenant, IHasCreationTime
    {
        [Required]
        public int TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public DateTime CreationTime { get; set; }

        public string ReferenceNo { get; set; }
        public BalanceRecharge()
        {
            CreationTime = Clock.Now;
        }
    }
}
