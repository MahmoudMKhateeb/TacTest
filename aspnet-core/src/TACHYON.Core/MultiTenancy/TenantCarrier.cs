using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.MultiTenancy
{
    [Table("TenantCarriers")]
    public  class TenantCarrier:Entity<long>,IMustHaveTenant, ICreationAudited
    {
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant  TenantShipper { get; set; }
        public int CarrierTenantId { get; set; }
        [ForeignKey(nameof(CarrierTenantId))]
        public Tenant CarrierShipper { get; set; }
        public long? CreatorUserId { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
