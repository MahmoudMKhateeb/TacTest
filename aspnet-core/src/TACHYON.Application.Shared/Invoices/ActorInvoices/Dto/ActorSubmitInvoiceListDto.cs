﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.DedicatedDynamicInvocies;
using TACHYON.Invoices.SubmitInvoices;

namespace TACHYON.Invoices.ActorInvoices.Dto
{
    public class ActorSubmitInvoiceListDto: CreationAuditedEntityDto<long>
    {
        public string ReferencNumber { get; set; }
       // public int PeriodId { get; set; }
        public string TenantName { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }

        public string CarrierActorName { get; set; }
        public int? CarrierActorId { get; set; }
        public string Status { get; set; }
        public Guid? DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContentType { get; set; }
        public ActorInvoiceChannel ActorInvoiceChannel { get; set; }
        public string ActorInvoiceChannelTitle { get; set; }
    }
}
