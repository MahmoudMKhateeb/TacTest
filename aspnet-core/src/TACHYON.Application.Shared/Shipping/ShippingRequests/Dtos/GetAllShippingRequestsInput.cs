using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class GetAllShippingRequestsInput : PagedAndSortedResultRequestDto ,IShouldNormalize
    {
        public string Filter { get; set; }

        public decimal? MaxVasFilter { get; set; }
        public decimal? MinVasFilter { get; set; }

        public  bool? IsBid { get; set; }

        public  bool? IsTachyonDeal { get; set; }

        public bool? IsTachyonDealer { get; set; }
        public ShippingRequestStatus? Status { get; set; }

        public bool? IsPricedWihtoutTrips { get; set; }

        public void Normalize()
        {

            if (string.IsNullOrWhiteSpace(Sorting)) return;
            if (Sorting.Contains("originalCityName"))
            {
                Sorting = Sorting.Replace("originalCityName", "OriginCityFk.DisplayName");
            }
            else if (Sorting.Contains("destinationCityName"))
            {
                Sorting = Sorting.Replace("destinationCityName", "DestinationCityFk.DisplayName");
            }
            else if (Sorting.Contains("totalBidsCount"))
            {
                Sorting = Sorting.Replace("totalBidsCount", "TotalBids");
            }
            else if (Sorting.Contains("routeType"))
            {
                Sorting=Sorting.Replace("routeType", "RouteTypeId");
            }

     

        }
    }
}