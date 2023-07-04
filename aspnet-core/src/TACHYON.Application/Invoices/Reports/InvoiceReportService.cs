using Abp.Domain.Repositories;
using Abp.Threading;
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
        private readonly IRepository<Invoice, long> _invoiceRepository;


        public InvoiceReportService(InvoiceAppService invoiceAppService, PdfExporterBase pdfExporterBase, IInvoiceNoteAppService invoiceNoteAppService, IRepository<InvoiceNote, long> invoiceNoteRepository, IRepository<ActorInvoice, long> actorInvoiceRepository, IRepository<Invoice, long> invoiceRepository)
        {
            _invoiceAppService = invoiceAppService;
            _pdfExporterBase = pdfExporterBase;
            _InvoiceNoteAppService = invoiceNoteAppService;
            _invoiceNoteRepository = invoiceNoteRepository;
            _actorInvoiceRepository = actorInvoiceRepository;
            _invoiceRepository = invoiceRepository;
        }

        public FileDto DownloadInvoiceReportPdf(long invoiceId)
        {
            DisableTenancyFilters();
            AsyncHelper.RunSync(() => DisableInvoiceDraftedFilter());
            var invoice = _invoiceRepository.GetAll().Select(x=> new { x.Channel, x.Id }).FirstOrDefault(i => i.Id == invoiceId);

            if (invoice.Channel == InvoiceChannel.Trip)
            {
               return DownloadTripInvoice(invoiceId);
            }
            else if (invoice.Channel == InvoiceChannel.SaasTrip)
            {
                return DownloadSAASTripInvoice(invoice.Id);
            }

            else if (invoice.Channel == InvoiceChannel.Penalty)
            {
                return DonwloadPenaltyInvoice(invoice.Id);
            }

            else if (invoice.Channel == InvoiceChannel.DynamicInvoice)
            {
                return DownloadDynamicInvoice(invoice.Id);
            }
            else if (invoice.Channel == InvoiceChannel.Dedicated)
            {
                return DownloadDedicatedDynamicInvoice(invoice.Id);
            }
            return null;
        }


        private FileDto DownloadTripInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/LandScapeInvoice.rdlc";

            ArrayList names = new();
            ArrayList data = new();

            var invoiceDto = _invoiceAppService.GetInvoiceReportInfo(invoiceId);
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(invoiceDto);

            names.Add("GetInvoiceShippingRequestsReportInfoDataset");
            data.Add(_invoiceAppService.GetInvoiceShippingRequestsReportInfo(invoiceId));
            var file = _pdfExporterBase.CreateRdlcPdfPackageFromList(invoiceDto.First().InvoiceNumber.ToString(), reportPath, names, data);
            return file;
        }

        private FileDto DownloadSAASTripInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/LandScapeInvoice_SAAS.rdlc";

            ArrayList names = new ();
            ArrayList data = new ();

            var invoiceDto = _invoiceAppService.GetInvoiceReportInfo(invoiceId);
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(invoiceDto);

            names.Add("GetSAASInvoiceShippingRequestsReportInfoDataset");
            data.Add(_invoiceAppService.GetSAASInvoiceShippingRequestsReportInfo(invoiceId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(invoiceDto.First().InvoiceNumber.ToString(), reportPath, names, data);
        }

        private FileDto DonwloadPenaltyInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/PenaltyInvoice.rdlc";

            ArrayList names = new ();
            ArrayList data = new ();

            var invoiceDto = _invoiceAppService.GetInvoiceReportInfo(invoiceId);
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(invoiceDto);

            names.Add("GetInvoicePenaltiseInvoiceReportInfo");
            data.Add(_invoiceAppService.GetInvoicePenaltiseInvoiceReportInfo(invoiceId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(invoiceDto.First().InvoiceNumber.ToString(), reportPath, names, data);
        }

        private FileDto DownloadDynamicInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/DynamicInvoice.rdlc";

            ArrayList names = new ();
            ArrayList data = new ();
            var invoiceDto = _invoiceAppService.GetInvoiceReportInfo(invoiceId);
            names.Add("GetInvoiceReportInfoDataset");
            data.Add(invoiceDto);

            names.Add("GetInvoiceShippingRequestsReportInfoDataset");
            data.Add(_invoiceAppService.GetDynamicInvoiceItemsReportInfo(invoiceId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(invoiceDto.First().InvoiceNumber.ToString(), reportPath, names, data);
        }

        private FileDto DownloadDedicatedDynamicInvoice(long invoiceId)
        {
            var reportPath = "/Invoices/Reports/DedicatedInvoice.rdlc";

            ArrayList names = new ();
            ArrayList data = new ();

            var invoiceDto = _invoiceAppService.GetInvoiceReportInfo(invoiceId);

            names.Add("GetInvoiceReportInfoDataset");
            data.Add(invoiceDto);

            names.Add("GetDedicatedInvoiceItemReportInfo");
            data.Add(_invoiceAppService.GetDedicatedDynamicInvoiceItemsReportInfo(invoiceId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(invoiceDto.First().InvoiceNumber.ToString(), reportPath, names, data) ;
        }

        public FileDto DownloadInvoiceNoteReportPdf(long invoiceNoteId)
        {
            var reportPath = "/Invoices/Reports/InvoiceNote.rdlc";

            ArrayList names = new ();
            ArrayList data = new ();

            names.Add("GetInvoiceNoteReportInfoDataset");
            data.Add(_InvoiceNoteAppService.GetInvoiceNoteReportInfo(invoiceNoteId));

            names.Add("GetInvoiceNoteItemReportInfoDataset");
            data.Add(_InvoiceNoteAppService.GetInvoiceNoteItemReportInfo(invoiceNoteId));

            return _pdfExporterBase.CreateRdlcPdfPackageFromList(GetInvoiceNumberFromNote(invoiceNoteId), reportPath, names, data);
        }

        //private string GetInvoiceNumber(Invoice invoice)
        //{
        //   // var invoice = _invoiceAppService.GetInvoiceReportInfo(invoiceId);
        //    return invoice?.InvoiceNumber.ToString();
        //}

        private string GetInvoiceNumberFromNote(long invoiceNoteId)
        {
            return _invoiceNoteRepository.FirstOrDefault(invoiceNoteId)?.ReferanceNumber.ToString();
        }

        public async Task<FileDto> DownloadActorShipperInvoiceReportPdf(long actorInvoiceId)
        {
            var actorInvoice = await _actorInvoiceRepository.FirstOrDefaultAsync(x => x.Id == actorInvoiceId);
            if (actorInvoice.ActorInvoiceChannel == DedicatedDynamicInvocies.ActorInvoiceChannel.trip)
            {
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
            }

            else
            {
               // Complete here download dedicated invoice
                var reportPath = "/Invoices/Reports/DedicatedInvoice_Actor.rdlc";

                ArrayList names = new ArrayList();
                ArrayList data = new ArrayList();

                var invoice = await _invoiceAppService.GetActorShipperInvoiceReportInfo(actorInvoiceId);
                names.Add("GetInvoiceReportInfoDataset");
                data.Add(invoice);

                names.Add("GetDedicatedInvoiceItemReportInfo");
                data.Add(_invoiceAppService.GetDedicatedDynamicActorInvoiceItemsReportInfo(actorInvoiceId));
                var number = invoice.FirstOrDefault()?.InvoiceNumber.ToString();
                return _pdfExporterBase.CreateRdlcPdfPackageFromList(number, reportPath, names, data);
            }

        }



    }
}