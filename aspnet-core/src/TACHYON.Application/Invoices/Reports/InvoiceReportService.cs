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

        public InvoiceReportService(InvoiceAppService invoiceAppService, PdfExporterBase pdfExporterBase, IInvoiceNoteAppService invoiceNoteAppService)
        {
            _invoiceAppService = invoiceAppService;
            _pdfExporterBase = pdfExporterBase;
            _InvoiceNoteAppService = invoiceNoteAppService;
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
            var number = invoice.FirstOrDefault()?.InvoiceNumber.ToString();
            return _pdfExporterBase.CreateRdlcPdfPackageFromList(number, reportPath, names, data);
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

            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Invoice", reportPath, names, data);
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

            return _pdfExporterBase.CreateRdlcPdfPackageFromList("InvoiceNote", reportPath, names, data);
        }
    }
}