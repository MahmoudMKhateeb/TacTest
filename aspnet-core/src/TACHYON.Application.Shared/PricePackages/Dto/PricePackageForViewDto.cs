﻿using Abp.Application.Services.Dto;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.PricePackages.Dto
{
    
    public class PricePackageForViewDto : EntityDto<long>
    {
        public string OriginCity { get; set; }

        public string DestinationCity { get; set; }

        public string CompanyName { get; set; }

        public int CompanyTenantId { get; set; }
        
        public decimal FinalPrice { get; set; }

        public string PricePackageId { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// This property to let client side know if this price package is sent offer by it
        /// if HasOffer true (that's mean the TMS send an offer to shipper and accept it on behalf)
        /// </summary>
        public bool HasOffer { get; set; }
        
        public bool HasDirectRequest { get; set; }

        public bool IsRequestPriced { get; set; }

        public string TransportType { get; set; }

        public PricePackageUsageType UsageType { get; set; }
        public string TruckType { get; set; }

        public int? AppendixId { get; set; }

        public bool HasParentOffer { get; set; }
    }
}