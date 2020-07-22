
using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Offers.Dtos
{
    public class CreateOrEditOfferDto : EntityDto<int?>
    {

        [StringLength(OfferConsts.MaxDisplayNameLength, MinimumLength = OfferConsts.MinDisplayNameLength)]
        public string DisplayName { get; set; }


        [StringLength(OfferConsts.MaxDescriptionLength, MinimumLength = OfferConsts.MinDescriptionLength)]
        public string Description { get; set; }


        public decimal Price { get; set; }


        public Guid TrucksTypeId { get; set; }

        public int TrailerTypeId { get; set; }

        public int? GoodCategoryId { get; set; }

        public int RouteId { get; set; }


    }
}