using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.EntityTemplates
{
    public class DedicatedShippingRequestTemplateDto : EntityDto<long?>
    {
        // step 1
        public TimeUnit RentalDurationUnit { get; set; }
        public int NumberOfTrucks { get; set; }
        public double ExpectedMileage { get; set; }
        public string ServiceAreaNotes { get; set; }
        /// <summary>
        /// Number of duration per time unit
        /// </summary>
        [Required]
        [Range(1,100)]
        public int RentalDuration { get; set; }
        [Required] public int? GoodCategoryId { get; set; }
        public int PackingTypeId { get; set; }
        public int? TransportTypeId { get; set; }
        public long TrucksTypeId { get; set; }
        public int? CapacityId { get; set; }
        public string OtherGoodsCategoryName { get; set; }
        public string OtherTransportTypeName { get; set; }
        public string OtherTrucksTypeName { get; set; }
        public string OtherPackingTypeName { get; set; }
        public int CountryId { get; set; }
        public List<ShippingRequestDestinationCitiesDto> ShippingRequestDestinationCities { get; set; }
        
        
        // base step 1
        [JsonProperty("shipperId")]
        public int TenantId { get; set; }

        public bool IsBid { get; set; }

        
        public bool IsTachyonDeal { get; set; }
        public bool IsDirectRequest { get; set; }
        
        public int? CarrierTenantIdForDirectRequest { get; set; }

        public int ShippingTypeId { get; set; }
        
        public int? ShipperActorId { get; set; }
        public int? CarrierActorId { get; set; }

        public bool IsInternalBrokerRequest { get; set; }

        
        // step 2
        
        public List<CreateOrEditShippingRequestVasListDto> ShippingRequestVasList { get; set; }
        public bool IsDrafted { get; set; }
        public int DraftStep { get; set; }
    }
}