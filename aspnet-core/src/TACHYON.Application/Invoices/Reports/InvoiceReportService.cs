using Abp.Domain.Repositories;
using Org.BouncyCastle.Asn1.Esf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.DataExporting;
using TACHYON.Dto;
using TACHYON.Invoices.ActorInvoices;
using TACHYON.Invoices.InoviceNote;

namespace TACHYON.Invoices.Reports
{
    public class InvoiceReportService : TACHYONAppServiceBase
    {
        private readonly InvoiceAppService _invoiceAppService;
        private readonly PdfExporterBase _pdfExporterBase;
        private readonly IInvoiceNoteAppService _InvoiceNoteAppService;
        private readonly IRepository<InvoiceNote, long> _invoiceNoteRepository;
        private readonly IRepository<ActorInvoice, long> _actorInvoiceRepository;

        public InvoiceReportService(InvoiceAppService invoiceAppService, PdfExporterBase pdfExporterBase, IInvoiceNoteAppService invoiceNoteAppService, IRepository<InvoiceNote, long> invoiceNoteRepository, IRepository<ActorInvoice, long> actorInvoiceRepository)
        {
            _invoiceAppService = invoiceAppService;
            _pdfExporterBase = pdfExporterBase;
            _InvoiceNoteAppService = invoiceNoteAppService;
            _invoiceNoteRepository = invoiceNoteRepository;
            _actorInvoiceRepository = actorInvoiceRepository;
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

        public FileDto DownloadDynamicInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/DynamicInvoice.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(_invoiceAppService.GetInvoiceReportInfo(invoiceId));

            names.Add("GetInvoiceShippingRequestsReportInfoDataset");
            data.Add(_invoiceAppService.GetDynamicInvoiceItemsReportInfo(invoiceId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(GetInvoiceNumber(invoiceId), reportPath, names, data);
        }

        public FileDto DownloadDedicatedDynamicInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/DedicatedInvoice.rdlc";

            ArrayList names = new ArrayList();
            ArrayList data = new ArrayList();
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(_invoiceAppService.GetInvoiceReportInfo(invoiceId));

            names.Add("GetDedicatedInvoiceItemReportInfo");
            data.Add(_invoiceAppService.GetDedicatedDynamicInvoiceItemsReportInfo(invoiceId));

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

        public async Task<FileDto> DownloadActorShipperInvoiceReportPdf(long actorInvoiceId)
        {
            var actorInvoice = await _actorInvoiceRepository.FirstOrDefaultAsync(x => x.Id == actorInvoiceId);
            //if(actorInvoice.ActorInvoiceChannel == DedicatedDynamicInvocies.ActorInvoiceChannel.trip)
           // {
                var reportPath = "/Invoices/Reports/LandScapeInvoice_Actor.rdlc";

                ArrayList names = new ArrayList();
                ArrayList data = new ArrayList();

                var invoice = await _invoiceAppService.GetActorShipperInvoiceReportInfo(actorInvoiceId);
                names.Add("GetInvoiceReportInfoDataset");
                data.Add(invoice);

                names.Add("GetInvoiceShippingRequestsReportInfoDataset");
                data.Add(_invoiceAppService.GetActorInvoiceShippingRequestsReportInfo(actorInvoiceId));
                var number = invoice.FirstOrDefault()?.InvoiceNumber.ToString();
                return _pdfExporterBase.CreateRdlcPdfPackageFromList(number, reportPath, names, data);
           // }

            //else
            //{
            //    Complete here download dedicated invoice
            //    var reportPath = "/Invoices/Reports/DedicatedInvoice_Actor.rdlc";

            //    ArrayList names = new ArrayList();
            //    ArrayList data = new ArrayList();

            //    var invoice = await _invoiceAppService.GetActorShipperInvoiceReportInfo(actorInvoiceId);
            //    names.Add("GetInvoiceReportInfoDataset");
            //    data.Add(invoice);

            //    names.Add("GetInvoiceShippingRequestsReportInfoDataset");
            //    data.Add(_invoiceAppService.GetActorInvoiceShippingRequestsReportInfo(actorInvoiceId));
            //    var number = invoice.FirstOrDefault()?.InvoiceNumber.ToString();
            //    return _pdfExporterBase.CreateRdlcPdfPackageFromList(number, reportPath, names, data);
            //}

        }



    }
}