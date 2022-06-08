using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Documents.DocumentFiles.Dtos;

namespace TACHYON.Shipping.Notes.Dto
{
    public class CreateOrEditShippingRequestAndTripNotesDto 
    {
        public int NoteId { get; set; }
        public string Note { get; set; }
        public int? TripId { get; set; }
        public long? ShippingRequetId { get; set; }
        public int TenantId { get; set; }
        public VisibilityNotes Visibility { get; set; } = VisibilityNotes.Internal;
        public List<CreateOrEditDocumentFileDto> CreateOrEditDocumentFileDto { get; set; }
        public bool IsPrintedByWabillInvoice { get; set; }
    }
}