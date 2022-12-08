using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TACHYON.MultiTenancy;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TrucksTypes;

namespace TACHYON.PricePackages
{
    public class BasePricePackage : FullAuditedEntity, IMustHaveTenant
    {
        public string PricePackageId { get; set; }
        [Required]
        [StringLength(PricePackagesConst.MaxDisplayNameLength, MinimumLength = PricePackagesConst.MinDisplayNameLength)]
        public string DisplayName { get; set; }
        public int TenantId { get; set; }
        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
        public int TransportTypeId { get; set; }
        [ForeignKey(nameof(TransportTypeId))]
        public TransportType TransportTypeFk { get; set; }
        public long TrucksTypeId { get; set; }
        [ForeignKey(nameof(TrucksTypeId))]
        public TrucksType TrucksTypeFk { get; set; }
    }
}