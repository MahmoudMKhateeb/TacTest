using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TACHYON.PriceOffers;
using TACHYON.Shipping.ShippingRequestTrips;
using System.Linq.Dynamic.Core;
using System.Linq;

namespace TACHYON.Shipping.Trips
{
   public class ShippingRequestsTripManager: TACHYONDomainServiceBase
    {
        private readonly IRepository<ShippingRequestTrip> _shippingRequestTrip;
        private readonly PriceOfferManager _priceOfferManager;

        public ShippingRequestsTripManager(IRepository<ShippingRequestTrip> shippingRequestTrip, PriceOfferManager priceOfferManager)
        {
            _shippingRequestTrip = shippingRequestTrip;
            _priceOfferManager = priceOfferManager;
        }


        public async Task GeneratePrices(ShippingRequestTrip trip)
        {
            DisableTenancyFilters();
            var offer = await _priceOfferManager.GetOffercceptedByShippingRequestId(trip.ShippingRequestId);

            trip.CommissionType = offer.CommissionType;
            trip.SubTotalAmount = offer.ItemPrice;
            trip.VatAmount = offer.ItemVatAmount;
            trip.TotalAmount = offer.ItemTotalAmount;
            trip.SubTotalAmountWithCommission = offer.ItemSubTotalAmountWithCommission;
            trip.VatAmountWithCommission = offer.ItemVatAmountWithCommission;
            trip.TotalAmountWithCommission = offer.ItemTotalAmountWithCommission;
            trip.CommissionAmount = offer.ItemCommissionAmount;
            trip.CommissionPercentageOrAddValue = offer.CommissionPercentageOrAddValue;
            trip.TaxVat = offer.TaxVat;
            if (trip.ShippingRequestTripVases == null || trip.ShippingRequestTripVases.Count==0) return;
            foreach(var vas in trip.ShippingRequestTripVases)
            {
                var item = offer.PriceOfferDetails.FirstOrDefault(x => x.SourceId == vas.ShippingRequestVasId && x.PriceType== PriceOfferType.Vas);
                vas.CommissionType = item.CommissionType;
                vas.SubTotalAmount = item.ItemPrice;
                vas.VatAmount = item.ItemVatAmount;
                vas.TotalAmount = item.ItemTotalAmount;
                vas.SubTotalAmountWithCommission = item.ItemSubTotalAmountWithCommission;
                vas.VatAmountWithCommission = item.ItemVatAmountWithCommission;
                vas.TotalAmountWithCommission = item.ItemTotalAmountWithCommission;
                vas.CommissionPercentageOrAddValue = item.CommissionPercentageOrAddValue;
                vas.CommissionAmount = item.ItemCommissionAmount;
                vas.Quantity = 1;
            }

        }
    }
}
