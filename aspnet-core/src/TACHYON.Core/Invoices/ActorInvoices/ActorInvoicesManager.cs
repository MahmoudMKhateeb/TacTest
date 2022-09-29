using Abp.Domain.Repositories;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TACHYON.Actors;
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

        public async Task<bool> BuildActorShipperInvoices(int  actorId )
        {

            var actor =await  _actorRepository.FirstOrDefaultAsync(actorId);

            var trips = await  _shippingRequestTripRepository.GetAll()
             .Include(trip => trip.ShippingRequestTripVases)
             .ThenInclude(v => v.ActorShipperPriceFk)
             .Include(trip => trip.ShippingRequestFk)
             .Include(trip => trip.ActorShipperPriceFk)
            
             .Where(trip => trip.ShippingRequestFk.ShipperActorId == actorId)
             .Where(trip => trip.ActorShipperPriceFk != null )
             .Where(trip=> !trip.ActorShipperPriceFk.IsActorShipperHaveInvoice)
             .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered)
             .ToListAsync();


             var dueDate = Clock.Now.AddDays(actor.InvoiceDueDays);


             if (trips != null && trips.Count() > 0)
             {
                 decimal totalAmount = (decimal)trips.Sum(r => r.ActorShipperPriceFk.TotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.ActorShipperPriceFk.TotalAmountWithCommission));
                 decimal vatAmount = (decimal)trips.Sum(r => r.ActorShipperPriceFk.VatAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.ActorShipperPriceFk.VatAmountWithCommission));
                 decimal subTotalAmount = (decimal)trips.Sum(r => r.ActorShipperPriceFk.SubTotalAmountWithCommission + r.ShippingRequestTripVases.Sum(v => v.ActorShipperPriceFk.SubTotalAmountWithCommission));
                 decimal taxVat = (decimal)trips.FirstOrDefault().ActorShipperPriceFk.TaxVat;



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

                foreach(var trip in trips)
                {
                    trip.ActorShipperPriceFk.IsActorShipperHaveInvoice = true;
                }
                return true;
             }
            return false;
        }



        public async Task<bool> BuildActorCarrierInvoices(int actorId)
        {

            var actor = await _actorRepository.FirstOrDefaultAsync(actorId);

            var trips = await _shippingRequestTripRepository.GetAll()
             .Include(trip => trip.ShippingRequestTripVases)
             .ThenInclude(v => v.ActorCarrierPriceFk)
             .Include(trip => trip.ShippingRequestFk)
             .Include(trip => trip.ActorCarrierPriceFk)

             .Where(trip => trip.ShippingRequestFk.CarrierActorId == actorId)
             .Where(trip => trip.ActorCarrierPriceFk != null)
             .Where(trip=> !trip.ActorCarrierPriceFk.IsActorCarrierHaveInvoice)
             .Where(trip => trip.Status == Shipping.Trips.ShippingRequestTripStatus.Delivered)
             .ToListAsync();


            var dueDate = Clock.Now.AddDays(actor.InvoiceDueDays);


            if (trips != null && trips.Count()>0)
            {
                decimal totalAmount = (decimal)trips.Sum(r => r.ActorCarrierPriceFk.SubTotalAmount + r.ActorCarrierPriceFk.VatAmount + r.ShippingRequestTripVases.Sum(v => v.ActorCarrierPriceFk.SubTotalAmount + v.ActorCarrierPriceFk.VatAmount));
                decimal vatAmount = (decimal)trips.Sum(r => r.ActorCarrierPriceFk.VatAmount + r.ShippingRequestTripVases.Sum(v => v.ActorCarrierPriceFk.SubTotalAmount));
                decimal subTotalAmount = (decimal)trips.Sum(r => r.ActorCarrierPriceFk.SubTotalAmount + r.ShippingRequestTripVases.Sum(v => v.ActorCarrierPriceFk.SubTotalAmount));
                decimal taxVat = (decimal)trips.FirstOrDefault().ActorCarrierPriceFk.TaxVat;



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

                foreach(var trip in trips)
                {
                    trip.ActorCarrierPriceFk.IsActorCarrierHaveInvoice = true;
                }
                return true;
            }
            return false;
        }
    }
}
