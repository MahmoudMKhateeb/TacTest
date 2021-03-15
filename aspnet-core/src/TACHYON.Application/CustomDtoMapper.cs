using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.DynamicEntityParameters;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using Abp.Webhooks;
using AutoMapper;
using System;
using System.Collections.ObjectModel;
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
using TACHYON.Cities.Dtos;
using TACHYON.Countries;
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
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Goods.GoodsDetails.Dtos;
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
using TACHYON.PickingTypes;
using TACHYON.PickingTypes.Dtos;
using TACHYON.Routs;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutPoints;
using TACHYON.Routs.RoutPoints.Dtos;
using TACHYON.Routs.RoutSteps;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Routs.RoutTypes;
using TACHYON.Routs.RoutTypes.Dtos;
using TACHYON.Sessions.Dto;
using TACHYON.Shipping.Drivers.Dto;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Shipping.ShippingRequestStatuses;
using TACHYON.Shipping.ShippingRequestStatuses.Dtos;
using TACHYON.Shipping.ShippingRequestTrips;
using TACHYON.Shipping.ShippingTypes;
using TACHYON.Shipping.ShippingTypes.Dtos;
using TACHYON.Shipping.Trips;
using TACHYON.Shipping.Trips.Dto;
using TACHYON.Shipping.TripStatuses;
using TACHYON.Shipping.TripStatuses.Dtos;
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
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
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
            #region Trips
                configuration.CreateMap<CreateOrEditTripStatusDto, TripStatus>().ReverseMap();
                configuration.CreateMap<TripStatusDto, TripStatus>().ReverseMap();

            configuration.CreateMap<ShippingRequestTrip,ShippingRequestsTripListDto>()
                 .ForMember(dst => dst.OriginFacility, opt => opt.MapFrom(src => src.OriginFacilityFk!=null ? $"{src.OriginFacilityFk.Name} - {src.OriginFacilityFk.Address}" :""))
                 .ForMember(dst => dst.DestinationFacility, opt => opt.MapFrom(src => src.DestinationFacilityFk!=null ?$"{src.DestinationFacilityFk.Name} - {src.DestinationFacilityFk.Address}" :""))
                 .ForMember(dst => dst.Truck, opt => opt.MapFrom(src => src.AssignedTruckFk!=null ?src.AssignedTruckFk.ModelName :string.Empty))
                 .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk!=null ?src.AssignedDriverUserFk.Name :string.Empty))
                 .ForMember(dst => dst.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))

                 .ReverseMap();
            configuration.CreateMap<ShippingRequestTrip, ShippingRequestsTripForViewDto>()
                 .ForMember(dst => dst.OriginFacility, opt => opt.MapFrom(src => src.OriginFacilityFk != null ? $"{src.OriginFacilityFk.Name} - {src.OriginFacilityFk.Address}" : ""))
                 .ForMember(dst => dst.DestinationFacility, opt => opt.MapFrom(src => src.DestinationFacilityFk != null ? $"{src.DestinationFacilityFk.Name} - {src.DestinationFacilityFk.Address}" : ""))
                 .ForMember(dst => dst.Truck, opt => opt.MapFrom(src => src.AssignedTruckFk != null ? src.AssignedTruckFk.ModelName : string.Empty))
                 .ForMember(dst => dst.Driver, opt => opt.MapFrom(src => src.AssignedDriverUserFk != null ? src.AssignedDriverUserFk.Name : string.Empty))
                 .ForMember(dst => dst.Status, opt => opt.MapFrom(src => Enum.GetName(typeof(ShippingRequestTripStatus), src.Status)))
                .ReverseMap();

            
            configuration.CreateMap<ShippingRequestTrip, ShippingRequestTripDriverListDto>()
                .ForMember(dst => dst.Source, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.RouteFk.OriginCityFk.DisplayName} - {src.OriginFacilityFk.Address}"))
                .ForMember(dst => dst.Distination, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.RouteFk.DestinationCityFk.DisplayName} - {src.DestinationFacilityFk.Address}"))
                .ForMember(dst => dst.RoutTypeId, opt => opt.MapFrom(src => src.ShippingRequestFk.RouteFk.RoutTypeId))
                .ForMember(dst => dst.StartDate, opt => opt.MapFrom(src => src.StartTripDate))
                .ForMember(dst => dst.EndDate, opt => opt.MapFrom(src => src.EndTripDate))
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status));

            configuration.CreateMap<ShippingRequestTrip, ShippingRequestTripDriverDetailsDto>()
            .ForMember(dst => dst.Source, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.RouteFk.OriginCityFk.DisplayName} - {src.OriginFacilityFk.Address}"))
            .ForMember(dst => dst.Distination, opt => opt.MapFrom(src => $"{src.ShippingRequestFk.RouteFk.DestinationCityFk.DisplayName} - {src.DestinationFacilityFk.Address}"))
            .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dst => dst.TotalWeight, opt => opt.MapFrom(src => src.ShippingRequestFk.TotalWeight))
            .ForMember(dst => dst.PackingType, opt => opt.MapFrom(src => src.ShippingRequestFk.PackingTypeFk.DisplayName))
            .ForMember(dst => dst.RoutePoints, opt => opt.MapFrom(src => src.RoutPoints));


            configuration.CreateMap<RoutPoint, ShippingRequestTripDriverRoutePointDto>()
            .ForMember(dst => dst.Address, opt => opt.MapFrom(src => $"{src.FacilityFk.Name} - {src.FacilityFk.Address}"))
            .ForMember(dst => dst.Location, opt => opt.MapFrom(src => src.FacilityFk.Location));
            #endregion

            configuration.CreateMap<CreateOrEditPackingTypeDto, PackingType>().ReverseMap();
            configuration.CreateMap<PackingTypeDto, PackingType>().ReverseMap();
            configuration.CreateMap<CreateOrEditShippingTypeDto, ShippingType>().ReverseMap();
            configuration.CreateMap<ShippingTypeDto, ShippingType>().ReverseMap();
            configuration.CreateMap<CreateOrEditNationalityDto, Nationality>().ReverseMap();
            configuration.CreateMap<NationalityDto, Nationality>().ReverseMap();
            configuration.CreateMap<CreateOrEditNationalityTranslationDto, NationalityTranslation>().ReverseMap();
            configuration.CreateMap<NationalityTranslationDto, NationalityTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditTransportTypesTranslationDto, TransportTypesTranslation>().ReverseMap();
            configuration.CreateMap<TransportTypesTranslationDto, TransportTypesTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditShippingRequestVasDto, ShippingRequestVas>().ReverseMap();
            configuration.CreateMap<ShippingRequestVasDto, ShippingRequestVas>().ReverseMap();
            configuration.CreateMap<CreateOrEditVasPriceDto, VasPrice>().ReverseMap();
            configuration.CreateMap<VasPriceDto, VasPrice>().ReverseMap();
            configuration.CreateMap<CreateOrEditVasDto, Vas>().ReverseMap();
            configuration.CreateMap<VasDto, Vas>().ReverseMap();
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
            configuration.CreateMap<FacilityDto, Facility>()
                 .ForPath(dst => dst.Location.X, opt => opt.MapFrom(src => src.Longitude))
                 .ForPath(dst => dst.Location.Y, opt => opt.MapFrom(src => src.Latitude))
                .ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<DocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<ImportTruckDocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentTypeDto, DocumentType>().ReverseMap();

            
            configuration.CreateMap<CreateOrEditShippingRequestVasListDto, ShippingRequestVas>().ReverseMap();
            configuration.CreateMap<CreateOrEditRouteDto, Route>().ReverseMap();

            configuration.CreateMap<CreateOrEditShippingRequestDto, ShippingRequest>()
                .ForMember(d => d.ShippingRequestVases, opt => opt.Ignore())
            .AfterMap(AddOrUpdateShippingRequest)
                .ReverseMap();

            configuration.CreateMap<CreateOrEditShippingRequestTripDto, ShippingRequestTrip>()
                .ForMember(d=>d.RoutPoints,opt=>opt.Ignore())
                .ForMember(d => d.ShippingRequestTripVases, opt => opt.Ignore())
                .AfterMap(AddOrUpdateShippingRequestTrip);
            

            configuration.CreateMap<ShippingRequestTrip,CreateOrEditShippingRequestTripDto>()
                .ForMember(dest => dest.RoutPoints, opt => opt.MapFrom(src => src.RoutPoints))
                .ForMember(dest => dest.ShippingRequestTripVases, opt => opt.MapFrom(src => src.ShippingRequestTripVases));


            configuration.CreateMap<ShippingRequestTripVas,CreateOrEditShippingRequestTripVasDto>()
                   .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ShippingRequestVasFk.VasFk.DisplayName))
                .ReverseMap();
            configuration.CreateMap<ShippingRequestTripVas,ShippingRequestTripVasDto>()
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ShippingRequestVasFk.VasFk.Name))
             .ReverseMap();

            configuration.CreateMap<ShippingRequestVasListOutput, ShippingRequestVas>()
                .ReverseMap();
            configuration.CreateMap<CreateOrEditShippingRequestVasListDto, ShippingRequestVas>()
                .ReverseMap();
            configuration.CreateMap<CreateOrEditShippingRequestVasListDto, ShippingRequestVas>()
                .ReverseMap();
            
            configuration.CreateMap<ShippingRequestBidDto, ShippingRequestBid>()
                .ForPath(dst => dst.Tenant.Name, opt => opt.MapFrom(src => src.CarrierName))
                .ReverseMap();
            configuration.CreateMap<CreatOrEditShippingRequestBidDto, ShippingRequestBid>().ReverseMap();
            configuration.CreateMap<ShippingRequestDto, ShippingRequest>().ReverseMap();
            configuration.CreateMap<CreateOrEditGoodsDetailDto, GoodsDetail>().ReverseMap();
            configuration.CreateMap<GoodsDetailDto, GoodsDetail>().ReverseMap();
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
                .ReverseMap();

            configuration.CreateMap<CreateOrEditRoutPointDto, RoutPoint>()
                .AfterMap(AddOrUpdateShippingRequestTripRoutePointGoods);

            configuration.CreateMap<RoutPoint, CreateOrEditRoutPointDto>()
                .ForMember(dest => dest.GoodsDetailListDto, opt => opt.MapFrom(src => src.GoodsDetails))
                .ForPath(dest => dest.Longitude, opt => opt.MapFrom(src => src.FacilityFk.Location.X))
                .ForPath(dest => dest.Latitude, opt => opt.MapFrom(src => src.FacilityFk.Location.Y));

            configuration.CreateMap<CreateOrEditRouteDto, Route>().ReverseMap();
            configuration.CreateMap<RouteDto, Route>().ReverseMap();
            configuration.CreateMap<CreateOrEditCityDto, City>().ReverseMap();
            configuration.CreateMap<CityDto, City>().ReverseMap();
            configuration.CreateMap<CreateOrEditCountyDto, County>().ReverseMap();
            configuration.CreateMap<CountyDto, County>().ReverseMap();
            configuration.CreateMap<CreateOrEditRoutTypeDto, RoutType>().ReverseMap();
            configuration.CreateMap<RoutTypeDto, RoutType>().ReverseMap();
            configuration.CreateMap<CreateOrEditGoodCategoryDto, GoodCategory>().ReverseMap();
            configuration.CreateMap<GoodCategoryDto, GoodCategory>().ReverseMap();
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
            configuration.CreateMap<TruckDto, Truck>().ReverseMap();
            configuration.CreateMap<CreateOrEditTrucksTypeDto, TrucksType>().ReverseMap();
            configuration.CreateMap<TrucksTypeDto, TrucksType>().ReverseMap();
            configuration.CreateMap<CreateOrEditTruckStatusDto, TruckStatus>().ReverseMap();
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
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
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

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */
        }
        /// <summary>
        /// MultiLingualMapping configuration 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        public static void CreateMultiLingualMappings(IMapperConfigurationExpression configuration, MultiLingualMapContext context)
        {
            configuration.CreateMultiLingualMap<DocumentType, long, DocumentTypeTranslation, DocumentTypeDto>(context)
                .EntityMap.ForMember(dst => dst.RequiredFrom, opt => opt.MapFrom(src => src.DocumentsEntityFk.DisplayName))
                .ForMember(dst => dst.Edition, opt => opt.MapFrom(src => src.EditionFk.DisplayName))
                .ReverseMap();

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
        }

        private static void AddOrUpdateShippingRequest(CreateOrEditShippingRequestDto dto, ShippingRequest Request)
        {
            if (Request.ShippingRequestVases == null) Request.ShippingRequestVases = new Collection<ShippingRequestVas>();
            foreach (var vas in dto.ShippingRequestVasList)
            {

                if (!vas.Id.HasValue )
                {
                    Request.ShippingRequestVases.Add(_Mapper.Map<ShippingRequestVas>(vas));
                }
                else
                {
                    _Mapper.Map(vas, Request.ShippingRequestVases.SingleOrDefault(c => c.Id == vas.Id));
                }
            }

        }

        private static void AddOrUpdateShippingRequestTrip(CreateOrEditShippingRequestTripDto dto,ShippingRequestTrip trip)
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
            if (dto.ShippingRequestTripVases !=null)
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