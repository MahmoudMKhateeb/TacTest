using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text;
using TACHYON.Invoices;

namespace TACHYON.DedidcatedDynamicActorInvoices.Dtos
{
    public class CreateOrEditDedicatedActorInvoiceDto: EntityDto<long?>, ICustomValidate
    {
        public int? ShipperActorId { get; set; }
        public int? CarrierActorId { get; set; }
        public InvoiceAccountType InvoiceAccountType { get; set; }
        public long ShippingRequestId { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }

        public List<CreateOrEditDedicatedActorInvoiceItemDto> DedicatedActorInvoiceItems { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if(InvoiceAccountType == InvoiceAccountType.AccountReceivable && ShipperActorId == null)
            {
                context.Results.Add(new ValidationResult("Shipper actor must not be null"));
            }
            else if (InvoiceAccountType == InvoiceAccountType.AccountPayable && CarrierActorId == null)
            {
                context.Results.Add(new ValidationResult("carrier actor must not be null"));
            }
        }
    }
}
