using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace TACHYON.Shipping.ShippingRequestBids.Dtos
{
    public class GetAllBidsShippingRequestForCarrierInput : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string Filter { get; set; }
        public bool IsMatchingOnly { get; set; }
        public bool IsMyBidsOnly { get; set; }
        public long? TruckTypeId { get; set; }
        public long? TransportType { get; set; }
        public int? CapacityId { get; set; }
        public bool? IsMyAssignedBidsOnly { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(Sorting)) return;
            if (Sorting.Contains("shipperName"))
            {
                Sorting = Sorting.Replace("shipperName", "Tenant.TenancyName");
            }
            else if (Sorting.Contains("truckTypeDisplayName"))
            {
                Sorting = Sorting.Replace("truckTypeDisplayName", "TrucksTypeFk.DisplayName");
            }
            else if (Sorting.Contains("goodCategoryName"))
            {
                Sorting = Sorting.Replace("goodCategoryName", "GoodCategoryFk.DisplayName");
            }
            else if (Sorting.Contains("BasePrice"))
            {
                Sorting = null; //Sorting.Replace("BasePrice", "ShippingRequestBids.BasePrice");
            }
        }
    }


    public class GetAllBidsShippingRequestForShipperInput : PagedAndSortedResultRequestDto
    {
    }
}