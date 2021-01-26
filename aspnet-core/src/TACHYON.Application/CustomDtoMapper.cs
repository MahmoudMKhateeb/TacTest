using TACHYON.Trucks.PlateTypes.Dtos;
using TACHYON.Trucks.PlateTypes;
using TACHYON.Nationalities.Dtos;
using TACHYON.Nationalities;
using TACHYON.Nationalities.NationalitiesTranslation.Dtos;
using TACHYON.Nationalities.NationalitiesTranslation;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes.TransportTypesTranslations;
using TACHYON.ShippingRequestVases.Dtos;
using TACHYON.ShippingRequestVases;
using TACHYON.Vases.Dtos;
using TACHYON.Vases;
using TACHYON.TermsAndConditions;
using TACHYON.TermsAndConditions.Dtos;
using TACHYON.Trucks.TruckCategories.TruckCapacities;
using TACHYON.Trucks.TruckCategories.TransportTypes.Dtos;
using TACHYON.Trucks.TruckCategories.TransportTypes;
using TACHYON.Documents.DocumentTypeTranslations.Dtos;
using TACHYON.Documents.DocumentTypeTranslations;
using TACHYON.Documents.DocumentsEntities.Dtos;
using TACHYON.Documents.DocumentsEntities;
using TACHYON.Shipping.ShippingRequestStatuses.Dtos;
using TACHYON.Shipping.ShippingRequestStatuses;
using TACHYON.PickingTypes.Dtos;
using TACHYON.PickingTypes;
using TACHYON.AddressBook.Ports.Dtos;
using TACHYON.AddressBook.Ports;
using TACHYON.UnitOfMeasures.Dtos;
using TACHYON.UnitOfMeasures;
using TACHYON.AddressBook.Dtos;
using TACHYON.AddressBook;
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
using TACHYON.Cities;
using TACHYON.Cities.Dtos;
using TACHYON.Cities.Dtos;
using TACHYON.Countries;
using TACHYON.Countries;
using TACHYON.Countries.Dtos;
using TACHYON.Countries.Dtos;
using TACHYON.Documents.DocumentFiles;
using TACHYON.Documents.DocumentFiles.Dtos;
using TACHYON.Documents.DocumentTypes;
using TACHYON.Documents.DocumentTypes.Dtos;
using TACHYON.Drivers.importing.Dto;
using TACHYON.DynamicEntityParameters.Dto;
using TACHYON.Editions;
using TACHYON.Editions.Dto;
using TACHYON.Friendships;
using TACHYON.Friendships.Cache;
using TACHYON.Friendships.Dto;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Goods.GoodCategories.Dtos;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Goods.GoodsDetails;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Goods.GoodsDetails.Dtos;
using TACHYON.Localization.Dto;
using TACHYON.MultiTenancy;
using TACHYON.MultiTenancy.Dto;
using TACHYON.MultiTenancy.HostDashboard.Dto;
using TACHYON.MultiTenancy.Payments;
using TACHYON.MultiTenancy.Payments.Dto;
using TACHYON.Notifications.Dto;
using TACHYON.Offers;
using TACHYON.Offers;
using TACHYON.Offers.Dtos;
using TACHYON.Offers.Dtos;
using TACHYON.Organizations.Dto;
using TACHYON.Routs;
using TACHYON.Routs;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.Dtos;
using TACHYON.Routs.RoutSteps;
using TACHYON.Routs.RoutSteps;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Routs.RoutSteps.Dtos;
using TACHYON.Routs.RoutTypes;
using TACHYON.Routs.RoutTypes;
using TACHYON.Routs.RoutTypes.Dtos;
using TACHYON.Routs.RoutTypes.Dtos;
using TACHYON.Sessions.Dto;
using TACHYON.Shipping.ShippingRequestBids;
using TACHYON.Shipping.ShippingRequestBids.Dtos;
using TACHYON.Shipping.ShippingRequests;
using TACHYON.Shipping.ShippingRequests.Dtos;
using TACHYON.Trailers;
using TACHYON.Trailers;
using TACHYON.Trailers.Dtos;
using TACHYON.Trailers.Dtos;
using TACHYON.Trailers.PayloadMaxWeights;
using TACHYON.Trailers.PayloadMaxWeights.Dtos;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerStatuses;
using TACHYON.Trailers.TrailerStatuses.Dtos;
using TACHYON.Trailers.TrailerStatuses.Dtos;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trailers.TrailerTypes;
using TACHYON.Trailers.TrailerTypes.Dtos;
using TACHYON.Trailers.TrailerTypes.Dtos;
using TACHYON.Trucks;
using TACHYON.Trucks;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.Dtos;
using TACHYON.Trucks.Importing.Dto;
using TACHYON.Trucks.TruckCategories.TruckCapacities.Dtos;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.Trucks.TrucksTypes.Dtos;
using TACHYON.WebHooks.Dto;

