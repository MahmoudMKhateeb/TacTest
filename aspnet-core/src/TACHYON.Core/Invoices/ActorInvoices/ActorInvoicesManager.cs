using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Actors;
using TACHYON.Dto;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips;

namespace TACHYON.Invoices.ActorInvoices
{
    public class ActorInvoicesManager :TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTripRepository;
        private readonly IRepository<Actor> _actorRepository;

        private readonly IRepository<ActorInvoice,long> _actorInvoiceRepository;
        private readonly IRepository<ActorSubmitInvoice, long> _actorSubmitInvoiceRepository;

        public ActorInvoicesManager(IRepository<ShippingRequestTrip> shippingRequestTripRepository, IRepository<Actor> actorRepository,
            IRepository<ActorInvoice, long> actorInvoiceRepository,
            IRepository<ActorSubmitInvoice, long> actorSubmitInvoiceRepository)
        {
            this._shippingRequestTripRepository = shippingRequestTripRepository;
            this._actorRepository = actorRepository;
            this._actorInvoiceRepository = actorInvoiceRepository;
            _actorSubmitInvoiceRepository = actorSubmitInvoiceRepository;
        }

        public async Task<bool> BuildActorShipperInvoices(int  actorId, List<SelectItemDto> SelectedTrips )
        {

            var actor = await _actorRepository.FirstOrDefaultAsync(actorId);

            List<ShippingRequestTrip> trips = await GetAllShipperActorUnInvoicedTrips(actorId, SelectedTrips);

            var dueDate = Clock.Now.AddDays(actor.InvoiceDueDays);




             if (trips != null && trips.Count() > 0)
             {
                 decimal totalAmount = (decimal)trips.Sum(r => r.ShippingRequestFk.ActorShipperPrice.TotalAmountWithCommission + r.ShippingRequestTripVases.Select(x=> x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.TotalAmountWithCommission));
                 decimal vatAmount = (decimal)trips.Sum(r => r.ShippingRequestFk.ActorShipperPrice.VatAmountWithCommission + r.ShippingRequestTripVases.Select(x=> x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.VatAmountWithCommission));
                 decimal subTotalAmount = (decimal)trips.Sum(r => r.ShippingRequestFk.ActorShipperPrice.SubTotalAmountWithCommission + r.ShippingRequestTripVases.Select(x=> x.ShippingRequestVasFk).Sum(v => v.ActorShipperPrice.SubTotalAmountWithCommission));
                 decimal taxVat = (decimal)trips.Select(x=> x.ShippingRequestFk).FirstOrDefault().ActorShipperPrice.TaxVat;


                var actorInvoice = new ActorInvoice()
                {
                    TenantId = actor.TenantId,
                    ShipperActorId = actor.Id,
                    DueDate = dueDate,
                    TotalAmount = totalAmount,
                    VatAmount = vatAmount,
                    SubTotalAmount = subTotalAmount,
                    TaxVat = taxVat,
                    Trips = trips
                };

                await _actorInvoiceRepository.InsertAsync(actorInvoice);

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

            return await _shippingRequestTripRepository.GetAll()
             .Include(trip => trip.ShippingRequestTripVases)
             .ThenInclude(x=> x.ShippingRequestVasFk)
             .ThenInclude(v => v.ActorShipperPrice)
             .Include(trip => trip.ShippingRequestFk)
             .ThenInclude(request => request.ActorShipperPrice)
             .WhereIf(trips != null , x=> trips.Select(y=>y.Id).Select(int.Parse).Contains(x.Id))
             .Where(trip => trip.ShippingRequestFk.ShipperActorId == actorId)
             .Where(trip => trip.ShippingRequestFk.ActorShipperPriceId.HasValue)
             .Where(trip => !trip.IsActorShipperHaveInvoice)
             .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered ||
             trip.Status == Shipping.Trips.ShippingRequestTripStatus.DeliveredAndNeedsConfirmation).ToListAsync();
        }

        public async Task<bool> BuildActorCarrierInvoices(int actorId, List<SelectItemDto> SelectedTrips)
        {

            var actor = await _actorRepository.FirstOrDefaultAsync(actorId);

            List<ShippingRequestTrip> trips = await GetAllCarrierActorUnInvoicedTrips(actorId, SelectedTrips);

            var dueDate = Clock.Now.AddDays(actor.InvoiceDueDays);


            if (trips != null && trips.Count>0)
            {
                decimal totalAmount = (decimal)trips.Sum(r => r.ShippingRequestFk.ActorCarrierPrice.SubTotalAmount + r.ShippingRequestFk.ActorCarrierPrice.VatAmount + r.ShippingRequestTripVases.Select(x=> x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount + v.ActorCarrierPrice.VatAmount));
                decimal vatAmount = (decimal)trips.Sum(r => r.ShippingRequestFk.ActorCarrierPrice.VatAmount + r.ShippingRequestTripVases.Select(x=> x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount));
                decimal subTotalAmount = (decimal)trips.Sum(r => r.ShippingRequestFk.ActorCarrierPrice.SubTotalAmount + r.ShippingRequestTripVases.Select(x=> x.ShippingRequestVasFk).Sum(v => v.ActorCarrierPrice.SubTotalAmount));
                decimal taxVat = (decimal)trips.Select(x=> x.ShippingRequestFk).FirstOrDefault().ActorCarrierPrice.TaxVat;



                var actorSubmitInvoice = new ActorSubmitInvoice()
                {
                    TenantId = actor.TenantId,
                    CarrierActorId = actor.Id,
                    DueDate = dueDate,
                    TotalAmount = totalAmount,
                    VatAmount = vatAmount,
                    SubTotalAmount = subTotalAmount,
                    TaxVat = taxVat,
                    Trips = trips
                };

                await _actorSubmitInvoiceRepository.InsertAsync(actorSubmitInvoice);

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
            
            return await _shippingRequestTripRepository.GetAll()
             .Include(trip => trip.ShippingRequestTripVases)
             .ThenInclude(x=> x.ShippingRequestVasFk)
             .ThenInclude(v => v.ActorCarrierPrice)
             .Include(trip => trip.ShippingRequestFk)
             .ThenInclude(request => request.ActorCarrierPrice)
             .WhereIf(trips != null, x => trips.Select(y => y.Id).Select(int.Parse).Contains(x.Id))
             .Where(trip => trip.ShippingRequestFk.CarrierActorId == actorId)
             .Where(trip => trip.ShippingRequestFk.ActorCarrierPriceId.HasValue)
             .Where(trip => !trip.IsActorCarrierHaveInvoice)
             .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered ||
             trip.Status == Shipping.Trips.ShippingRequestTripStatus.DeliveredAndNeedsConfirmation)
             .ToListAsync();
        }
    }
}
