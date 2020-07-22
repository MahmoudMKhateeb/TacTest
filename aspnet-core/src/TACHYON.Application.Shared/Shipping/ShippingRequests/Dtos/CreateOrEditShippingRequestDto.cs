
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Routs.RoutSteps.Dtos;

namespace TACHYON.Shipping.ShippingRequests.Dtos
{
    public class CreateOrEditShippingRequestDto : EntityDto<long?>
    {

        public decimal Vas { get; set; }


        public Guid? TrucksTypeId { get; set; }

        public int? TrailerTypeId { get; set; }

        public int? RouteId { get; set; }

        public CreateOrEditGoodsDetailDto CreateOrEditGoodsDetailDto { get; set; }

        public List<CreateOrEditRoutStepDto> CreateOrEditRoutStepDtoList { get; set; }

    }
}