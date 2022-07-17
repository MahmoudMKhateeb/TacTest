using Abp.Domain.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.DataExporting;
using TACHYON.Dto;
using TACHYON.Invoices.InoviceNote;

namespace TACHYON.Invoices.Reports
{
    public class InvoiceReportService : TACHYONAppServiceBase
    {
        private readonly InvoiceAppService _invoiceAppService;
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly IInvoiceNoteAppService _InvoiceNoteAppService;
        private readonly IRepository<InvoiceNote, long> _invoiceNoteRepository;

        public InvoiceReportService(InvoiceAppService invoiceAppService, PdfExporterBase pdfExporterBase, IInvoiceNoteAppService invoiceNoteAppService, IRepository<InvoiceNote, long> invoiceNoteRepository)
        {
            _invoiceAppService = invoiceAppService;
            _pdfExporterBase = pdfExporterBase;
            _InvoiceNoteAppService = invoiceNoteAppService;
            _invoiceNoteRepository = invoiceNoteRepository;
        }

        public FileDto DownloadInvoiceReportPdf(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/LandScapeInvoice.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            var invoice = _invoiceAppService.GetInvoiceReportInfo(invoiceId);
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(invoice);

            names.Add("GetInvoiceShippingRequestsReportInfoDataset");
            data.Add(_invoiceAppService.GetInvoiceShippingRequestsReportInfo(invoiceId));
            return _pdfExporterBase.CreateRdlcPdfPackageFromList(GetInvoiceNumber(invoiceId), reportPath, names, data);
        }
        public FileDto DonwloadPenaltyInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/PenaltyInvoice.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(_invoiceAppService.GetInvoiceReportInfo(invoiceId));

            names.Add("GetInvoicePenaltiseInvoiceReportInfo");
            data.Add(_invoiceAppService.GetInvoicePenaltiseInvoiceReportInfo(invoiceId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(GetInvoiceNumber(invoiceId), reportPath, names, data);
        }

        public FileDto DownloadInvoiceNoteReportPdf(long invoiceNoteId)
        {
            var reportPath = "/Invoices/Reports/InvoiceNote.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();

            names.Add("GetInvoiceNoteReportInfoDataset");
            data.Add(_InvoiceNoteAppService.GetInvoiceNoteReportInfo(invoiceNoteId));

            names.Add("GetInvoiceNoteItemReportInfoDataset");
            data.Add(_InvoiceNoteAppService.GetInvoiceNoteItemReportInfo(invoiceNoteId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(GetInvoiceNumberFromNote(invoiceNoteId), reportPath, names, data);
        }

        private string GetInvoiceNumber(long invoiceId)
        {
            var invoice = _invoiceAppService.GetInvoiceReportInfo(invoiceId);
            return invoice.FirstOrDefault()?.InvoiceNumber.ToString();
        }

        private string GetInvoiceNumberFromNote(long invoiceNoteId)
        {
            return _invoiceNoteRepository.FirstOrDefault(invoiceNoteId)?.ReferanceNumber.ToString();
        }
    }
}