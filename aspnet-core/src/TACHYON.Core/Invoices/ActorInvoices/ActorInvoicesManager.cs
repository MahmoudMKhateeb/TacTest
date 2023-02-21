using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Actors;
using TACHYON.DedicatedDynamicActorInvoices;
using TACHYON.DedicatedDynamicInvocies;
using TACHYON.Dto;
using TACHYON.Features;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;

namespace TACHYON.Invoices.ActorInvoices
{
    public class ActorInvoicesManager : TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IFeatureChecker _featureChecker;
        private readonly IRepository<ActorInvoice, long> _actorInvoiceRepository;
        private readonly IRepository<ActorSubmitInvoice, long> _actorSubmitInvoiceRepository;
        private readonly IAbpSession _session;

        public ActorInvoicesManager(IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<Actor> actorRepository,
            IRepository<ActorInvoice, long> actorInvoiceRepository,
            IRepository<ActorSubmitInvoice, long> actorSubmitInvoiceRepository, IFeatureChecker featureChecker, IAbpSession session)
        {
            this._shippingRequestTripRepository = shippingRequestTripRepository;
            this._actorRepository = actorRepository;
            this._actorInvoiceRepository = actorInvoiceRepository;
            _actorSubmitInvoiceRepository = actorSubmitInvoiceRepository;
            _featureChecker = featureChecker;
            _session = session;
        }

