using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;

namespace TACHYON.AutoMapper.Shipping.Trips
{
    public class ShippingRequestTripRejectReasonProfile : Profile
    {
        public ShippingRequestTripRejectReasonProfile()
        {
            CreateMap<ShippingRequestTripRejectReasonTranslation, ShippingRequestTripRejectReasonTranslationDto>();
            CreateMap<ShippingRequestTripRejectReason, CreateOrEditShippingRequestTripRejectReasonDto>()
                 .ForMember(dst => dst.Translations, opt => opt.MapFrom(src => src.Translations)).ReverseMap();

            
        }
    }
}
