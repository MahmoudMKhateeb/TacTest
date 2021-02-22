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


        public void Normalize()
        {
            if (Sorting == "GoodsCategoryName")
            {
                Sorting = "GoodCategoryFk.DisplayName";
            }
            else if(Sorting== "OriginalCityName")
            {
                Sorting = "RouteFk.OriginCityFk.DisplayName";
            }
            else if (Sorting == "DestinationCityName")
            {
                Sorting = "RouteFk.DestinationCityFk.DisplayName";
            }
            else if (Sorting == "DriverName")
            {
                Sorting = "AssignedDriverUserFk.Name";
            }
            else if (Sorting == "RoutTypeName")
            {
                Sorting = "RoutTypeFk.DisplayName";
            }
            else if (Sorting == "ShippingRequestStatusName")
            {
                Sorting = "ShippingRequestStatusFk.DisplayName";
            }
            else if (Sorting == "TruckTypeDisplayName")
            {
                Sorting = "AssignedTruckFk.TrucksTypeFk.DisplayName";
            }
        }
    }
}