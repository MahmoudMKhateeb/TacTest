using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices.ActorInvoices.Dto
{
    public class ActorInvoiceInfoDto : EntityDto<long>, IHasCreationTime
    {
        public string ClientName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string FaxNo { get; set; }
        public string Attn { get; set; }
        public string CR { get; set; }
        public string TenantVatNumber { get; set; }
        public string ContractNo { get; set; }
        public string ProjectName { get; set; }
        public string Period { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsPaid { get; set; }
        public InvoiceAccountType AccountType { get; set; }
        public string Note { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public long InvoiceNumber { get; set; }
        public List<InvoiceItemDto> Items { get; set; }



        public   byte[] Logo { get; set; }

        public string BrokerName { get; set; }


        public string BrokerBankNameEnglish { get; set; }
        public string BrokerBankNameArabic { get; set; }
        public string BrokerBankAccountNumber { get; set; }
        public string BrokerIban { get; set; }
        public string BrokerEmailAddress { get; set; }
        public string BrokerWebSite { get; set; }
        public string BrokerAddress { get; set; }
        public string BrokerMobile { get; set; }

        public string BrokerCr { get; set; }


        public string BrokerVat { get; set; }

    }
}