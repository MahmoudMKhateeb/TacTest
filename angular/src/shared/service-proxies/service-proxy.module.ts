﻿import { AbpHttpConfigurationService, AbpHttpInterceptor, RefreshTokenService } from 'abp-ng2-module';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import * as ApiServiceProxies from './service-proxies';
import { ZeroRefreshTokenService } from '@account/auth/zero-refresh-token.service';
import { ZeroTemplateHttpConfigurationService } from './zero-template-http-configuration.service';

@NgModule({
  providers: [
    ApiServiceProxies.RedemptionCodesServiceProxy,
    ApiServiceProxies.RedeemCodesServiceProxy,
    ApiServiceProxies.BayanIntegrationResultsServiceProxy,
    ApiServiceProxies.RegionsServiceProxy,
    ApiServiceProxies.ActorInvoiceServiceProxy,
    ApiServiceProxies.ActorSubmitInvoiceServiceProxy,
    ApiServiceProxies.ActorsPriceOffersServiceProxy,
    ApiServiceProxies.ActorsServiceProxy,
    ApiServiceProxies.EmailTemplatesServiceProxy,
    ApiServiceProxies.DriverLicenseTypesServiceProxy,
    ApiServiceProxies.DangerousGoodTypesServiceProxy,
    ApiServiceProxies.TruckCapacitiesTranslationsServiceProxy,
    ApiServiceProxies.TruckStatusesTranslationsServiceProxy,
    ApiServiceProxies.CitiesTranslationsServiceProxy,
    ApiServiceProxies.CountriesTranslationsServiceProxy,
    ApiServiceProxies.PlateTypesServiceProxy,
    ApiServiceProxies.NationalitiesServiceProxy,
    ApiServiceProxies.NationalityTranslationsServiceProxy,
    ApiServiceProxies.TransportTypesTranslationsServiceProxy,
    ApiServiceProxies.TrucksTypesTranslationsServiceProxy,
    ApiServiceProxies.TransportTypesTranslationsServiceProxy,
    ApiServiceProxies.ReceiversServiceProxy,
    ApiServiceProxies.NationalitiesServiceProxy,
    ApiServiceProxies.NationalityTranslationsServiceProxy,
    ApiServiceProxies.TransportTypesTranslationsServiceProxy,
    ApiServiceProxies.WaybillsServiceProxy,
    ApiServiceProxies.TruckCapacitiesTranslationsServiceProxy,
    ApiServiceProxies.TruckStatusesTranslationsServiceProxy,
    ApiServiceProxies.CitiesTranslationsServiceProxy,
    ApiServiceProxies.CountriesTranslationsServiceProxy,
    ApiServiceProxies.PlateTypesServiceProxy,
    ApiServiceProxies.NationalitiesServiceProxy,
    ApiServiceProxies.NationalityTranslationsServiceProxy,
    ApiServiceProxies.TransportTypesTranslationsServiceProxy,
    ApiServiceProxies.CitiesTranslationsServiceProxy,
    ApiServiceProxies.CountriesTranslationsServiceProxy,
    ApiServiceProxies.PlateTypesServiceProxy,
    ApiServiceProxies.TruckCapacitiesTranslationsServiceProxy,
    ApiServiceProxies.TruckStatusesTranslationsServiceProxy,
    ApiServiceProxies.NationalitiesServiceProxy,
    ApiServiceProxies.NationalityTranslationsServiceProxy,
    ApiServiceProxies.TransportTypesTranslationsServiceProxy,
    ApiServiceProxies.TrucksTypesTranslationsServiceProxy,
    ApiServiceProxies.TransportTypesTranslationsServiceProxy,
    ApiServiceProxies.PackingTypesServiceProxy,
    ApiServiceProxies.ShippingTypesServiceProxy,
    ApiServiceProxies.NationalitiesServiceProxy,
    ApiServiceProxies.NationalityTranslationsServiceProxy,
    ApiServiceProxies.TransportTypesTranslationsServiceProxy,
    ApiServiceProxies.VasPricesServiceProxy,
    ApiServiceProxies.VasesServiceProxy,
    ApiServiceProxies.TermAndConditionTranslationsServiceProxy,
    ApiServiceProxies.TermAndConditionsServiceProxy,
    ApiServiceProxies.CapacitiesServiceProxy,
    ApiServiceProxies.TransportTypesServiceProxy,
    ApiServiceProxies.DocumentTypeTranslationsServiceProxy,
    ApiServiceProxies.ShippingRequestStatusesServiceProxy,
    ApiServiceProxies.PortsServiceProxy,
    ApiServiceProxies.PickingTypesServiceProxy,
    ApiServiceProxies.UnitOfMeasuresServiceProxy,
    ApiServiceProxies.FacilitiesServiceProxy,
    ApiServiceProxies.DocumentFilesServiceProxy,
    ApiServiceProxies.DocumentTypesServiceProxy,
    ApiServiceProxies.ShippingRequestsServiceProxy,
    ApiServiceProxies.DedicatedShippingRequestsServiceProxy,
    ApiServiceProxies.GoodsDetailsServiceProxy,
    ApiServiceProxies.OffersServiceProxy,
    ApiServiceProxies.RoutStepsServiceProxy,
    ApiServiceProxies.RoutesServiceProxy,
    ApiServiceProxies.CitiesServiceProxy,
    ApiServiceProxies.CountiesServiceProxy,
    ApiServiceProxies.RoutTypesServiceProxy,
    ApiServiceProxies.GoodCategoriesServiceProxy,
    ApiServiceProxies.TrailersServiceProxy,
    ApiServiceProxies.TrailerStatusesServiceProxy,
    ApiServiceProxies.PayloadMaxWeightsServiceProxy,
    ApiServiceProxies.TrailerTypesServiceProxy,
    ApiServiceProxies.TrucksServiceProxy,
    ApiServiceProxies.TrucksTypesServiceProxy,
    ApiServiceProxies.TruckStatusesServiceProxy,
    ApiServiceProxies.AuditLogServiceProxy,
    ApiServiceProxies.CachingServiceProxy,
    ApiServiceProxies.ChatServiceProxy,
    ApiServiceProxies.CommonLookupServiceProxy,
    ApiServiceProxies.EditionServiceProxy,
    ApiServiceProxies.FriendshipServiceProxy,
    ApiServiceProxies.HostSettingsServiceProxy,
    ApiServiceProxies.InstallServiceProxy,
    ApiServiceProxies.LanguageServiceProxy,
    ApiServiceProxies.NotificationServiceProxy,
    ApiServiceProxies.OrganizationUnitServiceProxy,
    ApiServiceProxies.PermissionServiceProxy,
    ApiServiceProxies.ProfileServiceProxy,
    ApiServiceProxies.RoleServiceProxy,
    ApiServiceProxies.SessionServiceProxy,
    ApiServiceProxies.TenantServiceProxy,
    ApiServiceProxies.TenantDashboardServiceProxy,
    ApiServiceProxies.TenantSettingsServiceProxy,
    ApiServiceProxies.TimingServiceProxy,
    ApiServiceProxies.UserServiceProxy,
    ApiServiceProxies.InvoiceNoteServiceProxy,
    ApiServiceProxies.UserLinkServiceProxy,
    ApiServiceProxies.UserLoginServiceProxy,
    ApiServiceProxies.WebLogServiceProxy,
    ApiServiceProxies.AccountServiceProxy,
    ApiServiceProxies.TokenAuthServiceProxy,
    ApiServiceProxies.TenantRegistrationServiceProxy,
    ApiServiceProxies.HostDashboardServiceProxy,
    ApiServiceProxies.PaymentServiceProxy,
    ApiServiceProxies.DemoUiComponentsServiceProxy,
    ApiServiceProxies.InvoiceServiceProxy,
    ApiServiceProxies.SubscriptionServiceProxy,
    ApiServiceProxies.InstallServiceProxy,
    ApiServiceProxies.UiCustomizationSettingsServiceProxy,
    ApiServiceProxies.PayPalPaymentServiceProxy,
    ApiServiceProxies.StripePaymentServiceProxy,
    ApiServiceProxies.DashboardCustomizationServiceProxy,
    ApiServiceProxies.WebhookEventServiceProxy,
    ApiServiceProxies.WebhookSubscriptionServiceProxy,
    ApiServiceProxies.WebhookSendAttemptServiceProxy,
    ApiServiceProxies.UserDelegationServiceProxy,
    ApiServiceProxies.DynamicParameterServiceProxy,
    ApiServiceProxies.DynamicEntityParameterDefinitionServiceProxy,
    ApiServiceProxies.EntityDynamicParameterServiceProxy,
    ApiServiceProxies.DynamicParameterValueServiceProxy,
    ApiServiceProxies.EntityDynamicParameterValueServiceProxy,
    ApiServiceProxies.InvoicePeriodServiceProxy,
    ApiServiceProxies.PenaltiesServiceProxy,
    ApiServiceProxies.InvoiceServiceProxy,
    ApiServiceProxies.BalanceRechargeServiceProxy,
    ApiServiceProxies.SubmitInvoicesServiceProxy,
    ApiServiceProxies.TransactionServiceProxy,
    ApiServiceProxies.ShippingRequestsTripServiceProxy,
    ApiServiceProxies.ShippingRequestReasonAccidentServiceProxy,
    ApiServiceProxies.ShippingRequestTripAccidentServiceProxy,
    ApiServiceProxies.ShippingRequestTripRejectReasonServiceProxy,
    ApiServiceProxies.InvoiceReportServiceServiceProxy,
    ApiServiceProxies.AppLocalizationServiceProxy,
    ApiServiceProxies.TenantCarrierServiceProxy,
    ApiServiceProxies.ShippingRequestDirectRequestServiceProxy,
    ApiServiceProxies.PriceOfferServiceProxy,
    ApiServiceProxies.InvoicePaymentMethodServiceProxy,
    ApiServiceProxies.ShippingRequestsTachyonDealerServiceProxy,
    ApiServiceProxies.InvoicesProformaServiceProxy,
    ApiServiceProxies.TrackingServiceProxy,
    ApiServiceProxies.RatingServiceProxy,
    ApiServiceProxies.ShippingRequestDriverServiceProxy,
    ApiServiceProxies.DriverLicenseTypesServiceProxy,
    ApiServiceProxies.ShippingRequestTripAccidentCommentsServiceProxy,
    ApiServiceProxies.CarrierDashboardServiceProxy,
    ApiServiceProxies.EntityLogServiceProxy,
    ApiServiceProxies.ShipperDashboardServiceProxy,
    ApiServiceProxies.EntityTemplateServiceProxy,
    ApiServiceProxies.SrPostPriceUpdateServiceProxy,
    ApiServiceProxies.ImportShipmentFromExcelServiceProxy,
    ApiServiceProxies.ShippingRequestAndTripNotesServiceProxy,
    ApiServiceProxies.ShippingRequestUpdateServiceProxy,
    ApiServiceProxies.DynamicInvoiceServiceProxy,
    ApiServiceProxies.PricePackageServiceProxy,
    ApiServiceProxies.PricePackageProposalServiceProxy,
    ApiServiceProxies.TruckAttendancesServiceProxy,
    ApiServiceProxies.DedicatedDynamiceInvoicesServiceProxy,
    ApiServiceProxies.PricePackageAppendixServiceProxy,
    ApiServiceProxies.DedicatedDynamicActorInvoicesServiceProxy,
    ApiServiceProxies.BrokerDashboardServiceProxy,
    ApiServiceProxies.TMSAndHostDashboardServiceProxy,
    ApiServiceProxies.ForceDeliverTripsServiceProxy,
    ApiServiceProxies.ReportDefinitionServiceProxy,
    ApiServiceProxies.ReportServiceProxy,
    ApiServiceProxies.SaasPricePackageServiceProxy,
    ApiServiceProxies.TripDriversServiceProxy,

    { provide: RefreshTokenService, useClass: ZeroRefreshTokenService },
    { provide: AbpHttpConfigurationService, useClass: ZeroTemplateHttpConfigurationService },
    { provide: HTTP_INTERCEPTORS, useClass: AbpHttpInterceptor, multi: true },
  ],
})
export class ServiceProxyModule {}
