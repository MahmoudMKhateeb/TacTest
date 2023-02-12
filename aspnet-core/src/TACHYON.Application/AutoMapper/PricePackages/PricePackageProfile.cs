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
                .ForMember(src => src.PricePerExtraDrop,
                    opt => opt.MapFrom(des =>
                        des.PricePerExtraDrop.HasValue ? des.PricePerExtraDrop.ToString() : "---"))
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
            const string activeKey = "Active", notActiveKey = "NotActive";
            CreateMap<TmsPricePackage, TmsPricePackageListDto>()
                .ForMember(x => x.HasProposal, x => x.MapFrom(i => i.ProposalId.HasValue))
                .ForMember(x => x.OriginCity, x => x.MapFrom(i => i.OriginCity.DisplayName))
                .ForMember(x => x.DestinationCity, x => x.MapFrom(i => i.DestinationCity.DisplayName))
                .ForMember(x => x.Shipper, x => x.MapFrom(i => i.DestinationTenant.Name))
                .ForMember(x => x.TruckType, x => x.MapFrom(i => i.TrucksTypeFk.Key))
                .ForMember(x => x.Status,
                    x => x.MapFrom(i =>
                        (i.ProposalId.HasValue && i.Proposal.AppendixId.HasValue && i.Proposal.Appendix.IsActive)
                            ?
                            activeKey
                            : (i.AppendixId.HasValue && i.Appendix.IsActive)
                                ? activeKey
                                : notActiveKey))
                .ForMember(x => x.ProposalName,
                    x => x.MapFrom(i => i.ProposalId.HasValue ? i.Proposal.ProposalName : string.Empty))
                .ForMember(x => x.Appendix,
                    x => x.MapFrom(i =>
                        i.ProposalId.HasValue && i.Proposal.AppendixId.HasValue ? i.Proposal.Appendix.ContractName :
                        i.AppendixId.HasValue ? i.Appendix.ContractName : string.Empty))
                .ForMember(x => x.EditionName, x => x.MapFrom(i => i.DestinationTenant.Edition.DisplayName))
                .ForMember(x => x.RouteType,
                    x => x.MapFrom(i => i.RouteType.ToString()));

            CreateMap<CreateOrEditTmsPricePackageDto, TmsPricePackage>().ReverseMap();
            CreateMap<CreateOrEditProposalDto, PricePackageProposal>()
                .ForMember(x => x.TmsPricePackages, x => x.Ignore())
                .AfterMap((dto, proposal) =>
                {
                    if (!dto.Id.HasValue)
                        proposal.Status = ProposalStatus.New;
                });
            
            CreateMap<PricePackageProposal, CreateOrEditProposalDto>()
                .ForMember(x => x.TmsPricePackages, x => x.MapFrom(i => i.TmsPricePackages.Select(t => t.Id)));

            CreateMap<PricePackageProposal, ProposalListItemDto>()
                .ForMember(x => x.ShipperName, x => x.MapFrom(i => i.Shipper.Name))
                .ForMember(x => x.NumberOfPricePackages, x => x.MapFrom(i => i.TmsPricePackages.Count))
                .ForMember(x => x.AppendixNumber, x => x.MapFrom(i => i.AppendixId.HasValue ? i.Appendix.Version : string.Empty));
            CreateMap<PricePackageProposal, ProposalForViewDto>()
                .ForMember(x => x.StatusTitle, x => x.MapFrom(i => Enum.GetName(typeof(ProposalStatus), i.Status)))
                .ForMember(x => x.Shipper, x => x.MapFrom(i => i.Shipper.companyName));
            CreateMap<TmsPricePackage, PricePackageSelectItemDto>()
                .ForMember(x => x.TruckType,
                    x => x.MapFrom(i =>
                        i.TrucksTypeFk.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.TrucksTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName
                            : i.TrucksTypeFk.DisplayName))
                .ForMember(x => x.OriginCity,
                    x => x.MapFrom(i =>
                        i.OriginCity.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
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
                            : i.DestinationCity.DisplayName));
           
            CreateMap<NormalPricePackage, PricePackageSelectItemDto>()
                .ForMember(x => x.TruckType,
                    x => x.MapFrom(i =>
                        i.TrucksTypeFk.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.TrucksTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName
                            : i.TrucksTypeFk.DisplayName))
                .ForMember(x => x.OriginCity,
                    x => x.MapFrom(i =>
                        i.OriginCityFK.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.OriginCityFK.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName
                            : i.OriginCityFK.DisplayName))
                .ForMember(x => x.DestinationCity,
                    x => x.MapFrom(i =>
                        i.DestinationCityFK.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.DestinationCityFK.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName
                            : i.DestinationCityFK.DisplayName));

            CreateMap<PricePackageAppendix, AppendixListDto>()
                .ForMember(x => x.CompanyName,
                    x => x.MapFrom(i => i.ProposalId.HasValue ? i.Proposal.Shipper.Name : i.DestinationTenant.Name))
                .ForMember(x=> x.EditionName,x=> x.MapFrom(i=> i.ProposalId.HasValue ? i.Proposal.Shipper.Edition.DisplayName : i.DestinationTenant.Edition.DisplayName))
                .ForMember(x=> x.AppendixNumber,x=> x.MapFrom(i=> i.Version))
                .ForMember(x=> x.NumberOfPricePackages,x=> x.MapFrom(i=> i.ProposalId.HasValue ? i.Proposal.TmsPricePackages.Count : i.NormalPricePackages.Count + i.TmsPricePackages.Count))
                .ForMember(x => x.ContractNumber,
                    x => x.MapFrom(i =>
                        i.ProposalId.HasValue
                            ? i.Proposal.Shipper.ContractNumber
                            : i.DestinationTenant.ContractNumber));

            CreateMap<PricePackageAppendix, AppendixForViewDto>()
                .ForMember(x => x.ProposalName, x => x.MapFrom(i => i.Proposal.ProposalName))
                .ForMember(x => x.ContractNumber,
                    x => x.MapFrom(i =>
                        i.ProposalId.HasValue ? i.Proposal.Shipper.ContractNumber : i.DestinationTenant.ContractNumber))
                .ForMember(x => x.StatusTitle, x => x.MapFrom(i => i.Status.ToString()));
            CreateMap<CreateOrEditAppendixDto, PricePackageAppendix>()
                .ForMember(x => x.TmsPricePackages, x => x.Ignore())
                .ForMember(x => x.DestinationTenantId, x => x.MapFrom(i => i.DestinationCompanyId))
                .AfterMap(((dto, appendix) =>
                {
                    if (dto.Id.HasValue) return;
                    
                    appendix.Status = AppendixStatus.New;
                    appendix.IsActive = false;
                }));
            CreateMap<PricePackageAppendix, CreateOrEditAppendixDto>()
                .ForMember(x => x.DestinationCompanyId, x => x.MapFrom(i => i.DestinationTenantId))
                .ForMember(x => x.PricePackages, x => x.Ignore())
                .AfterMap(((appendix, dto) =>
                {
                    dto.PricePackages = new List<string>();
                    var tmsPricePackage = appendix.TmsPricePackages.Select(x => x.PricePackageId);
                    var normalPricePackage = appendix.NormalPricePackages.Select(x => x.PricePackageId);
                    dto.PricePackages.AddRange(tmsPricePackage);
                    dto.PricePackages.AddRange(normalPricePackage);
                }));

            CreateMap<TmsPricePackage, TmsPricePackageForViewDto>()
                .ForMember(x => x.CompanyName, x => x.MapFrom(i => i.DestinationTenant.Name))
                .ForMember(x => x.AppendixId, x => x.MapFrom(i => i.ProposalId.HasValue ? i.Proposal.AppendixId : i.AppendixId))
                .ForMember(x => x.FinalPrice, x => x.MapFrom(i => i.TotalPrice))
                .ForMember(x => x.TruckType,
                    x => x.MapFrom(i =>
                        i.TrucksTypeFk.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.TrucksTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName
                            : i.TrucksTypeFk.DisplayName))
                .ForMember(x => x.TransportType,
                    x => x.MapFrom(i =>
                        i.TransportTypeFk.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.TransportTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName
                            : i.TransportTypeFk.DisplayName))
                .ForMember(x => x.OriginCity,
                    x => x.MapFrom(i =>
                        i.OriginCity.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
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
                .ForMember(x => x.CompanyTenantId, x => x.MapFrom(i => i.DestinationTenantId))
                .ForMember(x=> x.IsTmsPricePackage,x=> x.MapFrom(i=> true))
                .ForMember(x => x.IsShipperPricePackage,
                    x => x.MapFrom(i => i.ProposalId.HasValue && !i.AppendixId.HasValue));


            CreateMap<NormalPricePackage, TmsPricePackageForViewDto>()
                .ForMember(x => x.HasOffer, x => x.MapFrom(i => i.BidNormalPricePackages.Any()))
                .ForMember(x => x.CompanyName, x => x.MapFrom(i => i.Tenant.Name))
                .ForMember(x => x.FinalPrice, x => x.MapFrom(i => i.TachyonMSRequestPrice))
                .ForMember(x => x.TruckType,
                    x => x.MapFrom(i =>
                        i.TrucksTypeFk.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.TrucksTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName
                            : i.TrucksTypeFk.DisplayName))
                .ForMember(x => x.TransportType,
                    x => x.MapFrom(i =>
                        i.TransportTypeFk.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.TransportTypeFk.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName
                            : i.TransportTypeFk.DisplayName))
                .ForMember(x => x.OriginCity, x => x.MapFrom(i =>
                    i.OriginCityFK.Translations.FirstOrDefault(
                        t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                        ? i.OriginCityFK.Translations
                            .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                            .TranslatedDisplayName
                        : i.OriginCityFK.DisplayName))
                .ForMember(x => x.DestinationCity,
                    x => x.MapFrom(i =>
                        i.DestinationCityFK.Translations.FirstOrDefault(t =>
                            t.Language.Contains(CultureInfo.CurrentUICulture.Name)) != null
                            ? i.DestinationCityFK.Translations
                                .FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name))
                                .TranslatedDisplayName
                            : i.DestinationCityFK.DisplayName))
                .ForMember(x => x.CompanyTenantId, x => x.MapFrom(i => i.TenantId))
                .ForMember(x => x.IsShipperPricePackage, x => x.MapFrom(i => false));
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
                .ForMember(x => x.Type, x => x.MapFrom(i => i.Type.ToString()));

            CreateMap<PricePackageProposal, ProposalAutoFillDataDto>()
                .ForMember(x => x.AppendixDate, x => x.MapFrom(i => i.ProposalDate));
        }
    }
}