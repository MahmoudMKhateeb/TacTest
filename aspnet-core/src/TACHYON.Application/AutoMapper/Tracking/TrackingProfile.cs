﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Tracking.Dto;

namespace TACHYON.AutoMapper.Tracking
{
    public class TrackingProfile : Profile
    {

        public TrackingProfile()
        {
            CreateMap<ShippingRequestTrip, TrackingListDto>()
             .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ShippingRequestFk.CarrierTenantFk.Name))
             .ForMember(dst => dst.RouteTypeId, opt => opt.MapFrom(src => src.ShippingRequestFk.RouteTypeId))
            .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk.FullName))
            .ForMember(dst => dst.DriverImageProfile, opt => opt.MapFrom(src => src.AssignedDriverUserFk.ProfilePictureId))
            .ForMember(dst => dst.Origin, opt => opt.MapFrom(src => src.OriginFacilityFk.Address))
            .ForMember(dst => dst.Destination, opt => opt.MapFrom(src => src.DestinationFacilityFk.Address))
            ;
        }
    }
}