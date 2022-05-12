using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Rating;
using TACHYON.Rating.dtos;

namespace TACHYON.AutoMapper.Rating
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<CreateCarrierRatingByShipperDto, RatingLog>().ReverseMap();

            CreateMap<CreateDriverRatingDtoByReceiverDto, RatingLog>().ReverseMap();

            CreateMap<CreateDeliveryExpRateByReceiverDto, RatingLog>().ReverseMap();

            CreateMap<CreateFacilityRateByDriverDto, RatingLog>().ReverseMap();

            CreateMap<CreateShippingExpRateByDriverDto, RatingLog>().ReverseMap();

            CreateMap<CreateShipperRateByCarrierDto, RatingLog>().ReverseMap();
        }
    }
}