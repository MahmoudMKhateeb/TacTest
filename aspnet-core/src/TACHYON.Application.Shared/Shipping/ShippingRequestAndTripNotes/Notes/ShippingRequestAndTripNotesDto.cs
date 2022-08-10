using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Shipping.Notes.Dto
{
    public class ShippingRequestAndTripNotesDto
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public int TripId { get; set; }
        public string TenantName { get; set; }
        public string TenantImage { get; set; }
        public long ShippingRequetId { get; set; }
        public int TenantId { get; set; }
        public int LastModifierUserId { get; set; }
        public string LastModifierUserImage { get; set; }
        public string LastModifierUserName { get; set; }
        public DateTime LastModificationTime { get; set; }
        public VisibilityNotes Visibility { get; set; } = VisibilityNotes.Internal;
        public List<DocumentFileDto> DocumentFiles { get; set; }
        public bool IsPrintedByWabillInvoice { get; set; }
        public DateTime CreationTime { get; set; }

    }
}