using Abp.Application.Services;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Common;
using TACHYON.Invoices.InoviceNote.Dto;

namespace TACHYON.Invoices.InoviceNote
{
    public interface IInvoiceNoteAppService : IApplicationService
    {
        Task<LoadResult> GetAllInoviceNote(LoadOptionsInput input);
        Task CreateOrEdit(CreateOrEditInvoiceNoteDto input);
        Task ChangeInvoiceNoteStatus(long id);
        Task GenrateFullVoidInvoiceNote(long id);
        Task<GetInvoiceNoteForEditDto> GetInvoiceNoteForEdit(int id);
        List<InvoiceNoteInfoDto> GetInvoiceNoteReportInfo(long id);
        List<InvoiceNoteItemDto> GetInvoiceNoteItemReportInfo(long invoiceNoteId);
        Task<PartialVoidInvoiceDto> GetInvoiceForPartialVoid(long id);
    }
}
