
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutSteps.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CreateOrEditShippingRequestDto : EntityDto<long?>
    {
        public decimal Vas { get; set; }

        public List<CreateOrEditRoutStepDto> CreateOrEditRoutStepDtoList { get; set; }

        public virtual bool IsBid { get; set; }

        public virtual bool IsTachyonDeal { get; set; }

        /// <summary>
        /// if we clone request
        /// </summary>
        public long? FatherShippingRequestId { get; set; }

        /// <summary>
        /// if assigned to carrier
        /// </summary>
        public int? CarrierTenantId { get; set; }

        public long? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }

        public int? GoodCategoryId { get; set; }

        public int? NumberOfDrops { get; set; }
        public bool StageOneFinish { get; set; }
        public bool StageTowFinish { get; set; }
        public bool StageThreeFinish { get; set; }


        //Route
        public CreateOrEditRouteDto CreateOrEditRouteDto { get; set; }





    }
}