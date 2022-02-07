using System;
using System.Collections;
using System.Collections.Generic;
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

            names.Add("GetInvoiceReportInfoDataset");
            data.Add(_invoiceAppService.GetInvoiceReportInfo(invoiceId));

            names.Add("GetInvoiceShippingRequestsReportInfoDataset");
            data.Add(_invoiceAppService.GetInvoiceShippingRequestsReportInfo(invoiceId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList("Invoice", reportPath, names, data);
        }
    }
}