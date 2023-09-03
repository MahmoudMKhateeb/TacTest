using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations;
using TACHYON.Authorization.Users.Dto;

namespace TACHYON.MultiTenancy.Dto
{
    public class TenantListDto : EntityDto, IPassivable, IHasCreationTime
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
        public string AccountNumber { get; set; }

        public string EditionDisplayName { get; set; }

        [DisableAuditing] public string ConnectionString { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public int? EditionId { get; set; }

        public bool IsInTrialPeriod { get; set; }
        public string ContractNumber { get; set; }

        public virtual string companyName { get; set; }

        public virtual string MobileNo { get; set; }

        public decimal Balance { get; set; } = 0;
        public decimal ReservedBalance { get; set; } = 0;
        public decimal CreditBalance { get; set; } = 0;
        public decimal Rate { get; set; }
        public string MoiNumber { get; set; }
        public int? InsuranceCoverage { get; set; }
        public int? ValueOfGoods { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string Address { get; set; }
        public string DocumentStatus { get; set; }
    }
}