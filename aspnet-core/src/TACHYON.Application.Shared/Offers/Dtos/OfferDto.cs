
using Abp.Application.Services.Dto;
using System;

namespace TACHYON.Offers.Dtos
{
    public class OfferDto : EntityDto
    {
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }


        public Guid TrucksTypeId { get; set; }

        public int TrailerTypeId { get; set; }

        public int? GoodCategoryId { get; set; }

        public int RouteId { get; set; }


    }
}