using AutoMapper;
using System;
using System.Globalization;
using System.Linq;
using TACHYON.PricePackages;
using TACHYON.PricePackages.Dto;
using TACHYON.PricePackages.Dto.PricePackageAppendices;
using TACHYON.PricePackages.Dto.PricePackageProposals;
using TACHYON.PricePackages.PricePackageAppendices;
using TACHYON.PricePackages.PricePackageProposals;
using TACHYON.ServiceAreas;
using TACHYON.Shipping.ShippingRequests;

namespace TACHYON.AutoMapper.PricePackages
{
    public class PricePackageProfile : Profile
    {
        public PricePackageProfile()
        {

            const string activeKey = "Active", notActiveKey = "NotActive";
            CreateMap<PricePackage, PricePackageListDto>()
                .ForMember(x => x.HasProposal, x => x.MapFrom(i => i.ProposalId.HasValue))
                .ForMember(x => x.ServiceAreas, x => x.MapFrom(i => i.ServiceAreas.Select(x=> x.CityId).ToList()))
                .ForMember(x => x.HasAppendix, x => x.MapFrom(i => i.AppendixId.HasValue))
                .ForMember(x => x.OriginCity, x => x.MapFrom(i => i.OriginCity.DisplayName))
                .ForMember(x => x.DestinationCity, x => x.MapFrom(i => i.DestinationCity.DisplayName))
                .ForMember(x => x.Company, x => x.MapFrom(i => i.UsageType == PricePackageUsageType.AsCarrier ? i.Tenant.Name :  i.DestinationTenant.Name))
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
                    x => x.MapFrom(i => i.ProposalId.HasValue ? i.Proposal.ProposalName : "__"))
                .ForMember(x => x.Appendix,
                    x => x.MapFrom(i =>
                        i.ProposalId.HasValue && i.Proposal.AppendixId.HasValue ? i.Proposal.Appendix.ContractName :
                        i.AppendixId.HasValue ? i.Appendix.ContractName : "__"))
                .ForMember(x => x.EditionName, x => x.MapFrom(i => i.UsageType == PricePackageUsageType.AsCarrier ? i.Tenant.Edition.DisplayName : i.DestinationTenant.Edition.DisplayName))
                .ForMember(x => x.RouteType,
                    x => x.MapFrom(i => i.RouteType.HasValue? i.RouteType.Value.ToString(): string.Empty));

            CreateMap<CreateOrEditServiceAreaDto, ServiceArea>().ReverseMap();
            CreateMap<CreateOrEditPricePackageDto, PricePackage>()
                .ForMember(x=> x.ServiceAreas,x=> x.MapFrom(i=> i.ServiceAreas)).ReverseMap();
            
            CreateMap<CreateOrEditProposalDto, PricePackageProposal>()
                .ForMember(x => x.PricePackages, x => x.Ignore())
                .AfterMap((dto, proposal) =>
                {
                    if (!dto.Id.HasValue)
                        proposal.Status = ProposalStatus.New;
                });
            
            CreateMap<PricePackageProposal, CreateOrEditProposalDto>()
                .ForMember(x => x.PricePackages, x => x.MapFrom(i => i.PricePackages.Select(t => t.Id)));

            CreateMap<PricePackageProposal, ProposalListItemDto>()
                .ForMember(x => x.ShipperName, x => x.MapFrom(i => i.Shipper.Name))
                .ForMember(x => x.NumberOfPricePackages, x => x.MapFrom(i => i.PricePackages.Count))
                .ForMember(x => x.AppendixNumber, x => x.MapFrom(i => i.AppendixId.HasValue ? i.Appendix.Version : string.Empty));
            CreateMap<PricePackageProposal, ProposalForViewDto>()
                .ForMember(x => x.StatusTitle, x => x.MapFrom(i => Enum.GetName(typeof(ProposalStatus), i.Status)))
                .ForMember(x => x.Shipper, x => x.MapFrom(i => i.Shipper.companyName));
            CreateMap<PricePackage, PricePackageSelectItemDto>()
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

            CreateMap<PricePackageAppendix, AppendixListDto>()
                .ForMember(x => x.CompanyName,
                    x => x.MapFrom(i => i.ProposalId.HasValue ? i.Proposal.Shipper.Name : i.DestinationTenant.Name))
                .ForMember(x=> x.EditionName,x=> x.MapFrom(i=> i.ProposalId.HasValue ? i.Proposal.Shipper.Edition.DisplayName : i.DestinationTenant.Edition.DisplayName))
                .ForMember(x=> x.AppendixNumber,x=> x.MapFrom(i=> i.Version))
                .ForMember(x=> x.NumberOfPricePackages,x=> x.MapFrom(i=> i.ProposalId.HasValue ? i.Proposal.PricePackages.Count : i.PricePackages.Count))
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
                .ForMember(x => x.PricePackages, x => x.Ignore())
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
                    dto.PricePackages = appendix.PricePackages.Select(x => x.Id).ToList();
                }));

            CreateMap<PricePackage, PricePackageForViewDto>()
                .ForMember(x => x.CompanyName,
                    x => x.MapFrom(i => i.UsageType == PricePackageUsageType.AsTachyonManageService ?
                        i.DestinationTenant.Name : i.Tenant.Name)) 
                .ForMember(x => x.AppendixId,
                    x => x.MapFrom(i => i.ProposalId.HasValue ? i.Proposal.AppendixId : i.AppendixId))
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
                .ForMember(x => x.CompanyTenantId, x => x.MapFrom(i => i.DestinationTenantId));

            CreateMap<PricePackage, PricePackageForPricingDto>()
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
                    x => x.MapFrom(i => i.RouteType.HasValue? i.RouteType.Value.ToString(): string.Empty))
                .ForMember(x => x.Type, x => x.MapFrom(i => i.Type.ToString()));

            CreateMap<PricePackageProposal, ProposalAutoFillDataDto>()
                .ForMember(x => x.AppendixDate, x => x.MapFrom(i => i.ProposalDate));

            CreateMap<PricePackageForViewDto,PricePackageLookup>()
                .ForMember(x => x.PricePackage, x => x.MapFrom(i => i))
                .ForMember(x => x.HasOffer, x => x.MapFrom(i => i.HasOffer))
                .ForMember(x => x.HasDirectRequest, x => x.MapFrom(i => i.HasDirectRequest))
                .ForMember(x => x.HasParentOffer, x => x.MapFrom(i => i.HasParentOffer)).ReverseMap();

            CreateMap<PricePackage, PricePackageForPriceCalculationDto>()
                .ForMember(x => x.Destination, x => x.MapFrom(i => i.DestinationCity.DisplayName))
                .ForMember(x => x.Origin, x => x.MapFrom(i => i.OriginCity.DisplayName))
                .ForMember(x => x.TruckType, x => x.MapFrom(i => i.TrucksTypeFk.Key))
                .ForMember(x => x.PricePackageId, x => x.MapFrom(i => i.Id))
                .ForMember(x => x.IsMultiDrop,
                    x => x.MapFrom(i => i.RouteType == ShippingRequestRouteType.MultipleDrops));
        }
    }
}