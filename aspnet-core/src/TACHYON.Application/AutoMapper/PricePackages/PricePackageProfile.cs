using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TACHYON.PriceOffers;
using TACHYON.PricePackages;
using TACHYON.PricePackages.Dto.NormalPricePackage;

namespace TACHYON.AutoMapper.PricePackages
{
    public class PricePackageProfile : Profile
    {
        public PricePackageProfile()
        {
            //NormalPricePackage

            CreateMap<NormalPricePackage, NormalPricePackageDto>()
                .ForMember(src => src.PricePerExtraDrop, opt => opt.MapFrom(des => des.PricePerExtraDrop.HasValue ? des.PricePerExtraDrop.ToString() : "---"))
                .ForMember(src => src.TenantName, opt => opt.MapFrom(des => des.Tenant.Name))
                .ForMember(src => src.TruckType, opt => opt.MapFrom(des => des.TrucksTypeFk.DisplayName))
                .ForMember(src => src.Origin, opt => opt.MapFrom(des => des.OriginCityFK.DisplayName))
                .ForMember(src => src.Destination, opt => opt.MapFrom(des => des.DestinationCityFK.DisplayName));

            CreateMap<NormalPricePackage, NormalPricePackageProfileDto>()
                .ForMember(src => src.TruckType, opt => opt.MapFrom(des => des.TrucksTypeFk.DisplayName))
                .ForMember(src => src.Origin, opt => opt.MapFrom(des => des.OriginCityFK.DisplayName))
                .ForMember(src => src.Destination, opt => opt.MapFrom(des => des.DestinationCityFK.DisplayName));

            CreateMap<NormalPricePackage, PricePackageOfferDto>()
                .ForMember(src => src.TruckType, opt => opt.MapFrom(des => des.TrucksTypeFk.DisplayName))
                .ForMember(src => src.Origin, opt => opt.MapFrom(des => des.OriginCityFK.DisplayName))
                .ForMember(src => src.Destination, opt => opt.MapFrom(des => des.DestinationCityFK.DisplayName))
                .ForMember(src => src.NormalPricePackageId, opt => opt.MapFrom(des => des.Id));

            CreateMap<CreateOrEditNormalPricePackageDto, NormalPricePackage>().ReverseMap();
            CreateMap<PriceOffer, PricePackageOfferDto>()
                .ForMember(src => src.Items, opt => opt.MapFrom(des => des.PriceOfferDetails))
                .ReverseMap();

            CreateMap<PricePackageOffer, PriceOffer>()
               .ForMember(src => src.PriceOfferDetails, opt => opt.MapFrom(des => des.Items))
               .ReverseMap();
            CreateMap<PriceOfferDetail, PricePackageOfferItem>()
               .ReverseMap();
            CreateMap<PriceOfferDetail, PricePackageOfferItemDto>().ReverseMap();
            CreateMap<PricePackageOffer, PricePackageOfferDto>()
                .ReverseMap();
            CreateMap<PricePackageOfferItemDto, PricePackageOfferItem>().ReverseMap();
        }
    }
}