using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.DynamicEntityParameters;
using Abp.EntityHistory;
using Abp.Extensions;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using Abp.Webhooks;
using AutoMapper;
using DevExtreme.AspNet.Data.ResponseModel;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using TACHYON.AddressBook;
using TACHYON.AddressBook.Dtos;
using TACHYON.AddressBook.Ports;
using TACHYON.AddressBook.Ports.Dtos;
using TACHYON.Auditing.Dto;
using TACHYON.Authorization.Accounts.Dto;
using TACHYON.Authorization.Delegation;
using TACHYON.Authorization.Permissions.Dto;
using TACHYON.Authorization.Roles;
using TACHYON.Authorization.Roles.Dto;
using TACHYON.Authorization.Users;
using TACHYON.Authorization.Users.Delegation.Dto;
using TACHYON.Authorization.Users.Dto;
using TACHYON.Authorization.Users.Importing.Dto;
using TACHYON.Authorization.Users.Profile.Dto;
using TACHYON.Chat;
using TACHYON.Chat.Dto;
using TACHYON.Cities;
using TACHYON.Cities.CitiesTranslations;
using TACHYON.Cities.CitiesTranslations.Dtos;
using TACHYON.Cities.Dtos;
using TACHYON.Countries;
using TACHYON.Countries.CountriesTranslations;
using TACHYON.Countries.CountriesTranslations.Dtos;
using TACHYON.Countries.Dtos;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Documents.DocumentsEntities.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Documents.DocumentTypeTranslations;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Drivers.importing.Dto;
using TACHYON.DynamicEntityParameters.Dto;
using TACHYON.Editions;
using TACHYON.Editions.Dto;
using TACHYON.Friendships;
using TACHYON.Friendships.Cache;
using TACHYON.Friendships.Dto;
using TACHYON.Goods;
using TACHYON.Goods.Dtos;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Invoices;
using TACHYON.Invoices.Balances;
using TACHYON.Invoices.Balances.Dto;
using TACHYON.Invoices.Dto;
using TACHYON.Invoices.Groups;
using TACHYON.Invoices.Periods;
using TACHYON.Invoices.Periods.Dto;
using TACHYON.Invoices.SubmitInvoices;
using TACHYON.Invoices.SubmitInvoices.Dto;
using TACHYON.Invoices.Transactions;
using TACHYON.Invoices.Transactions.Dto;
using TACHYON.Localization;
using TACHYON.Localization.Dto;
using TACHYON.MultiTenancy;
using TACHYON.MultiTenancy.Dto;
using TACHYON.MultiTenancy.HostDashboard.Dto;
using TACHYON.MultiTenancy.Payments;
using TACHYON.MultiTenancy.Payments.Dto;
using TACHYON.Nationalities;
using TACHYON.Nationalities.Dtos;
using TACHYON.Nationalities.NationalitiesTranslation;
using TACHYON.Nationalities.NationalitiesTranslation.Dtos;
using TACHYON.Notifications.Dto;
using TACHYON.Offers;
using TACHYON.Offers.Dtos;
using TACHYON.Organizations.Dto;
using TACHYON.Packing.PackingTypes;
using TACHYON.Packing.PackingTypes.Dtos;
using TACHYON.Receivers;
using TACHYON.Receivers.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutSteps;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Sessions.Dto;
using TACHYON.Shipping.Accidents;
using TACHYON.Shipping.Accidents.Dto;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestStatuses.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.Shipping.ShippingTypes.Dtos;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.Trips.RejectReasons.Dtos;
using TACHYON.ShippingRequestTripVases;
using TACHYON.ShippingRequestTripVases.Dtos;
using TACHYON.ShippingRequestVases;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.TermsAndConditions;
using TACHYON.TermsAndConditions.Dtos;
using TACHYON.Trailers;
using TACHYON.Trailers.Dtos;
using TACHYON.Trailers.PayloadMaxWeights;
using TACHYON.Trailers.PayloadMaxWeights.Dtos;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerStatuses.Dtos;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trailers.TrailerTypes.Dtos;
using TACHYON.Trucks;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.Importing.Dto;
using TACHYON.Trucks.PlateTypes;
using TACHYON.Trucks.PlateTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations;
using TACHYON.Trucks.TruckCategories.TruckCapacities.TruckCapacitiesTranslations.Dtos;
using TACHYON.Trucks.TruckStatusesTranslations;
using TACHYON.Trucks.TruckStatusesTranslations.Dtos;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations;
using TACHYON.Trucks.TrucksTypes.TrucksTypesTranslations.Dtos;
using TACHYON.UnitOfMeasures;
using TACHYON.UnitOfMeasures.Dtos;
using TACHYON.Vases;
using TACHYON.Vases.Dtos;
using TACHYON.WebHooks.Dto;