namespace TACHYON
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditPlateTypeDto, PlateType>().ReverseMap();
            configuration.CreateMap<PlateTypeDto, PlateType>().ReverseMap();
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
            //configuration.CreateMap<TermAndConditionDto, TermAndCondition>().ReverseMap();
            configuration.CreateMap<CreateOrEditCapacityDto, Capacity>().ReverseMap();
            configuration.CreateMap<CapacityDto, Capacity>().ReverseMap();
            configuration.CreateMap<CreateOrEditTransportTypeDto, TransportType>().ReverseMap();
            configuration.CreateMap<TransportTypeDto, TransportType>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentTypeTranslationDto, DocumentTypeTranslation>().ReverseMap();
            configuration.CreateMap<DocumentTypeTranslationDto, DocumentTypeTranslation>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentsEntityDto, DocumentsEntity>().ReverseMap();
            configuration.CreateMap<DocumentFileForCreateOrEditDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<DocumentsEntityDto, DocumentsEntity>().ReverseMap();
            configuration.CreateMap<CreateOrEditShippingRequestStatusDto, ShippingRequestStatus>().ReverseMap();
            configuration.CreateMap<ShippingRequestStatusDto, ShippingRequestStatus>().ReverseMap();
            configuration.CreateMap<CreateOrEditPickingTypeDto, PickingType>().ReverseMap();
            configuration.CreateMap<PickingTypeDto, PickingType>().ReverseMap();
            configuration.CreateMap<CreateOrEditPortDto, Port>().ReverseMap();
            configuration.CreateMap<PortDto, Port>().ReverseMap();
            configuration.CreateMap<CreateOrEditUnitOfMeasureDto, UnitOfMeasure>().ReverseMap();
            configuration.CreateMap<UnitOfMeasureDto, UnitOfMeasure>().ReverseMap();
            configuration.CreateMap<CreateOrEditFacilityDto, Facility>().ReverseMap();
            configuration.CreateMap<FacilityDto, Facility>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<DocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<ImportTruckDocumentFileDto, DocumentFile>().ReverseMap();
            configuration.CreateMap<CreateOrEditDocumentTypeDto, DocumentType>().ReverseMap();
            //configuration.CreateMap<DocumentTypeDto, DocumentType>()
            //    .ForPath(dst => dst.DocumentsEntityFk.DisplayName, opt => opt.MapFrom(src => src.RequiredFrom))
            //    .ForPath(dst => dst.EditionFk.DisplayName, opt => opt.MapFrom(src => src.Edition))
            //    .ReverseMap();
            configuration.CreateMap<CreateOrEditShippingRequestDto, ShippingRequest>()
                .ForMember(dst => dst.RouteFk, opt => opt.MapFrom(src => src.CreateOrEditRouteDto))
                .ForMember(dst => dst.RoutSteps, opt => opt.MapFrom(src => src.CreateOrEditRoutStepDtoList))
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
            configuration.CreateMap<CreateOrEditRoutStepDto, RoutStep>().ReverseMap();
            configuration.CreateMap<RoutStepDto, RoutStep>().ReverseMap();
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
    }
}