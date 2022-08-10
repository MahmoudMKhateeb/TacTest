using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Invoices.Dto
{
    public class InvoiceListDto : CreationAuditedEntityDto<long>
    {
        public long InvoiceNumber { get; set; }
        public int PeriodId { get; set; }
        public string TenantName { get; set; }
        public string Period { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public InvoiceAccountType AccountType { get; set; }
        public InvoiceChannel Channel { get; set; }
        public string InvoiceChannelTitle { get { return Channel.GetEnumDescription(); } }
        public string AccountTypeTitle { get { return AccountType.GetEnumDescription(); } }
        public decimal TotalAmount { get; set; }
    }
}