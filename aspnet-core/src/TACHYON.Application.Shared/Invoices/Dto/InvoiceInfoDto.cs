using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace TACHYON.Invoices.Dto
{
    public class InvoiceInfoDto : EntityDto<long>, IHasCreationTime
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
        public string FinancialName { get; set; }
        public string FinancialPhone { get; set; }
        public string FinancialEmail { get; set; }
        public string QRCode { get; set; }
        public InvoiceChannel Channel { get; set; }

        public List<InvoiceItemDto> Items { get; set; }

        /// <summary>
        /// Shown in PDF 
        /// </summary>
        public string BankNameArabic { get; set; }

        /// <summary>
        /// Shown in PDF 
        /// </summary>
        public string BankNameEnglish { get; set; }

    }
}