using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DedicatedDynamicInvocies;

namespace TACHYON.DedicatedDynamicInvoices.Dtos
{
    public class DedicatedDynamicInvoiceItemDto
    {
        public string Sequence { get; set; }
        public string Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string TruckType { get; set; }
        public string TruckPlateNumber { get; set; }
        public string Duration { get; set; }
        public decimal PricePerDay { get; set; }
        public decimal SubTotalAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal TaxVat { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remarks { get; set; }
    }
}