namespace TACHYON
{
    internal static class CustomDtoMapper
    {
        private static IMapper _Mapper;
        public static void SetMapper(IMapper Mapper)
        {
            _Mapper = Mapper;
        }
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditDangerousGoodTypeDto, DangerousGoodType>().ReverseMap();
            configuration.CreateMap<DangerousGoodTypeDto, DangerousGoodType>().ReverseMap();
            configuration.CreateMap<DangerousGoodTypeTranslation, DangerousGoodTypeTranslationDto>()
                .ForMember(dest => dest.Name,
                    dest =>
                        dest.MapFrom(src => src.TranslatedName));
            configuration.CreateMap<CreateOrEditDangerousGoodTypeTranslationDto, DangerousGoodTypeTranslation>();
            configuration.CreateMap<CreateOrEditCitiesTranslationDto, CitiesTranslation>().ReverseMap();
            configuration.CreateMap<CitiesTranslationDto, CitiesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditCountriesTranslationDto, CountriesTranslation>().ReverseMap();
            configuration.CreateMap<CountriesTranslationDto, CountriesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditTruckCapacitiesTranslationDto, TruckCapacitiesTranslation>().ReverseMap();
            configuration.CreateMap<TruckCapacitiesTranslationDto, TruckCapacitiesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditTruckStatusesTranslationDto, TruckStatusesTranslation>().ReverseMap();
            configuration.CreateMap<TruckStatusesTranslationDto, TruckStatusesTranslation>().ReverseMap();

            configuration.CreateMap<TruckStatus, GetTruckStatusForViewDto>()
                .ForMember(x => x.TruckStatus,
                    x => x.MapFrom(i => i));
            configuration.CreateMap<TruckStatus, TruckStatusDto>().ReverseMap();
            configuration.CreateMap<PackingTypeTranslation, PackingTypeTranslationDto>().ReverseMap();
            configuration.CreateMap<PackingTypeTranslation, PackingTypeTranslationDto>().ReverseMap();

            configuration.CreateMap<CreateOrEditPackingTypeDto, PackingType>().ForMember(x => x.Translations, x => x.Ignore());
            // configuration.CreateMap<GetPackingTypeForViewDto, PackingType>().ForPath(x => x, x => x.MapFrom(src => src.PackingType)).ReverseMap();

            configuration.CreateMap<PackingType, CreateOrEditPackingTypeDto>()
                .ForMember(x => x.TranslationDtos, x =>
                    x.MapFrom(i => i.Translations));


            configuration.CreateMap<PackingType, PackingTypeDto>()
                .ForMember(x => x.DisplayName, x =>
                    x.MapFrom(i => i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null ? i.DisplayName : i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName))
                .ForMember(x => x.Description, x =>
                    x.MapFrom(i => i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null ? i.Description : i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).Description));

            configuration.CreateMap<TruckStatusesTranslation, GetTruckStatusesTranslationForViewDto>()
                .ForMember(x => x.TruckStatusDisplayName,
                    x => x.MapFrom(i => i.Core.DisplayName))
                .ForMember(x => x.TruckStatusesTranslation,
                    x => x.MapFrom(i => i));