        public async Task<bool> BuildActorShipperInvoices(int actorId, List<SelectItemDto> SelectedTrips)
        {

            var actor = await _actorRepository.FirstOrDefaultAsync(actorId);

            List<ShippingRequestTrip> trips = await GetAllShipperActorUnInvoicedTrips(actorId, SelectedTrips);

            var dueDate = Clock.Now.AddDays(actor.InvoiceDueDays);




            if (trips != null && trips.Count() > 0)
            {

                var nonDirectTrips = trips.Where(x => x.ShippingRequestId.HasValue).ToList();
                var directTrips = trips.Where(x => !x.ShippingRequestId.HasValue).ToList();


                decimal nonDirectTripsTotalAmount = 0;
                decimal nonDirectTripsVatAmount = 0;
                decimal nonDirectTripsSubTotalAmount = 0;
                //decimal nonDirectTripsTaxVat = 0;


                decimal directTripsTotalAmount = 0;
                decimal directTripsVatAmount = 0;
                decimal directTripsSubTotalAmount = 0;
                //decimal directTripsTaxVat = 0;



                if (nonDirectTrips.Any())
                {
                    nonDirectTripsTotalAmount = (decimal)nonDirectTrips.Sum(r => r.ShippingRequestFk.ActorShipperPrice.TotalAmountWithCommission + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.TotalAmountWithCommission));
                    nonDirectTripsVatAmount = (decimal)nonDirectTrips.Sum(r => r.ShippingRequestFk.ActorShipperPrice.VatAmountWithCommission + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.VatAmountWithCommission));
                    nonDirectTripsSubTotalAmount = (decimal)nonDirectTrips.Sum(r => r.ShippingRequestFk.ActorShipperPrice.SubTotalAmountWithCommission + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.SubTotalAmountWithCommission));
                   // nonDirectTripsTaxVat = (decimal)nonDirectTrips.Select(x => x.ShippingRequestFk).FirstOrDefault().ActorShipperPrice.TaxVat;

                }

                if (directTrips.Any())
                {
                    directTripsTotalAmount = (decimal)directTrips.Sum(r => r.ActorShipperPrice.TotalAmountWithCommission + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.TotalAmountWithCommission));
                    directTripsVatAmount = (decimal)directTrips.Sum(r => r.ActorShipperPrice.VatAmountWithCommission + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.VatAmountWithCommission));
                    directTripsSubTotalAmount = (decimal)directTrips.Sum(r => r.ActorShipperPrice.SubTotalAmountWithCommission + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.SubTotalAmountWithCommission));
                   // directTripsTaxVat = (decimal)directTrips.FirstOrDefault().ActorShipperPrice.TaxVat;


                }


                decimal totalAmount = nonDirectTripsTotalAmount + directTripsTotalAmount;
                decimal vatAmount = nonDirectTripsVatAmount + directTripsVatAmount;
                decimal subTotalAmount = nonDirectTripsSubTotalAmount + directTripsSubTotalAmount;
                decimal taxVat = 0.15m;

                var actorInvoice = new ActorInvoice()
                {
                    TenantId = actor.TenantId,
                    ShipperActorId = actor.Id,
                    DueDate = dueDate,
                    TotalAmount = totalAmount,
                    VatAmount = vatAmount,
                    SubTotalAmount = subTotalAmount,
                    TaxVat = taxVat,
                    ActorInvoiceChannel = ActorInvoiceChannel.trip,
                    Trips = trips
                };

                var invoice = await _actorInvoiceRepository.InsertAsync(actorInvoice);

                //Generate invoice number
                await GenerateInvoiceNumber(invoice);

                foreach (var trip in trips)
                {
                    trip.IsActorShipperHaveInvoice = true;
                }
                return true;
            }
            return false;
        }

        public async Task<List<ShippingRequestTrip>> GetAllShipperActorUnInvoicedTrips(int actorId, List<SelectItemDto> trips)
        {

            var tripsList = await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShippingRequestId.HasValue)
             .Include(trip => trip.ShippingRequestTripVases)
             .ThenInclude(x => x.ShippingRequestVasFk)
             .ThenInclude(v => v.ActorShipperPrice)
             .Include(trip => trip.ShippingRequestFk)
             .ThenInclude(request => request.ActorShipperPrice)
             .WhereIf(trips != null, x => trips.Select(y => y.Id).Select(int.Parse).Contains(x.Id))
             .Where(trip => trip.ShippingRequestFk.ShippingRequestFlag == Shipping.ShippingRequests.ShippingRequestFlag.Normal)
             .Where(trip => trip.ShippingRequestFk.ShipperActorId == actorId)
             .Where(trip => trip.ShippingRequestFk.ActorShipperPriceId.HasValue)
             .Where(trip => !trip.IsActorShipperHaveInvoice)
             .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered ||
             trip.Status == Shipping.Trips.ShippingRequestTripStatus.DeliveredAndNeedsConfirmation).ToListAsync();

            var directTrips = await _shippingRequestTripRepository.GetAll()
            .Where(x => !x.ShippingRequestId.HasValue)
            .Include(trip => trip.ShippingRequestTripVases)
            .ThenInclude(x => x.ShippingRequestVasFk)
            .ThenInclude(v => v.ActorShipperPrice)
            .Include(x => x.ActorCarrierPrice)
            .Include(x => x.ActorShipperPrice)
            .WhereIf(trips != null, x => trips.Select(y => y.Id).Select(int.Parse).Contains(x.Id))
            .Where(trip => trip.ShipperActorId == actorId)
            .Where(trip => trip.ActorShipperPrice != null)
            .Where(trip => !trip.IsActorShipperHaveInvoice)
            .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered ||
            trip.Status == Shipping.Trips.ShippingRequestTripStatus.DeliveredAndNeedsConfirmation).ToListAsync();

            var result = tripsList.Concat(directTrips);

            return result.ToList();
        }

        public async Task<bool> BuildActorCarrierInvoices(int actorId, List<SelectItemDto> SelectedTrips)
        {

            var actor = await _actorRepository.FirstOrDefaultAsync(actorId);

            List<ShippingRequestTrip> trips = await GetAllCarrierActorUnInvoicedTrips(actorId, SelectedTrips);

            var dueDate = Clock.Now.AddDays(actor.InvoiceDueDays);


            if (trips != null && trips.Count > 0)
            {
                var nonDirectTrips = trips.Where(x => x.ShippingRequestId.HasValue).ToList();
                var directTrips = trips.Where(x => !x.ShippingRequestId.HasValue);



                decimal nonDirectTripsTotalAmount = 0;
                decimal nonDirectTripsVatAmount = 0;
                decimal nonDirectTripsSubTotalAmount = 0;
                decimal nonDirectTripsTaxVat = 0;

                decimal directTripsTotalAmount = 0;
                decimal directTripsVatAmount = 0;
                decimal directTripsSubTotalAmount = 0;
                //decimal directTripsTaxVat = 0;



                if (nonDirectTrips != null)
                {
                    nonDirectTripsTotalAmount = (decimal)nonDirectTrips.Sum(r => r.ShippingRequestFk.ActorCarrierPrice.SubTotalAmount + r.ShippingRequestFk.ActorCarrierPrice.VatAmount + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount + v.ActorCarrierPrice.VatAmount));
                    nonDirectTripsVatAmount = (decimal)nonDirectTrips.Sum(r => r.ShippingRequestFk.ActorCarrierPrice.VatAmount + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount));
                    nonDirectTripsSubTotalAmount = (decimal)nonDirectTrips.Sum(r => r.ShippingRequestFk.ActorCarrierPrice.SubTotalAmount + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount));
                    //nonDirectTripsTaxVat = (decimal)nonDirectTrips.Select(x => x.ShippingRequestFk).FirstOrDefault()?.ActorCarrierPrice?.TaxVat;

                }

                if (directTrips != null)
                {
                    directTripsTotalAmount = (decimal)directTrips.Sum(r => r.ActorCarrierPrice.SubTotalAmount + r.ActorCarrierPrice.VatAmount + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount + v.ActorCarrierPrice.VatAmount));
                    directTripsVatAmount = (decimal)directTrips.Sum(r => r.ActorCarrierPrice.VatAmount + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount));
                    directTripsSubTotalAmount = (decimal)directTrips.Sum(r => r.ActorCarrierPrice.SubTotalAmount + r.ShippingRequestTripVases.Select(x => x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount));
                    //directTripsTaxVat = (decimal)directTrips.First()?.TaxVat;


                }

                decimal totalAmount = nonDirectTripsTotalAmount + directTripsTotalAmount;
                decimal vatAmount = nonDirectTripsVatAmount + directTripsVatAmount;
                decimal subTotalAmount = nonDirectTripsSubTotalAmount + directTripsSubTotalAmount;
                decimal taxVat = 0.15m; 



                var actorSubmitInvoice = new ActorSubmitInvoice()
                {
                    TenantId = actor.TenantId,
                    CarrierActorId = actor.Id,
                    DueDate = dueDate,
                    TotalAmount = totalAmount,
                    VatAmount = vatAmount,
                    SubTotalAmount = subTotalAmount,
                    TaxVat = taxVat,
                    ActorInvoiceChannel = ActorInvoiceChannel.trip,
                    Trips = trips
                };

                var invoice = await _actorSubmitInvoiceRepository.InsertAsync(actorSubmitInvoice);

                //Generate submit invoice number
                await GenerateSubmitReferenceNumber(invoice);



                foreach (var trip in trips)
                {
                    trip.IsActorCarrierHaveInvoice = true;
                }
                return true;
            }
            return false;
        }

        public async Task<List<ShippingRequestTrip>> GetAllCarrierActorUnInvoicedTrips(int actorId, List<SelectItemDto> trips)
        {
            DisableTenancyFilters();
            var tripsList = await _shippingRequestTripRepository.GetAll()
                .Where(x => x.ShippingRequestId.HasValue)
             .Include(trip => trip.ShippingRequestTripVases)
             .ThenInclude(x => x.ShippingRequestVasFk)
             .ThenInclude(v => v.ActorCarrierPrice)
             .Include(trip => trip.ShippingRequestFk)
             .ThenInclude(request => request.ActorCarrierPrice)
             .WhereIf(trips != null, x => trips.Select(y => y.Id).Select(int.Parse).Contains(x.Id))
             .Where(trip => trip.ShippingRequestFk.ShippingRequestFlag == Shipping.ShippingRequests.ShippingRequestFlag.Normal)
             .Where(trip => trip.ShippingRequestFk.CarrierActorId == actorId)
             .Where(trip => trip.ShippingRequestFk.ActorCarrierPriceId.HasValue)
             .Where(trip => !trip.IsActorCarrierHaveInvoice)
             .Where(x => x.ShippingRequestFk.CarrierTenantId == _session.TenantId && x.ShippingRequestFk.CarrierActorFk.TenantId == _session.TenantId)
             .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered ||
             trip.Status == Shipping.Trips.ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
             .ToListAsync();

            var directShipments = await _shippingRequestTripRepository.GetAll()
              .Include(trip => trip.ShippingRequestTripVases)
              .ThenInclude(x => x.ShippingRequestVasFk)
              .ThenInclude(v => v.ActorCarrierPrice)
              .Include(trip => trip.ActorCarrierPrice)
              .Include(trip => trip.ActorShipperPrice)
              .Where(x => !x.ShippingRequestId.HasValue)
              .Where(trip => trip.ActorCarrierPrice != null)
              .WhereIf(trips != null, x => trips.Select(y => y.Id).Select(int.Parse).Contains(x.Id))
              .Where(trip => trip.CarrierActorId == actorId)
              .Where(trip => !trip.IsActorCarrierHaveInvoice)
              .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered ||
                             trip.Status == Shipping.Trips.ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
              .ToListAsync();

            var result = tripsList.Concat(directShipments);

            return result.ToList();
        }

        //shipper actor invoice
        public async Task GenerateDedicatedDynamicActorInvoice(DedicatedDynamicActorInvoice dedicatedDynamicActorInvoice)
        {
            decimal subTotalAmount = dedicatedDynamicActorInvoice.SubTotalAmount;
            var tax = 15;

            decimal vatAmount = dedicatedDynamicActorInvoice.VatAmount;
            decimal totalAmount = dedicatedDynamicActorInvoice.TotalAmount;

            DateTime dueDate = Clock.Now;

            var actorInvoice = new ActorInvoice
            {
                TenantId = dedicatedDynamicActorInvoice.TenantId,
                ShipperActorId = dedicatedDynamicActorInvoice.ShipperActorId,
                DueDate = dueDate,
                IsPaid = false,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                ActorInvoiceChannel = ActorInvoiceChannel.Dedicated,
                Note = dedicatedDynamicActorInvoice.Notes,
                TaxVat = tax
            };
            //await _invoiceRepository.InsertAsync(invoice);
            actorInvoice.Id = await _actorInvoiceRepository.InsertAndGetIdAsync(actorInvoice);
            dedicatedDynamicActorInvoice.ActorInvoiceId = actorInvoice.Id;
            //dedicatedDynamicInvoice.ShippingRequest.IsShipperHaveInvoice = true;
            dedicatedDynamicActorInvoice.DedicatedDynamicActorInvoiceItems.ForEach(x =>
            x.DedicatedShippingRequestTruck.ActorInvoiceId = actorInvoice.Id);

            //Generate invoice number
            await GenerateInvoiceNumber(actorInvoice);
        }


        public async Task GenerateDedicatedDynamicActorSubmitInvoice(DedicatedDynamicActorInvoice dedicatedDynamicActorInvoice)
        {
            decimal subTotalAmount = dedicatedDynamicActorInvoice.SubTotalAmount;
            var tax = 15;

            decimal vatAmount = dedicatedDynamicActorInvoice.VatAmount;
            decimal totalAmount = dedicatedDynamicActorInvoice.TotalAmount;

            DateTime dueDate = Clock.Now;

            var actorInvoice = new ActorSubmitInvoice
            {
                TenantId = dedicatedDynamicActorInvoice.TenantId,
                CarrierActorId = dedicatedDynamicActorInvoice.CarrierActorId,
                DueDate = dueDate,
                TotalAmount = totalAmount,
                VatAmount = vatAmount,
                SubTotalAmount = subTotalAmount,
                TaxVat = tax,
                ActorInvoiceChannel = ActorInvoiceChannel.Dedicated
            };
            //await _invoiceRepository.InsertAsync(invoice);
            actorInvoice.Id = await _actorSubmitInvoiceRepository.InsertAndGetIdAsync(actorInvoice);
            dedicatedDynamicActorInvoice.ActorSubmitInvoiceId = actorInvoice.Id;
            //dedicatedDynamicInvoice.ShippingRequest.IsShipperHaveInvoice = true;
            dedicatedDynamicActorInvoice.DedicatedDynamicActorInvoiceItems.ForEach(x =>
            x.DedicatedShippingRequestTruck.ActorSubmitInvoiceId = actorInvoice.Id);

            //Generate invoice reference number
            await GenerateSubmitReferenceNumber(actorInvoice);
        }

        #region helper
        private async Task GenerateInvoiceNumber(ActorInvoice invoice)
        {
            var lastInvoice = (await _actorInvoiceRepository.GetAll().OrderByDescending(x => x.InvoiceNumber).FirstOrDefaultAsync())?.InvoiceNumber;
            var currentInvoice = Clock.Now.ToString("yy");

            if (lastInvoice != null) { currentInvoice += (Convert.ToInt32(lastInvoice.Substring(3)) + 1).ToString("0000000"); }
            else { currentInvoice += "0000001"; }
            if (await _featureChecker.IsEnabledAsync(AppFeatures.ShipperClients) && await _featureChecker.IsEnabledAsync(AppFeatures.CarrierClients))
            {
                invoice.InvoiceNumber = "B" + currentInvoice;
            }
            else if (await _featureChecker.IsEnabledAsync(AppFeatures.ShipperClients))
            {
                invoice.InvoiceNumber = "S" + currentInvoice;
            }
        }

        private async Task GenerateSubmitReferenceNumber(ActorSubmitInvoice invoice)
        {
            var lastInvoice = (await _actorSubmitInvoiceRepository.GetAll().OrderByDescending(x => x.ReferencNumber).FirstOrDefaultAsync())?.ReferencNumber;
            var currentInvoice = Clock.Now.ToString("yy");

            if (lastInvoice != null) { currentInvoice += (Convert.ToInt32(lastInvoice.Substring(3)) + 1).ToString("0000000"); }
            else { currentInvoice += "0000001"; }

            //person who Generate carrier actor invoices is always broker
            invoice.ReferencNumber = "B" + currentInvoice;
        }

        #endregion
    }
}
