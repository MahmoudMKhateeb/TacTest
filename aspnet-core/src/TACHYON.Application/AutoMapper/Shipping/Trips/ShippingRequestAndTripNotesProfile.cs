using AutoMapper;
using System;
using TACHYON.Common;
using TACHYON.Routs.RoutPoints;
using TACHYON.Shipping.Notes.Dto;
using TACHYON.Shipping.ShippingRequestAndTripNotes;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.Trips.Accidents.Dto;

namespace TACHYON.AutoMapper.Shipping.Trips
{
    public class ShippingRequestAndTripNotesProfile : Profile
    {
        public ShippingRequestAndTripNotesProfile()
        {
            CreateMap<ShippingRequestAndTripNote, ShippingRequestAndTripNotesDto>()
                .ForMember(dst => dst.TenantName, opt => opt.MapFrom(src => src.TenantFK != null ? src.TenantFK.Name : ""))
                .ReverseMap();

            CreateMap<ShippingRequestAndTripNote, CreateOrEditShippingRequestAndTripNotesDto>()
                .ForMember(r=>r.NoteId , opt=>opt.MapFrom(r=>r.Id))
                .ForMember(r => r.CreateOrEditDocumentFileDto, opt => opt.MapFrom(r => r.DocumentFiles));

            CreateMap<CreateOrEditShippingRequestAndTripNotesDto,ShippingRequestAndTripNote>()
                .ForMember(r => r.DocumentFiles, x => x.Ignore());
        }

    }
}