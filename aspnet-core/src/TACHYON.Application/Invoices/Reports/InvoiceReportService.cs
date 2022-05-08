using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.DataExporting;
using TACHYON.Dto;

namespace TACHYON.Invoices.Reports
{
    public class InvoiceReportService : TACHYONAppServiceBase
    {
        private readonly InvoiceAppService _invoiceAppService;
        private readonly PdfExporterBase _pdfExporterBase;


        public InvoiceReportService(InvoiceAppService invoiceAppService, PdfExporterBase pdfExporterBase)
        {
            _invoiceAppService = invoiceAppService;
            _pdfExporterBase = pdfExporterBase;
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
    }
}