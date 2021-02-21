using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Invoices.GroupsGroups.Dto;

namespace TACHYON.Invoices.Groups.Dto
{
   public class GroupPeriodInfoDto : EntityDto<long>, IHasCreationTime
    {
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string FaxNo { get; set; }
        public string Attn { get; set; }
        public string CR { get; set; }
        public string ContractNo { get; set; }
        public string ProjectName { get; set; }
        public string Period { get; set; }
        public bool IsDemand { get; set; }
        public Guid? BinaryObjectId { get; set; }
        public bool IsClaim { get; set; }
        public string Note { get; set; }
        public decimal AmountWithTaxVat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxVat { get; set; }
        public DateTime CreationTime { get; set; }
        public List<GroupShippingRequestDto> ShippingRequest { get; set; }
    }

}
