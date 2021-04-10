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
            CreateMap<ShippingRequestTripRejectReason, ShippingRequestTripRejectReasonListDto>();
            CreateMap<ShippingRequestTripRejectReason, CreateOrEditShippingRequestTripRejectReasonDto>().ReverseMap();

            
        }
    }
}
