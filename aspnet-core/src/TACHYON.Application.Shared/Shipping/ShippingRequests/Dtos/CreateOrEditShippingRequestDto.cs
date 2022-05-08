using Abp.Application.Services.Dto;
using Abp.Localization;
using Abp.Runtime.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Vases.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CreateOrEditShippingRequestDto : EntityDto<long?>, ICustomValidate, IHasVasListDto,
        IShippingRequestDtoHaveOthersName
    {
        public virtual bool IsBid { get; set; }

        //Add Bid details If IsBid equals True
        public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }

        public virtual bool IsTachyonDeal { get; set; }
        public bool IsDirectRequest { get; set; }
        [JsonIgnore] public ShippingRequestType RequestType { get; set; }

        /// <summary>
        /// if we clone request
        /// </summary>
        public long? FatherShippingRequestId { get; set; }

        /// <summary>
        /// if assigned to carrier
        /// </summary>
        public int? CarrierTenantId { get; set; }

        public virtual int? TransportTypeId { get; set; }

        public virtual long TrucksTypeId { get; set; }

        public virtual int? CapacityId { get; set; }

        public int? GoodCategoryId { get; set; }
        [Range(1, 20)] public int? NumberOfDrops { get; set; }
        public bool StageOneFinish { get; set; }
        public bool StageTowFinish { get; set; }
        public bool StageThreeFinish { get; set; }

        [Required] public DateTime StartTripDate { get; set; }
        public DateTime? EndTripDate { get; set; }
        [Range(1, 1000)] public int NumberOfTrips { get; set; }
        public int PackingTypeId { get; set; }
        public int NumberOfPacking { get; set; }
        public double TotalWeight { get; set; }
        public int ShippingTypeId { get; set; }


        //Route

        public ShippingRequestRouteType RouteTypeId { get; set; }


        [Required] public virtual int OriginCityId { get; set; }
        [Required] public virtual int DestinationCityId { get; set; }

        public string OtherGoodsCategoryName { get; set; }
        public string OtherTransportTypeName { get; set; }
        public string OtherTrucksTypeName { get; set; }
        public string OtherPackingTypeName { get; set; }

        //VasList
        public List<CreateOrEditShippingRequestVasListDto> ShippingRequestVasList { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            var localization = context.IocResolver
                .Resolve<ILocalizationManager>();

            switch (this.RouteTypeId)
            {
                case ShippingRequestRouteType.SingleDrop:
                    this.NumberOfDrops = 1;
                    break;
                //case ShippingRequestRouteType.TwoWay:
                //    this.NumberOfDrops = 2;
                //    break;
                default:
                    if (this.NumberOfDrops < 2)
                    {
                        var errorMessage = localization.GetString(
                            TACHYONConsts.LocalizationSourceName,
                            "TheNumberOfDropsMustHigerOrEqualTwo");
                        context.Results.Add(new ValidationResult(errorMessage));
                    }

                    break;
            }

            if (IsBid)
            {
                RequestType = ShippingRequestType.Marketplace;
            }
            else if (IsTachyonDeal)
            {
                RequestType = ShippingRequestType.TachyonManageService;
            }
            else
            {
                RequestType = ShippingRequestType.DirectRequest;
            }
        }
    }
}