            configuration.CreateMap<CreateOrEditShippingTypeDto, ShippingType>().ReverseMap();
            configuration.CreateMap<ShippingTypeDto, ShippingType>().ReverseMap();
            configuration.CreateMap<ShippingType, ShippingTypeDto>()
             .ForMember(x => x.DisplayName, x =>
                x.MapFrom(i => i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null ? i.DisplayName : i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName))
            .ForMember(x => x.Description, x =>
                x.MapFrom(i => i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).Description));

            configuration.CreateMap<ShippingTypeTranslation, ShippingTypeTranslationDto>().ReverseMap();
            configuration.CreateMap<ShippingTypeTranslation, CreateOrEditShippingTypeTranslationDto>().ReverseMap();



            configuration.CreateMap<CreateOrEditTruckCapacitiesTranslationDto, TruckCapacitiesTranslation>().ReverseMap();
            configuration.CreateMap<TruckCapacitiesTranslationDto, TruckCapacitiesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditTruckStatusesTranslationDto, TruckStatusesTranslation>().ReverseMap();
            configuration.CreateMap<TruckStatusesTranslationDto, TruckStatusesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditCitiesTranslationDto, CitiesTranslation>().ReverseMap();
            configuration.CreateMap<CitiesTranslationDto, CitiesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditCountriesTranslationDto, CountriesTranslation>().ReverseMap();
            configuration.CreateMap<CountriesTranslationDto, CountriesTranslation>().ReverseMap();
            //configuration.CreateMap<CreateOrEditPlateTypeDto, PlateType>().ReverseMap();
            //configuration.CreateMap<PlateTypeDto, PlateType>().ReverseMap();
            configuration.CreateMap<CreateOrEditNationalityDto, Nationality>().ReverseMap();
            configuration.CreateMap<NationalityDto, Nationality>().ReverseMap();
            configuration.CreateMap<CreateOrEditNationalityTranslationDto, NationalityTranslation>().ReverseMap();
            configuration.CreateMap<NationalityTranslationDto, NationalityTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditTrucksTypesTranslationDto, TrucksTypesTranslation>().ReverseMap();
            configuration.CreateMap<TrucksTypesTranslationDto, TrucksTypesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditTransportTypesTranslationDto, TransportTypesTranslation>().ReverseMap();
            configuration.CreateMap<TransportTypesTranslationDto, TransportTypesTranslation>().ReverseMap();
            configuration.CreateMap<VasTranslation, VasTranslationDto>().ReverseMap();
            configuration.CreateMap<Vas, GetVasForViewDto>()
                .ForMember(x => x.Vas, x => x.MapFrom(i => i));
            configuration.CreateMap<CreateOrEditShippingRequestVasDto, ShippingRequestVas>().ReverseMap();
            configuration.CreateMap<ShippingRequestVasDto, ShippingRequestVas>().ReverseMap();
            configuration.CreateMap<CreateOrEditVasPriceDto, VasPrice>().ReverseMap();
            configuration.CreateMap<VasPriceDto, VasPrice>().ReverseMap();
            // Need To Improve The Way Of Get Translation
            configuration.CreateMap<VasPrice, AvailableVasDto>()
                .ForMember(x => x.VasName, x =>
                    x.MapFrom(i =>
                        i.VasFk.Translations.FirstOrDefault(t =>
                            CultureInfo.CurrentUICulture.Name.Contains(t.Language)) != null
                            ? i.VasFk.Translations
                                .FirstOrDefault(t => CultureInfo.CurrentUICulture.Name.Contains(t.Language)).DisplayName
                            : i.VasFk.Name));

            configuration.CreateMap<Vas, GetVasForEditOutput>()
                .ForMember(x => x.Vas, x
                    => x.MapFrom(i => i));
            configuration.CreateMap<Vas, CreateOrEditVasDto>();
            configuration.CreateMap<CreateOrEditVasDto, Vas>()
                .ForMember(x => x.Translations, x => x.Ignore());
            configuration.CreateMap<VasDto, Vas>().ReverseMap();
            configuration.CreateMap<CreateOrEditReceiverDto, Receiver>().ReverseMap();
            configuration.CreateMap<ReceiverDto, Receiver>().ReverseMap();
            configuration.CreateMap<CreateOrEditTermAndConditionTranslationDto, TermAndConditionTranslation>().ReverseMap();
            configuration.CreateMap<TermAndConditionTranslationDto, TermAndConditionTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditTermAndConditionDto, TermAndCondition>().ReverseMap();
            configuration.CreateMap<CreateOrEditCapacityDto, Capacity>().ReverseMap();
            configuration.CreateMap<CapacityDto, Capacity>().ReverseMap();
            configuration.CreateMap<CreateOrEditTransportTypeDto, TransportType>().ReverseMap();
            configuration.CreateMap<TransportTypeDto, TransportType>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentTypeTranslationDto, DocumentTypeTranslation>().ReverseMap();
            configuration.CreateMap<DocumentTypeTranslationDto, DocumentTypeTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentsEntityDto, DocumentsEntity>().ReverseMap();
            configuration.CreateMap<DocumentFileForCreateOrEditDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<DocumentsEntityDto, DocumentsEntity>().ReverseMap();
            configuration.CreateMap<CreateOrEditShippingRequestStatusDto, Shipping.ShippingRequestStatuses.ShippingRequestStatus>().ReverseMap();
            configuration.CreateMap<ShippingRequestStatusDto, Shipping.ShippingRequestStatuses.ShippingRequestStatus>().ReverseMap();
            //configuration.CreateMap<CreateOrEditPickingTypeDto, PickingType>().ReverseMap();
            //configuration.CreateMap<PickingTypeDto, PickingType>().ReverseMap();
            configuration.CreateMap<CreateOrEditPortDto, Port>().ReverseMap();
            configuration.CreateMap<PortDto, Port>().ReverseMap();
            configuration.CreateMap<CreateOrEditUnitOfMeasureDto, UnitOfMeasure>().ReverseMap();
            configuration.CreateMap<UnitOfMeasureDto, UnitOfMeasure>().ReverseMap();
            configuration.CreateMap<CreateOrEditFacilityDto, Facility>().ReverseMap();
            configuration.CreateMap<Facility, CreateOrEditFacilityDto>()
                .ForMember(dst => dst.Longitude, opt => opt.MapFrom(src => src.Location.X))
                 .ForMember(dst => dst.Latitude, opt => opt.MapFrom(src => src.Location.Y));
            configuration.CreateMap<Facility, FacilityLocationListDto>()
                .ForMember(dst => dst.Longitude, opt => opt.MapFrom(src => src.Location.X))
                .ForMember(dst => dst.Latitude, opt => opt.MapFrom(src => src.Location.Y));
            configuration.CreateMap<FacilityDto, Facility>()
                 .ForPath(dst => dst.Location.X, opt => opt.MapFrom(src => src.Longitude))
                 .ForPath(dst => dst.Location.Y, opt => opt.MapFrom(src => src.Latitude))
                .ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<DocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<ImportTruckDocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentTypeDto, DocumentType>().ReverseMap();
            configuration.CreateMap<UserInGetDocumentFileForViewDto, User>().ReverseMap();
            // tag:#Mapper_DocumentFile_GetAllTenantsSubmittedDocumentsDto
            configuration.CreateMap<DocumentFile, GetAllTenantsSubmittedDocumentsDto>()
                    .ForMember(dto => dto.DocumentTypeName,
                        conf => conf.MapFrom(ol =>
                            ol.DocumentTypeFk.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null ? ol.DocumentTypeFk.DisplayName : ol.DocumentTypeFk.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)).Name))
                    .ForMember(dto => dto.SubmitterTenatTenancyName, opt => opt.MapFrom(src => src.TenantFk.TenancyName))
                    .ForMember(dto => dto.SubmitterTenatTenancyName, opt => opt.MapFrom(src => src.TenantFk.TenancyName))
                    .ReverseMap();

            configuration.CreateMap<DocumentFile, GetAllDriversSubmittedDocumentsDto>()
                    .ForMember(dto => dto.DocumentTypeName,
                        conf => conf.MapFrom(ol =>
                            ol.DocumentTypeFk.Translations
                                .First(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)).Name))
                    .ForMember(dto => dto.DriverName, opt => opt.MapFrom(src => src.UserFk.Name + " " + src.UserFk.Surname))
                    .ReverseMap();


            configuration.CreateMap<DocumentFile, GetAllTrucksSubmittedDocumentsDto>()
                    .ForMember(dto => dto.DocumentTypeName,
                        conf => conf.MapFrom(ol =>
                            ol.DocumentTypeFk.Translations
                                .First(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)).Name))
                    .ForMember(dto => dto.TruckPlateNumber, opt => opt.MapFrom(src => src.TruckFk.PlateNumber))
                    .ReverseMap();


            // tag:#Mapper_DocumentType_DocumentTypeDto
            configuration.CreateMap<DocumentType, DocumentTypeDto>()
                .ForMember(dto => dto.Name,
                    conf => conf.MapFrom(ol =>
                        ol.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)).Name))
                .ForMember(dto => dto.Language,
                    conf => conf.MapFrom(ol =>
                        ol.Translations.FirstOrDefault(x => x.Language.Contains(CultureInfo.CurrentUICulture.Name)).Language))
                .ForMember(dst => dst.RequiredFrom, opt => opt.MapFrom(src => src.DocumentsEntityFk.DisplayName))
                .ForMember(dst => dst.Edition, opt => opt.MapFrom(src => src.EditionFk.DisplayName))
                .ForMember(dst => dst.DocumentRelatedWithName,
                    opt => opt.MapFrom(src => src.DocumentRelatedWithFk.TenancyName))
                .ReverseMap();



            // configuration.CreateMap<CreateOrEditShippingRequestVasListDto, ShippingRequestVas>().ReverseMap();
            //configuration.CreateMap<CreateOrEditRouteDto, Route>().ReverseMap();

            configuration.CreateMap<CreateOrEditShippingRequestDto, ShippingRequest>()
                .ForMember(d => d.ShippingRequestVases, opt => opt.Ignore())
            .AfterMap(AddOrUpdateShippingRequest)
                .ReverseMap();

            configuration.CreateMap<EditShippingRequestStep4Dto, ShippingRequest>()
                .ForMember(dest => dest.IsDrafted, opt => opt.Ignore())
                .ForMember(dest => dest.DraftStep, opt => opt.Ignore())
                .ForMember(d => d.ShippingRequestVases, opt => opt.Ignore())
            .AfterMap(AddOrUpdateShippingRequest);

            configuration.CreateMap<ShippingRequest, EditShippingRequestStep4Dto>()
                .ForMember(dest => dest.ShippingRequestVasList, opt => opt.MapFrom(src => src.ShippingRequestVases));

            configuration.CreateMap<CreateOrEditShippingRequestTripDto, ShippingRequestTrip>()
                .ForMember(d => d.WaybillNumber, opt => opt.Ignore())
                .ForMember(d => d.RoutPoints, opt => opt.Ignore())
                .ForMember(d => d.ShippingRequestTripVases, opt => opt.Ignore())
                .AfterMap(AddOrUpdateShippingRequestTrip);



            configuration.CreateMap<ShippingRequestTripVas, CreateOrEditShippingRequestTripVasDto>()
                   .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ShippingRequestVasFk.VasFk.Name));
            configuration.CreateMap<CreateOrEditShippingRequestTripVasDto, ShippingRequestTripVas>();


            configuration.CreateMap<ShippingRequestTripVas, ShippingRequestTripVasDto>()
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ShippingRequestVasFk.VasFk.Name))
             .ReverseMap();

            configuration.CreateMap<ShippingRequestVasListOutput, ShippingRequestVas>()
                .ReverseMap();
            configuration.CreateMap<CreateOrEditShippingRequestVasListDto, ShippingRequestVas>()
                .ReverseMap();


            configuration.CreateMap<ShippingRequestBidDto, ShippingRequestBid>()
                .ForPath(dst => dst.Tenant.Name, opt => opt.MapFrom(src => src.CarrierName))
                .ReverseMap();
            configuration.CreateMap<CreatOrEditShippingRequestBidDto, ShippingRequestBid>().ReverseMap();
            configuration.CreateMap<ShippingRequestDto, ShippingRequest>().ReverseMap();
            configuration.CreateMap<CreateOrEditGoodsDetailDto, GoodsDetail>().ReverseMap();
            configuration.CreateMap<GoodsDetail, GoodsDetailDto>()
                .ReverseMap();
            configuration.CreateMap<CreateOrEditOfferDto, Offer>().ReverseMap();
            configuration.CreateMap<OfferDto, Offer>().ReverseMap();
            configuration.CreateMap<CreateOrEditRoutStepDto, RoutStep>()
                .ForMember(dst => dst.SourceRoutPointFk, opt => opt.MapFrom(src => src.CreateOrEditSourceRoutPointInputDto))
                .ForMember(dest => dest.DestinationRoutPointFk, opt => opt.MapFrom(src => src.CreateOrEditDestinationRoutPointInputDto))
                .ReverseMap();

            configuration.CreateMap<RoutStepDto, RoutStep>().ReverseMap();

            configuration.CreateMap<RoutPointDto, RoutPoint>()
                .ForPath(dest => dest.FacilityFk.Location.X, opt => opt.MapFrom(src => src.Longitude))
                .ForPath(dest => dest.FacilityFk.Location.Y, opt => opt.MapFrom(src => src.Latitude))
                .ForPath(dest => dest.FacilityFk.Name, opt => opt.MapFrom(src => src.Facility))
                .ForPath(dest => dest.GoodsDetails, opt => opt.MapFrom(src => src.GoodsDetailListDto))
                .ForPath(dest => dest.ReceiverFk.FullName, opt => opt.MapFrom(src => src.SenderOrReceiverContactName))
                .ReverseMap();

            configuration.CreateMap<CreateOrEditRoutPointDto, RoutPoint>()
                .ForMember(x => x.WaybillNumber, otp => otp.Ignore())
                .AfterMap(AddOrUpdateShippingRequestTripRoutePointGoods);

            configuration.CreateMap<RoutPoint, CreateOrEditRoutPointDto>()
                .ForMember(dest => dest.GoodsDetailListDto, opt => opt.MapFrom(src => src.GoodsDetails))
                .ForPath(dest => dest.Longitude, opt => opt.MapFrom(src => src.FacilityFk.Location.X))
                .ForPath(dest => dest.Latitude, opt => opt.MapFrom(src => src.FacilityFk.Location.Y));

            //configuration.CreateMap<CreateOrEditRouteDto, Route>().ReverseMap();
            //configuration.CreateMap<RouteDto, Route>().ReverseMap();
            configuration.CreateMap<CreateOrEditCityDto, City>().ReverseMap();
            configuration.CreateMap<City, CreateOrEditCityDto>()
                .ForPath(dst => dst.Longitude, opt => opt.MapFrom(src => src.Location.X))
                 .ForPath(dst => dst.Latitude, opt => opt.MapFrom(src => src.Location.Y));
            configuration.CreateMap<CityDto, City>()
                .ForPath(dst => dst.Location.X, opt => opt.MapFrom(src => src.Longitude))
                 .ForPath(dst => dst.Location.Y, opt => opt.MapFrom(src => src.Latitude))
                 .ReverseMap();

            configuration.CreateMap<CreateOrEditCountyDto, County>().ReverseMap();
            configuration.CreateMap<CountyDto, County>().ReverseMap();
            configuration.CreateMap<GoodCategory, GoodCategoryDto>()
                .ForMember(x => x.HasItems, x => x.MapFrom(i => i.GoodCategories.Any()))
                .ForMember(x => x.DisplayName, x => x.MapFrom(i => i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)) == null ? i.Key : i.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).DisplayName))
                .ReverseMap();

            configuration.CreateMap<CreateOrEditTrailerDto, Trailer>().ReverseMap();
            configuration.CreateMap<TrailerDto, Trailer>().ReverseMap();
            configuration.CreateMap<CreateOrEditTrailerStatusDto, TrailerStatus>().ReverseMap();
            configuration.CreateMap<TrailerStatusDto, TrailerStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditPayloadMaxWeightDto, PayloadMaxWeight>().ReverseMap();
            configuration.CreateMap<PayloadMaxWeightDto, PayloadMaxWeight>().ReverseMap();
            configuration.CreateMap<CreateOrEditTrailerTypeDto, TrailerType>().ReverseMap();
            configuration.CreateMap<TrailerTypeDto, TrailerType>().ReverseMap();
            configuration.CreateMap<CreateOrEditTruckDto, Truck>().ReverseMap();
            configuration.CreateMap<ImportTruckDto, Truck>().ReverseMap();
            // #Map_Truck_to_TruckDto
            configuration.CreateMap<Truck, TruckDto>()
                        .ForMember(dto => dto.TrucksTypeDisplayName, conf => conf.MapFrom(ol => ol.TrucksTypeFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName))
                        .ForMember(dto => dto.TransportTypeDisplayName, conf => conf.MapFrom(ol => ol.TransportTypeFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName))
                        .ForMember(dto => dto.TrucksTypeDisplayName, conf => conf.MapFrom(ol => ol.TrucksTypeFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName))
                        .ForMember(dto => dto.CapacityDisplayName, conf => conf.MapFrom(ol => ol.CapacityFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName))
                        .ForMember(dto => dto.TruckStatusDisplayName, conf => conf.MapFrom(ol => ol.TruckStatusFk.Translations.FirstOrDefault(t => t.Language.Contains(CultureInfo.CurrentUICulture.Name)).TranslatedDisplayName))
                       .ReverseMap();


            configuration.CreateMap<Truck, GetTruckForViewOutput>()
                .ForMember(dest => dest.Truck, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.TruckStatusDisplayName, opt => opt.MapFrom(src => src.TruckStatusFk.DisplayName));

            configuration.CreateMap<CreateOrEditTrucksTypeDto, TrucksType>()
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations))
                .ReverseMap();
            configuration.CreateMap<TrucksTypeDto, TrucksType>().ReverseMap();
            configuration.CreateMap<CreateOrEditTruckStatusDto, TruckStatus>()
                .ForMember(x => x.Translations, x => x.Ignore());

            configuration.CreateMap<CreateOrEditTruckStatusDto, TruckStatus>();

            configuration.CreateMap<TruckStatusDto, TruckStatus>().ReverseMap();
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Role>().ReverseMap();
            configuration.CreateMap<Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();

            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<Tenant, TenantProfileInformationDto>()
                .ForMember(x => x.CompanyName, x => x.MapFrom(i => i.companyName))
                .ForMember(x => x.CompanyInfo, x => x.MapFrom(i => i.Description))
                .ForMember(x => x.CompanyPhone, x => x.MapFrom(i => i.MobileNo))
                // .ForMember(x => x.Rating, x => x.MapFrom(i => i.Rate))
                .ForMember(x => x.CompanySite, x => x.MapFrom(i => i.Website));
            configuration.CreateMap<TenantProfileInformationDto, UpdateTenantProfileInformationInputDto>();
            configuration.CreateMap<TenantProfileInformationDto, TenantProfileInformationForViewDto>();
            configuration.CreateMap<UpdateTenantProfileInformationInputDto, Tenant>()
                .ForMember(x => x.MobileNo, x =>
                      x.MapFrom(i => i.CompanyPhone))
                .ForMember(x => x.Website, x =>
                      x.MapFrom(i => i.CompanySite))
                .ForMember(x => x.Description, x =>
                      x.MapFrom(i => i.CompanyInfo))
                .ForMember(x => x.companyName, x =>
                      x.MapFrom(i => i.CompanyName));
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();
            configuration.CreateMap<RegisterTenantInput, CreateTenantInput>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ForMember(dto => dto.IsAvailable,
                    dto => dto.MapFrom(user => user.DriverStatus == UserDriverStatus.Available));
            configuration.CreateMap<UserEditDto, User>()
                .ForMember(user => user.Password, options => options.Ignore())
                .ForMember(user => user.DriverStatus, user =>
                      user.MapFrom(dto => dto.IsAvailable ? UserDriverStatus.Available : UserDriverStatus.NotAvailable));
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, DriverListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();
            configuration.CreateMap<ImportDriverDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            //Webhooks
            configuration.CreateMap<WebhookSubscription, GetAllSubscriptionsOutput>();
            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOutput>()
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.WebhookName,
                    options => options.MapFrom(l => l.WebhookEvent.WebhookName))
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.Data,
                    options => options.MapFrom(l => l.WebhookEvent.Data));

            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOfWebhookEventOutput>();

            configuration.CreateMap<DynamicParameter, DynamicParameterDto>().ReverseMap();
            configuration.CreateMap<DynamicParameterValue, DynamicParameterValueDto>().ReverseMap();
            configuration.CreateMap<EntityDynamicParameter, EntityDynamicParameterDto>()
                .ForMember(dto => dto.DynamicParameterName,
                    options => options.MapFrom(entity => entity.DynamicParameter.ParameterName));
            configuration.CreateMap<EntityDynamicParameterDto, EntityDynamicParameter>();

            configuration.CreateMap<EntityDynamicParameterValue, EntityDynamicParameterValueDto>().ReverseMap();
            //User Delegations
            configuration.CreateMap<CreateUserDelegationDto, UserDelegation>();


            /*Invoices*/

            configuration.CreateMap<InvoicePeriodDto, InvoicePeriod>();
            configuration.CreateMap<InvoicePeriod, InvoicePeriodDto>();
            configuration.CreateMap<Invoice, InvoiceListDto>()
                .ForMember(dto => dto.TenantName, options => options.MapFrom(entity => entity.Tenant.Name))
                .ForMember(dto => dto.Period, options => options.MapFrom(entity => entity.InvoicePeriodsFK.DisplayName));

            configuration.CreateMap<Invoice, InvoiceInfoDto>()
                .ForMember(dto => dto.ClientName, options => options.MapFrom(entity => entity.Tenant.Name))
                .ForMember(dto => dto.Attn, options => options.MapFrom(entity => entity.Tenant.Name))
                .ForMember(dto => dto.ContractNo, options => options.MapFrom(entity => entity.Tenant.ContractNumber))
                .ForMember(dto => dto.Address, options => options.MapFrom(entity => entity.Tenant.Address))
                .ForMember(dto => dto.Period, options => options.MapFrom(entity => entity.InvoicePeriodsFK.DisplayName));

            configuration.CreateMap<InvoiceShippingRequests, InvoiceShippingRequestDto>()
              .ForMember(dto => dto.Price, options => options.MapFrom(entity => entity.ShippingRequests.Price))
              .ForMember(dto => dto.CreationTime, options => options.MapFrom(entity => entity.ShippingRequests.CreationTime))
              .ForMember(dto => dto.Source, options => options.MapFrom(entity => entity.ShippingRequests.OriginCityFk.DisplayName))
              .ForMember(dto => dto.Destination, options => options.MapFrom(entity => entity.ShippingRequests.DestinationCityFk));


            configuration.CreateMap<BalanceRecharge, BalanceRechargeListDto>()
               .ForMember(dto => dto.TenantName, options => options.MapFrom(entity => entity.Tenant.Name));

            configuration.CreateMap<CreateBalanceRechargeInput, BalanceRecharge>();


            configuration.CreateMap<Tenant, InvoiceTenantDto>()
               .ForMember(dto => dto.DisplayName, options => options.MapFrom(entity => entity.Name))
               .ForMember(dto => dto.Group, options => options.MapFrom(entity => entity.Edition.Name));

            configuration.CreateMap<SubmitInvoice, SubmitInvoiceListDto>()
                .ForMember(dto => dto.TenantName, options => options.MapFrom(entity => entity.Tenant.Name))
                .ForMember(dto => dto.Period, options => options.MapFrom(entity => entity.InvoicePeriodsFK.DisplayName));

            configuration.CreateMap<SubmitInvoice, SubmitInvoiceInfoDto>()
                .ForMember(dto => dto.ClientName, options => options.MapFrom(entity => entity.Tenant.Name))
                .ForMember(dto => dto.InvoiceNumber, options => options.MapFrom(entity => entity.ReferencNumber))
                .ForMember(dto => dto.Attn, options => options.MapFrom(entity => entity.Tenant.Name))
                .ForMember(dto => dto.ContractNo, options => options.MapFrom(entity => entity.Tenant.ContractNumber))
                .ForMember(dto => dto.Address, options => options.MapFrom(entity => entity.Tenant.Address))
                .ForMember(dto => dto.Period, options => options.MapFrom(entity => entity.InvoicePeriodsFK.DisplayName));


            configuration.CreateMap<GroupShippingRequests, SubmitInvoiceShippingRequestDto>()
  .ForMember(dto => dto.Price, options => options.MapFrom(entity => entity.ShippingRequests.Price))
  .ForMember(dto => dto.CreationTime, options => options.MapFrom(entity => entity.ShippingRequests.CreationTime))
  .ForMember(dto => dto.Source, options => options.MapFrom(entity => entity.ShippingRequests.OriginCityFk.DisplayName))
  .ForMember(dto => dto.Destination, options => options.MapFrom(entity => entity.ShippingRequests.DestinationCityFk));


            configuration.CreateMap<TACHYON.Invoices.Transactions.Transaction, TransactionListDto>()
                .ForMember(dto => dto.ClientName, options => options.MapFrom(entity => entity.Tenant.Name))
                 .ForMember(dto => dto.Edition, options => options.MapFrom(entity => entity.Tenant.Edition.DisplayName))
                 .ForMember(dto => dto.EditionId, options => options.MapFrom(entity => entity.Tenant.Edition.Id))
                //.ForMember(dto => dto.Channel, options => options.MapFrom(entity => Enum.GetName(typeof(ChannelType), entity.ChannelId)))
                ;

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
            configuration.CreateMap(typeof(TACHYONAppServiceBase.TachyonLoadResult<>), typeof(LoadResult)).ReverseMap();
        }
        /// <summary>
        /// MultiLingualMapping configuration 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        public static void CreateMultiLingualMappings(IMapperConfigurationExpression configuration, MultiLingualMapContext context)
        {

            //configuration.CreateMultiLingualMap<DocumentType, long, DocumentTypeTranslation, DocumentTypeDto>(context)
            //    .EntityMap
            //    .ForMember(dst => dst.RequiredFrom, opt => opt.MapFrom(src => src.DocumentsEntityFk.DisplayName))
            //    .ForMember(dst => dst.Edition, opt => opt.MapFrom(src => src.EditionFk.DisplayName))
            //    .ForMember(dst => dst.DocumentRelatedWithName, opt => opt.MapFrom(src => src.DocumentRelatedWithFk.TenancyName))
            //    .ReverseMap();

            configuration.CreateMultiLingualMap<TermAndCondition, TermAndConditionTranslation, TermAndConditionDto>(context)
                 .EntityMap.ForMember(dst => dst.EditionName, opt => opt.MapFrom(src => src.EditionFk.DisplayName))
                 .ReverseMap();

            configuration.CreateMultiLingualMap<TransportType, TransportTypesTranslation, TransportTypeDto>(context)
                .EntityMap
                .ReverseMap();

            // #Map_TransportType_TransportTypeSelectItemDto
            configuration.CreateMultiLingualMap<TransportType, TransportTypesTranslation, TransportTypeSelectItemDto>(context)
                  .EntityMap
                  .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                  .ReverseMap();

            configuration.CreateMultiLingualMap<County, CountriesTranslation, CountyDto>(context)
                .EntityMap
                .ReverseMap();

            configuration.CreateMultiLingualMap<County, CountriesTranslation, TenantCountryLookupTableDto>(context)
                .EntityMap
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ReverseMap();

            configuration.CreateMultiLingualMap<City, CitiesTranslation, CityDto>(context)
                .EntityMap
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ReverseMap();

            configuration.CreateMultiLingualMap<City, CitiesTranslation, TenantCityLookupTableDto>(context)
                .EntityMap
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ReverseMap();

            configuration.CreateMultiLingualMap<TruckStatus, long, TruckStatusesTranslation, TruckStatusDto>(context)
                .EntityMap
                .ReverseMap();

            // goto:#Map_TruckStatus_TruckTruckStatusLookupTableDto
            configuration.CreateMultiLingualMap<TruckStatus, long, TruckStatusesTranslation, TruckTruckStatusLookupTableDto>(context)
                .EntityMap
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ReverseMap();

            configuration.CreateMultiLingualMap<Capacity, TruckCapacitiesTranslation, CapacityDto>(context)
                .EntityMap
                .ReverseMap();

            // goto:#Map_Capacity_CapacitySelectItemDto
            configuration.CreateMultiLingualMap<Capacity, TruckCapacitiesTranslation, CapacitySelectItemDto>(context)
                .EntityMap
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ReverseMap();

            configuration.CreateMultiLingualMap<TrucksType, long, TrucksTypesTranslation, TrucksTypeDto>(context)
                .EntityMap
                .ReverseMap();

            configuration.CreateMultiLingualMap<TrucksType, long, TrucksTypesTranslation, TrucksTypeSelectItemDto>(context)
                .EntityMap
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ReverseMap();

            //configuration.CreateMultiLingualMap<Vas, VasTranslation, VasDto>(context)
            //    .EntityMap.ForMember(x => x.VasTranslation,
            //        x =>
            //            x.MapFrom(i => i.Translations));

            configuration.
                CreateMultiLingualMap<ShippingRequestReasonAccident, ShippingRequestReasonAccidentTranslation, ShippingRequestReasonAccidentListDto>(context);
            configuration.
                CreateMultiLingualMap<ShippingRequestTripRejectReason, ShippingRequestTripRejectReasonTranslation, ShippingRequestTripRejectReasonListDto>(context);
            configuration.
                CreateMultiLingualMap<AppLocalization, AppLocalizationTranslation, AppLocalizationListDto>(context);

            configuration.
                CreateMultiLingualMap<PlateType, PlateTypeTranslation, PlateTypeDto>(context);
            configuration.
                CreateMultiLingualMap<PlateType, PlateTypeTranslation, PlateTypeSelectItemDto>(context);
            configuration.
                CreateMultiLingualMap<GoodCategory, GoodCategoryTranslation, GetAllGoodsCategoriesForDropDownOutput>(context);
            //configuration.
            //    CreateMultiLingualMap<GoodCategory, GoodCategoryTranslation, GoodCategoryDto>(context);
        }

        private static void AddOrUpdateShippingRequest(CreateOrEditShippingRequestDto dto, ShippingRequest Request)
        {
            if (Request.ShippingRequestVases == null) Request.ShippingRequestVases = new Collection<ShippingRequestVas>();
            foreach (var vas in dto.ShippingRequestVasList)
            {

                if (!vas.Id.HasValue)
                {
                    Request.ShippingRequestVases.Add(_Mapper.Map<ShippingRequestVas>(vas));
                }
                else
                {
                    _Mapper.Map(vas, Request.ShippingRequestVases.SingleOrDefault(c => c.Id == vas.Id));
                }
            }

        }

        private static void AddOrUpdateShippingRequest(EditShippingRequestStep4Dto dto, ShippingRequest Request)
        {
            if (Request.ShippingRequestVases == null) Request.ShippingRequestVases = new Collection<ShippingRequestVas>();
            foreach (var vas in dto.ShippingRequestVasList)
            {

                if (!vas.Id.HasValue)
                {
                    Request.ShippingRequestVases.Add(_Mapper.Map<ShippingRequestVas>(vas));
                }
                else
                {
                    _Mapper.Map(vas, Request.ShippingRequestVases.SingleOrDefault(c => c.Id == vas.Id));
                }
            }

        }

        private static void AddOrUpdateShippingRequestTrip(CreateOrEditShippingRequestTripDto dto, ShippingRequestTrip trip)
        {
            if (dto.RoutPoints != null)
            {


                //map Points 
                if (trip.RoutPoints == null) trip.RoutPoints = new Collection<RoutPoint>();
                foreach (var routPoint in dto.RoutPoints)
                {
                    //Add
                    if (!routPoint.Id.HasValue)
                    {
                        trip.RoutPoints.Add(_Mapper.Map<RoutPoint>(routPoint));

                    }

                    //update
                    else
                    {
                        _Mapper.Map(routPoint, trip.RoutPoints.SingleOrDefault(c => c.Id == routPoint.Id));
                    }
                }
            }
            if (trip.ShippingRequestTripVases == null) trip.ShippingRequestTripVases = new Collection<ShippingRequestTripVas>();
            //map Vases
            if (dto.ShippingRequestTripVases != null)
            {
                foreach (var vas in dto.ShippingRequestTripVases)
                {
                    //Add
                    if (!vas.Id.HasValue)
                    {
                        trip.ShippingRequestTripVases.Add(_Mapper.Map<ShippingRequestTripVas>(vas));
                    }

                    //update
                    else
                    {
                        _Mapper.Map(vas, trip.ShippingRequestTripVases.SingleOrDefault(c => c.Id == vas.Id));
                    }
                }
            }

        }

        private static void AddOrUpdateShippingRequestTripRoutePointGoods(CreateOrEditRoutPointDto dto, RoutPoint point)
        {
            if (dto.GoodsDetailListDto == null) return;
            if (point.GoodsDetails == null) point.GoodsDetails = new Collection<GoodsDetail>();
            foreach (var good in dto.GoodsDetailListDto)
            {
                //Add
                if (!good.Id.HasValue)
                {
                    point.GoodsDetails.Add(_Mapper.Map<GoodsDetail>(good));

                }

                //update
                else
                {
                    _Mapper.Map(good, point.GoodsDetails.SingleOrDefault(c => c.Id == good.Id));
                }

            }

        }



    }
}