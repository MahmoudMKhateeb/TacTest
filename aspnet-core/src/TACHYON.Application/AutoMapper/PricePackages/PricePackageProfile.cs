using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TACHYON.PriceOffers;
using TACHYON.PricePackages;
using TACHYON.PricePackages.Dto.NormalPricePackage;
using TACHYON.PricePackages.Dto.PricePackageAppendices;
using TACHYON.PricePackages.Dto.PricePackageProposals;
using TACHYON.PricePackages.Dto.TmsPricePackages;
using TACHYON.PricePackages.PricePackageAppendices;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.PricePackages.TmsPricePackages;
using TACHYON.Shipping.ShippingRequests;

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
                .ForMember(x=> x.HasProposal,x=> x.MapFrom(i=> i.ProposalId.HasValue))
                .ForMember(x=> x.OriginCity,x=> x.MapFrom(i=> i.OriginCity.DisplayName))
                .ForMember(x=> x.DestinationCity,x=> x.MapFrom(i=> i.DestinationCity.DisplayName))
                .ForMember(x=> x.Shipper,x=> x.MapFrom(i=> i.DestinationTenant.Name))
                .ForMember(x=> x.TruckType,x=> x.MapFrom(i=> i.TrucksTypeFk.Key))
                .ForMember(x=> x.TransportType,x=> x.MapFrom(i=> i.TransportTypeFk.Key));
            
            CreateMap<CreateOrEditTmsPricePackageDto, TmsPricePackage>()
                .AfterMap(((dto, package) =>
                {
                    if (dto.Id.HasValue) return;

                    package.IsActive = false;
                    package.Status = PricePackageOfferStatus.NoOffer;
                    
                })).ReverseMap();
            CreateMap<CreateOrEditProposalDto, PricePackageProposal>()
                .ForMember(x=> x.TmsPricePackages,x=> x.Ignore())
                .AfterMap((dto, proposal) =>
                {
                    if (!dto.Id.HasValue)
                        proposal.Status = ProposalStatus.New;
                });
            CreateMap<PricePackageProposal, CreateOrEditProposalDto>()
                .ForMember(x=>x.TmsPricePackages,x=> x.MapFrom(i=> i.TmsPricePackages.Select(t=> t.Id)));

            CreateMap<PricePackageProposal, ProposalListItemDto>()
                .ForMember(x=> x.ShipperName,x=> x.MapFrom(i=> i.Shipper.Name));
            CreateMap<PricePackageProposal, ProposalForViewDto>()
                .ForMember(x=> x.StatusTitle,x=> x.MapFrom(i=> Enum.GetName(typeof(ProposalStatus),i.Status)))
                .ForMember(x=> x.Shipper,x=> x.MapFrom(i=> i.Shipper.companyName));
            CreateMap<TmsPricePackage, TmsPricePackageSelectItemDto>()
                .ForMember(x=> x.OriginCity,x=> x.MapFrom(i=> i.OriginCity.DisplayName))
                .ForMember(x=> x.DestinationCity,x=> x.MapFrom(i=> i.DestinationCity.DisplayName))
                .ForMember(x=> x.TruckType,x=> x.MapFrom(i=> i.TrucksTypeFk.Key));

            CreateMap<PricePackageAppendix, AppendixListDto>()
                .ForMember(x=> x.Shipper,x=> x.MapFrom(i=> i.Proposal.Shipper.Name))
                .ForMember(x=> x.ContractNumber,x=> x.MapFrom(i=> i.Proposal.Shipper.ContractNumber));
            
            CreateMap<PricePackageAppendix, AppendixForViewDto>()
                .ForMember(x=> x.Version,x=> x.Ignore())
                .ForMember(x=> x.ProposalName,x=> x.MapFrom(i=> i.Proposal.ProposalName))
                .ForMember(x=> x.StatusTitle,x=> x.MapFrom(i=> Enum.GetName(typeof(AppendixStatus),i.Status)));
            CreateMap<CreateOrEditAppendixDto, PricePackageAppendix>()
                .ForMember(x=>x.TmsPricePackages,x=> x.Ignore())
                .ForMember(x=> x.DestinationTenantId,x=> x.MapFrom(i=> i.DestinationCompanyId))
                .AfterMap(((dto, appendix) =>
                {
                    if (!dto.Id.HasValue)
                        appendix.Status = AppendixStatus.New;
                }));
            CreateMap<PricePackageAppendix, CreateOrEditAppendixDto>()
                .ForMember(x => x.DestinationCompanyId, x => x.MapFrom(i => i.Proposal.ShipperId))
                .ForMember(x=> x.TmsPricePackages,x=> x.MapFrom(i=> i.TmsPricePackages.Select(t=> t.Id)));

             CreateMap<TmsPricePackage, TmsPricePackageForViewDto>()
                .ForMember(x => x.CompanyName, x => x.MapFrom(i => i.DestinationTenant.Name))
                .ForMember(x => x.FinalPrice, x => x.MapFrom(i => i.TotalPrice))
                .ForMember(x => x.TruckType, x => x.MapFrom(i => i.TrucksTypeFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null ? i.TrucksTypeFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName : i.TrucksTypeFk.DisplayName))
                .ForMember(x => x.TransportType, x => x.MapFrom(i => i.TransportTypeFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null ? i.TransportTypeFk.Translations.FirstOrDefault(t=> t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName : i.TransportTypeFk.DisplayName))
                .ForMember(x => x.OriginCity, x => x.MapFrom(i =>
                    i.OriginCity.Translations.FirstOrDefault(
                        t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                        ? i.OriginCity.Translations
                            .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                            .TranslatedDisplayName
                        : i.OriginCity.DisplayName))
                .ForMember(x => x.DestinationCity,
                    x => x.MapFrom(i =>
                        i.DestinationCity.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.DestinationCity.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName
                            : i.DestinationCity.DisplayName))
                .ForMember(x => x.CompanyTenantId, x => x.MapFrom(i => i.DestinationTenantId));

             CreateMap<TmsPricePackage, TmsPricePackageForPricingDto>()
                 .ForMember(x => x.TruckType,
                     x => x.MapFrom(i =>
                         i.TrucksTypeFk.Translations.FirstOrDefault(t =>
                             t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                             ? i.TrucksTypeFk.Translations
                                 .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                 .DisplayName
                             : i.TrucksTypeFk.DisplayName))
                 .ForMember(x => x.TransportType,
                     x => x.MapFrom(i =>
                         i.TransportTypeFk.Translations.FirstOrDefault(t =>
                             t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                             ? i.TransportTypeFk.Translations
                                 .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                 .DisplayName
                             : i.TransportTypeFk.DisplayName))
                 .ForMember(x => x.OriginCity, x => x.MapFrom(i =>
                     i.OriginCity.Translations.FirstOrDefault(
                         t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                         ? i.OriginCity.Translations
                             .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                             .TranslatedDisplayName
                         : i.OriginCity.DisplayName))
                 .ForMember(x => x.DestinationCity,
                     x => x.MapFrom(i =>
                         i.DestinationCity.Translations.FirstOrDefault(t =>
                             t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                             ? i.DestinationCity.Translations
                                 .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                 .TranslatedDisplayName
                             : i.DestinationCity.DisplayName))
                 .ForMember(x => x.CompanyName, x => x.MapFrom(i => i.DestinationTenant.Name))
                 .ForMember(x => x.CompanyEditionName, x => x.MapFrom(i => i.DestinationTenant.Edition.DisplayName))
                 .ForMember(x => x.RouteType,
                     x => x.MapFrom(i => i.RouteType.ToString()))
                 .ForMember(x => x.Type, x => x.MapFrom(i => i.Type.ToString()))
                 .ForMember(x => x.OfferStatusTitle, x => x.MapFrom(i => i.Status.ToString()))
                 .ForMember(x => x.OfferStatus, x => x.MapFrom(i => i.Status))
                 .ForMember(x => x.CommissionType, x => x.MapFrom(i => i.CommissionType.ToString()));
        }
    }
}