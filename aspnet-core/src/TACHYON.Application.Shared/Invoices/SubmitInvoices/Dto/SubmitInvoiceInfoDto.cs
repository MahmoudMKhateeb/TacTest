﻿using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using TACHYON.Invoices.Dto;

namespace TACHYON.Invoices.SubmitInvoices.Dto
{
    public class SubmitInvoiceInfoDto : EntityDto<long>, IHasCreationTime
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
        public DateTime DueDate { get; set; }
        public DateTime CreationTime { get; set; }
        public string Note { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public long InvoiceNumber { get; set; }
        public List<InvoiceItemDto> Items { get; set; }


    }

}