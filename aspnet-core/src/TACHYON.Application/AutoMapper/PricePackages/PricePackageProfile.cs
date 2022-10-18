using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TACHYON.PriceOffers;
using TACHYON.PricePackages;
using TACHYON.PricePackages.Dto.NormalPricePackage;
using TACHYON.PricePackages.Dto.PricePackageProposals;
using TACHYON.PricePackages.Dto.TmsPricePackages;
using TACHYON.PricePackages.PricePackageProposals;

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
            CreateMap<TmsPricePackage, TmsPricePackageListDto>()
                .ForMember(x=> x.OriginCity,x=> x.MapFrom(i=> i.OriginCity.DisplayName))
                .ForMember(x=> x.DestinationCity,x=> x.MapFrom(i=> i.DestinationCity.DisplayName))
                .ForMember(x=> x.Shipper,x=> x.MapFrom(i=> i.Shipper.Name))
                .ForMember(x=> x.TruckType,x=> x.MapFrom(i=> i.TrucksTypeFk.Key))
                .ForMember(x=> x.TransportType,x=> x.MapFrom(i=> i.TransportTypeFk.Key));
            
            CreateMap<CreateOrEditTmsPricePackageDto, TmsPricePackage>().ReverseMap();
            CreateMap<CreateOrEditProposalDto, PricePackageProposal>()
                .ForMember(x=> x.TmsPricePackages,x=> x.Ignore())
                .AfterMap((dto, proposal) => proposal.Status = ProposalStatus.New);
            CreateMap<PricePackageProposal, CreateOrEditProposalDto>()
                .ForMember(x=>x.TmsPricePackages,x=> x.MapFrom(i=> i.TmsPricePackages.Select(t=> t.Id)));

            CreateMap<PricePackageProposal, ProposalListItemDto>();
            CreateMap<TmsPricePackage, TmsPricePackageSelectItemDto>()
                .ForMember(x=> x.OriginCity,x=> x.MapFrom(i=> i.OriginCity.DisplayName))
                .ForMember(x=> x.DestinationCity,x=> x.MapFrom(i=> i.DestinationCity.DisplayName))
                .ForMember(x=> x.TruckType,x=> x.MapFrom(i=> i.TrucksTypeFk.Key));
        }
    }
}