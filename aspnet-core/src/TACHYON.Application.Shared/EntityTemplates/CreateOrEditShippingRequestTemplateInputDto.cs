using Abp.Application.Services.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Vases.Dtos;

namespace TACHYON.EntityTemplates
{
    public class CreateOrEditShippingRequestTemplateInputDto : EntityDto<long?>, IHasVasListDto,IShippingRequestDtoHaveOthersName
    {
        // step 1
        public int? ShipperId { get; set; }

        public virtual bool IsBid { get; set; }
        
        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }
        public virtual bool IsTachyonDeal { get; set; }
        public bool IsDirectRequest { get; set; }
        
        public int? CarrierTenantIdForDirectRequest { get; set; }

        [Required] public int ShippingTypeId { get; set; }
        [Required] public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        public string ShipperReference { get; set; }
        public string ShipperInvoiceNo { get; set; }

        [JsonIgnore] public ShippingRequestType RequestType { get; set; }
        
        // step 2 
        [Required] public ShippingRequestRouteType RouteTypeId { get; set; }

        [Required] public int OriginCityId { get; set; }

        [Required] public int DestinationCityId { get; set; }
        
        public int OriginCountryId { get; set; }

        public int DestinationCountryId { get; set; }
        
        public int NumberOfDrops { get; set; }
        [Required] public int NumberOfTrips { get; set; }

        // step 3
        [Required] public int? GoodCategoryId { get; set; }
        public double TotalWeight { get; set; }
        public int PackingTypeId { get; set; }
        public int NumberOfPacking { get; set; }
        public virtual int? TransportTypeId { get; set; }
        public virtual long TrucksTypeId { get; set; }
        public virtual int? CapacityId { get; set; }
        public bool IsDrafted { get; set; }
        public int DraftStep { get; set; }
        public string OtherGoodsCategoryName { get; set; }
        public string OtherTransportTypeName { get; set; }
        public string OtherTrucksTypeName { get; set; }

        public string OtherPackingTypeName { get; set; }

        // step 4
        public List<CreateOrEditShippingRequestVasListDto> ShippingRequestVasList { get; set; }
        
        [JsonIgnore]
        public int TenantId { get; set; }
    }
}