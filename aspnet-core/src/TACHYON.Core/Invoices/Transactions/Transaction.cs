﻿using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.MultiTenancy;

namespace TACHYON.Invoices.Transactions
{
    [Table("Transactions")]
    public class Transaction : Entity<long>, IHasCreationTime, ICreationAudited
    {
        public byte ChannelId { get; set; }
        [ForeignKey("ChannelId")]
        public TransactionChannel Channel { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; } = 1;
        public int? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public Tenant Tenant { get; set; }

        public long SourceId { get; set; }
        public DateTime CreationTime { get; set; } = Clock.Now;
        public long? CreatorUserId { get; set;}


    }
}