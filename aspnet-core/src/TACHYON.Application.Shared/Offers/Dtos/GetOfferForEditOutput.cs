using Abp.Application.Services.Dto;
using System;
using System.ComponentModel.DataAnnotations;

namespace TACHYON.Offers.Dtos
{
    public class GetOfferForEditOutput
    {
        public CreateOrEditOfferDto Offer { get; set; }

        public string TrucksTypeDisplayName { get; set; }

        public string TrailerTypeDisplayName { get; set; }

        public string GoodCategoryDisplayName { get; set; }

        public string RouteDisplayName { get; set; }


    }
}