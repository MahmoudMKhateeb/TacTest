import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from '../app-menu';
import { AppMenuItem } from '../app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { AppBaseNavigationService } from './app-base-navigation.service';

@Injectable({ providedIn: 'root' })
export class AppHostNavigationService extends AppBaseNavigationService {
  constructor(
    private _permissionCheckerService: PermissionCheckerService,
    private _appSessionService: AppSessionService,
    private _featureCheckerService: FeatureCheckerService
  ) {
    super(_permissionCheckerService, _appSessionService, _featureCheckerService);
  }

  getMenu(): AppMenu {
    console.log('AppHostNavigationService');
    let menu = new AppMenu('MainMenu', 'MainMenu', [
      new AppMenuItem(
        'Dashboard',
        '',
        'interaction, interact, preferences, preformance, computer, online, rating, review.svg',
        '/app/main/dashboard'
      ),
      // ! host item with shared Sub-menu
      // ---------------------------------------------------------------------------------------------------------------------
      //start of reports
      //for host only
      // ---------------------------------------------------------------------------------------------------------------------
      //TODO: Need Permission
      new AppMenuItem(
        'Reports',
        'Pages.Administration.Host.Dashboard',

        'warning, signs, sign, alert, truck.svg',
        '',
        [],
        [
          new AppMenuItem('AccidentReason', 'Pages.ShippingRequestResoneAccidents', '', '/app/main/accidents/reasons'),
          new AppMenuItem('TripRejectReason', 'Pages.ShippingRequestTrips.Reject.Reason', '', '/app/main/trip/reject/reasons'),
        ]
        // undefined,
        // undefined,
        // () => !this.isEnabled('App.TachyonDealer')
      ),
      //end of report
      // ---------------------------------------------------------------------------------------------------------------------
      //start of Invoices
      new AppMenuItem(
        'Financials',
        'Pages.Invoices',
        'shopping, shop, ecommerce, commerce, clipboard, finance.svg',
        '',
        [],
        [
          // TODO: add shipper and carrier invoices menu item
          new AppMenuItem('PenaltiesList', 'Pages.Invoices', '', '/app/main/penalties/view'),
          new AppMenuItem('InvoicesNoteList', 'Pages.Invoices', '', '/app/main/invoicenote/view'),
          new AppMenuItem(
            'InvoicesList',
            'Pages.Invoices',
            '',
            '/app/main/invoices/view',
            undefined,
            undefined,
            undefined,
            undefined,
            () =>
              this.isEnabled('App.Shipper') ||
              this.isEnabled('App.TachyonDealer') ||
              this.isEnabled('App.CarrierAsASaas') ||
              !isNotNullOrUndefined(this._appSessionService.tenantId)
          ),
          new AppMenuItem(
            'SubmitInvoiceForHost',
            //TODO make it Pages.Administration.invoices
            '',
            '',
            '/app/main/invoices/submitinvoice',
            undefined,
            undefined,
            undefined,
            undefined,
            () => !this._appSessionService.tenantId
          ),

          // new AppMenuItem(
          //   'InvoicingFrequency',
          //   'Pages.Invoices',
          //   '',
          //   '/app/main/invoices/periods',
          //   undefined,
          //   undefined,
          //   undefined,
          //   undefined,
          //   () => this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          // ),
          new AppMenuItem(
            'PaymentMethods',
            'Pages.Invoices',
            '',
            '/app/main/invoices/paymentlist',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),
          new AppMenuItem(
            'PaymentDue',
            'Pages.Invoices',
            '',
            '/app/main/invoices/periods',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),

          new AppMenuItem(
            'BalnaceRecharges',
            'Pages.Invoices',
            '',
            '/app/main/invoices/balnacerecharges',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),

          new AppMenuItem('FinancialTransActionMenu', 'Pages.Invoices.Transaction', '', '/app/main/invoices/transaction'),

          // new AppMenuItem('PenaltiesList', 'Pages.Invoices', '', '/app/main/penalties/view'),

          // new AppMenuItem('InvoicesNoteList', 'Pages.Invoices', '', '/app/main/invoicenote/view'),

          // new AppMenuItem(
          //   'SubmitInvoice',
          //   'Pages.Invoices',

          //   '',
          //   '/app/main/invoices/submitinvoice',
          //   undefined,
          //   undefined,
          //   undefined,
          //   undefined,
          //   () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
          // ),

          // // new AppMenuItem(
          // //   'InvoicesProformas',
          // //   'Pages.Invoices',
          // //   '',
          // //   '/app/main/invoices/proformas',
          // //   undefined,
          // //   undefined,
          // //   undefined,
          // //   undefined,
          // //   () => this.isEnabled('App.Shipper') || this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          // // ),
        ]
        // undefined,
        // undefined,
        // () => !this.isEnabled('App.TachyonDealer')
      ),
      //end of  Invoices
      // ---------------------------------------------------------------------------------------------------------------------
      //start of Documents
      new AppMenuItem(
        'DocumentManagement',
        '',
        'interaction, interact, preferences, preformance, customer, rating, rate, questions.svg',
        '',
        [],
        [
          new AppMenuItem('DocumentManagement', 'Pages.DocumentTypes', '', '/app/main/documentTypes/documentTypes'),
          new AppMenuItem('DocumentsEntities', 'Pages.DocumentsEntities', '', '/app/main/documentsEntities/documentsEntities'),
          //TODO: the contracts subMenu Need Permission and Route
          new AppMenuItem(
            'NonMandatoryDocuments',
            'Pages.DocumentFiles',
            '',
            '/tenantNotRequiredDocuments/tenantNotRequiredDocuments',
            [],
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem('SubmittedDocuments', 'Pages.DocumentFiles', '', '/app/main/documentFiles/documentFiles'),
          // new AppMenuItem(
          //   'TrucksSubmittedDocuments',
          //   'Pages.DocumentFiles',
          //   '',
          //   '/app/main/documentFiles/TrucksSubmittedDocuments',
          //   [],
          //   undefined,
          //   undefined,
          //   undefined,
          //   () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
          // ),
          // new AppMenuItem(
          //   'DriversSubmittedDocuments',
          //   'Pages.DocumentFiles',
          //   '',
          //   '/app/main/documentFiles/DriversSubmittedDocuments',
          //   [],
          //   undefined,
          //   undefined,
          //   undefined,
          //   () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
          // ),
          // //TODO: the contracts subMenu Need Permission and Route
          // new AppMenuItem(
          //   'contracts',
          //   'flaticon2-sheet',
          //   'flaticon-file',
          //   '/app/main/comingSoon',
          //   [],
          //   undefined,
          //   undefined,
          //   undefined,
          //   () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
          // ),

          // new AppMenuItem('TenantRequiredDocuments', '', 'flaticon-settings', '/app/admin/tenantRequiredDocuments'),
          new AppMenuItem(
            'ActorsSubmittedDocuments',
            'Pages.DocumentFiles.Actors',
            '',
            '/app/main/documentFiles/ActorsSubmittedDocuments',
            [],
            undefined,
            undefined,
            undefined
          ),
        ],
        undefined,
        undefined
      ),
      //end of Documents
      // ---------------------------------------------------------------------------------------------------------------------
      //start of Administration
      new AppMenuItem(
        'Administration',
        '',
        'content marketing, digital marketing, marketing, settings, options, key.svg',
        '',
        [],
        [
          new AppMenuItem('Tenants', 'Pages.Tenants', '', '/app/admin/tenants'),

          new AppMenuItem('Editions', 'Pages.Editions', '', '/app/admin/editions'),
        ]
      ),
      // end of Administration
      // ---------------------------------------------------------------------------------------------------------------------
      // TODO: the operations menu and subMenus Need Permission !important
      // start of operations
      new AppMenuItem(
        'Operations',
        'Pages',
        'digital marketing, marketing, content marketing, launch, startup, laptop.svg',
        '',
        [],
        [
          new AppMenuItem('TachyonManageService', 'Pages', '', '/app/main/tms/shippingRequests'),
          new AppMenuItem('ShipmentTracking', 'Pages', '', '/app/main/tracking'),
          new AppMenuItem(
            'SavedTemplates',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/requestsTemplates',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.TachyonDealer')
          ),
          new AppMenuItem('Requests', 'Pages', '', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem(
            'Marketplace',
            'Pages',
            '',
            '/app/main/marketplace/list',
            undefined,
            undefined,
            undefined,
            () => !this.isEnabled('App.Shipper') || !this._appSessionService.tenantId
          ),
          // new AppMenuItem(
          //     'DirectShippingRequests',
          //     'Pages',
          //     '',
          //     '/app/main/directrequest/list',
          //     undefined,
          //     undefined,
          //     undefined,
          //     undefined,
          //     () => this.isEnabled('App.SendDirectRequest') || !this._appSessionService.tenantId
          // ),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
      ),
      // end of operations
      // ---------------------------------------------------------------------------------------------------------------------
      //start of PricePackages
      new AppMenuItem(
        'PricePackages',
        '',
        'shopping, shop, ecommerce, commerce, clipboard, finance.svg',
        '',
        [],
        [new AppMenuItem('PricePackages', 'Pages.NormalPricePackages', '', '/app/main/pricePackages/normalPricePackages')],
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
        undefined,
        undefined,
        () => (this.isEnabled('App.TachyonDealer') || this.isEnabled('App.Carrier')) && this.isEnabled('App.NormalPricePackage')
      ),
      //end of PricePackages
      // ---------------------------------------------------------------------------------------------------------------------
      //start of TMSSettings
      //for host only
      new AppMenuItem(
        'TMSSettings',
        '',
        'digital marketing, marketing, content marketing, puzzle, piece, strategy.svg',
        '',
        [],
        [
          // TODO: add the Registred Trucks and Registred Drivers menu item
          new AppMenuItem(
            'Trucks',
            'Pages.Trucks',
            '',
            '/app/main/trucks/trucks',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),
          new AppMenuItem('Drivers', 'Pages.Administration.Users', '', '/app/admin/drivers', undefined, undefined, undefined, undefined, undefined),
          new AppMenuItem('TransportTypes', 'Pages.TransportTypes', '', '/app/main/transportTypes/transportTypes'),
          new AppMenuItem('TrucksTypes', 'Pages.TrucksTypes', '', '/app/main/trucksTypes/trucksTypes'),
          new AppMenuItem('CapacityCategories', 'Pages.Capacities', '', '/app/main/truckCapacities/capacities'),
          new AppMenuItem('TruckStatuses', 'Pages.Administration.TruckStatuses', '', '/app/admin/trucks/truckStatuses'),
          new AppMenuItem('PlateTypes', 'Pages.Capacities', '', '/app/main/plateTypes/plateTypes'),
          new AppMenuItem('DriverLicenseType', 'Pages.DriverLicenseTypes', '', '/app/main/driverLicenseTypes/driverLicenseTypes'),
        ],
        undefined,
        undefined
      ),
      //end of  TMSsettings
      // ---------------------------------------------------------------------------------------------------------------------
      //start of shippmentsettings
      //for host only
      new AppMenuItem(
        'ShippingSettings',
        '',
        'content marketing, digital marketing, marketing, settings, hierarchy.svg',
        '',
        [],
        [
          new AppMenuItem('ShippingTypes', 'Pages.ShippingTypes', '', '/app/main/shippingTypes/shippingTypes'),
          new AppMenuItem('Routes', 'Pages.RoutTypes', '', '/app/main/routs/routes'),
          new AppMenuItem('GoodCategories', 'Pages.GoodCategories', '', '/app/main/goodCategories/goodCategories'),
          new AppMenuItem('DangerousGoodTypes', 'Pages.DangerousGoodTypes', '', '/app/main/goods/dangerousGoodTypes'),
          new AppMenuItem('PackingTypes', 'Pages.PackingTypes', '', '/app/main/packingTypes/packingTypes'),
          new AppMenuItem('UnitOfMeasures', 'Pages.Administration.UnitOfMeasures', '', '/app/admin/unitOfMeasures/unitOfMeasures'),
          new AppMenuItem('Vas', 'Pages.Administration.Vases', '', '/app/admin/vases/vases'),
        ]
      ),
      // end of shippmentsettings
      // ---------------------------------------------------------------------------------------------------------------------
      // Start Of ADdressBook
      new AppMenuItem(
        'AddressBook',
        '',
        'map, navigation, location, navigate, book, bookmark, pin.svg',
        '',
        [],
        [
          new AppMenuItem('FacilitiesSetup', 'Pages.Facilities', '', '/app/main/addressBook/facilities'),
          //TODO: Missing permission need to give host this permission Pages.Receivers
          new AppMenuItem('ReceiversSetup', 'Pages.Facilities', '', '/app/main/receivers/receivers', undefined, undefined, undefined, undefined),
        ],
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
        undefined,
        undefined,
        () => (!this.isEnabled('App.TachyonDealer') && !this.isEnabled('App.Carrier')) || this.isEnabled('App.CarrierAsASaas')
      ),
      // end  Of ADdressBook
      // TODO: not all of these are visable to the TachyonDealer Need to Fix the Permisions in order for it to work
      // ---------------------------------------------------------------------------------------------------------------------
      // TODO: add Information Hub menu item
      // ---------------------------------------------------------------------------------------------------------------------
      //host
      //start of Settings
      new AppMenuItem(
        'Settings',
        'Pages.Administration',
        'user, interface, agent, usability, settings, options, preferences, gears.svg',
        '',
        [],
        [
          // new AppMenuItem('OrganizationUnits', 'Pages.Administration.OrganizationUnits', 'flaticon-map', '/app/admin/organization-units'),
          //  new AppMenuItem(
          //    'ShippingRequestStatuses',
          //    'Pages.Administration.ShippingRequestStatuses',
          //    'label label-danger label-dot',
          //    '/app/admin/shippingRequestStatuses/shippingRequestStatuses'
          //  ),
          //Host
          new AppMenuItem('Countries', 'Pages.Counties', '', '/app/main/countries/counties'),
          new AppMenuItem('Cities', 'Pages.Cities', '', '/app/main/cities/cities'),
          new AppMenuItem('Nationalities', 'Pages.Nationalities', '', '/app/main/nationalities/nationalities'),
          new AppMenuItem('TermAndConditions', 'Pages.TermAndConditions', '', '/app/main/termsAndConditions/termAndConditions'),
          new AppMenuItem(
            'AppLocalization',
            'Pages.AppLocalizations',
            '',
            '',
            undefined,
            [
              new AppMenuItem('Translations', 'Pages.AppLocalizations', '', '', undefined, [
                new AppMenuItem(
                  'NationalityTranslations',
                  'Pages.NationalityTranslations',
                  '',
                  '/app/main/nationalitiesTranslation/nationalityTranslations'
                ),

                new AppMenuItem('CountriesTranslations', 'Pages.CountriesTranslations', '', '/app/main/countriesTranslations/countriesTranslations'),
                new AppMenuItem(
                  'TermAndConditionTranslations',
                  'Pages.Administration.TermAndConditionTranslations',
                  '',
                  '/app/admin/termsAndConditions/termAndConditionTranslations'
                ),
                new AppMenuItem(
                  'DocumentTypeTranslations',
                  'Pages.DocumentTypeTranslations',
                  '',
                  '/app/main/documentTypeTranslations/documentTypeTranslations'
                ),
              ]),
              new AppMenuItem('Languages', 'Pages.Administration.Host.Languages', '', '/app/admin/languages', ['/app/admin/languages/{name}/texts']),
            ],
            undefined,
            undefined,
            undefined,
            undefined
          ),

          // new AppMenuItem('AuditLogs', 'Pages.Administration.AuditLogs', '', '/app/admin/auditLogs'),
          new AppMenuItem('Maintenance', 'Pages.Administration.Host.Maintenance', '', '/app/admin/maintenance'),
          new AppMenuItem('GeneralSettings', 'Pages.Administration.Host.Settings', '', '/app/admin/hostSettings'),
          // new AppMenuItem('EmailTemplates', 'Pages.EmailTemplates', '', '/app/main/emailTemplates/emailTemplates'),
        ]
      ),
      //end of Settings

      // ---------------------------------------------------------------------------------------------------------------------
      //Start Of User Manegment
      new AppMenuItem(
        'UserManagement',
        '',
        'marketing, content marketing, digital marketing, strategy, statistics, analytics, user.svg',
        '',
        [],
        [
          new AppMenuItem('Roles', 'Pages.Administration.Roles', '', '/app/admin/roles'),
          new AppMenuItem('Users', 'Pages.Administration.Users', '', '/app/admin/users'),
        ]
      ),
      //End Of User Manegment
      // ---------------------------------------------------------------------------------------------------------------------

      // //start of requests
      // //start shipper menu
      // new AppMenuItem(
      //   'Operations',
      //   'Pages.ShippingRequests',
      //   'map, navigation, location, navigate, book, bookmark, pin.svg',
      //   '/app/main/comingSoon',
      //   [],

      //   //TODO: the CreateNewRequest subMenu Need Permission and Route
      //   [
      //     new AppMenuItem(
      //       'CreateNewRequest',
      //       'Pages.ShippingRequests',
      //       '',
      //       '/app/main/shippingRequests/shippingRequestWizard',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.Shipper')
      //     ),

      //     new AppMenuItem('MyShippingRequests', 'Pages.ShippingRequests', '', '/app/main/shippingRequests/shippingRequests'),
      //     // new AppMenuItem(
      //     //   'ShipmentHistory',
      //     //   'Pages.ShippingRequests',
      //     //   '',
      //     //   '/app/main/shippingRequests/ShipmentHistory',
      //     //   undefined,
      //     //   undefined,
      //     //   undefined,
      //     //   undefined,
      //     //   () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
      //     // ),
      //     new AppMenuItem(
      //       'Marketplace',
      //       '',
      //       '',
      //       '/app/main/marketplace/list',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => !this.isEnabled('App.Shipper')
      //     ),
      //     new AppMenuItem(
      //       'SavedTemplates',
      //       'Pages.ShippingRequests',
      //       '',
      //       '/app/main/shippingRequests/requestsTemplates',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.Shipper') || this.isEnabled('App.TachyonDealer') || this.isEnabled('App.CarrierAsASaas')
      //     ),
      //     new AppMenuItem('DirectShippingRequests', '', '', '/app/main/directrequest/list', undefined, undefined, undefined, undefined, () =>
      //       this.isEnabled('App.Carrier')
      //     ),
      //     new AppMenuItem(
      //       'ShipmentTracking',
      //       'Pages',
      //       '',
      //       '/app/main/tracking',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
      //     ),
      //     // TODO this Hole Component need To be removed Later
      //     // new AppMenuItem('waybills', undefined, 'flaticon-more', '/app/admin/waybills/waybills'),
      //   ],
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Shipper')
      // ),
      // // end shipper menu
      // //start carrier menu
      // new AppMenuItem(
      //   'Operations',
      //   '',
      //   'map, navigation, location, navigate, book, bookmark, pin.svg',
      //   '/app/main/comingSoon',
      //   [],

      //   //TODO: the CreateNewRequest subMenu Need Permission and Route
      //   [
      //     new AppMenuItem(
      //       'CreateNewRequest',
      //       'Pages.ShippingRequests',
      //       '',
      //       '/app/main/shippingRequests/shippingRequestWizard',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.Shipper') || this.isEnabled('App.CarrierAsASaas')
      //     ),
      //     new AppMenuItem('MyShippingRequests', 'Pages.ShippingRequests', '', '/app/main/shippingRequests/shippingRequests'),
      //     new AppMenuItem(
      //       'Marketplace',
      //       '',
      //       '',
      //       '/app/main/marketplace/list',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => !this.isEnabled('App.Shipper')
      //     ),
      //     new AppMenuItem(
      //       'SavedTemplates',
      //       'Pages.ShippingRequests',
      //       '',
      //       '/app/main/shippingRequests/requestsTemplates',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.CarrierAsASaas')
      //     ),

      //     new AppMenuItem(
      //       'DirectShippingRequests',
      //       '',
      //       '',
      //       '/app/main/directrequest/list',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.Carrier') || this.isEnabled('App.SendDirectRequest')
      //     ),
      //     //start of shipment tracking
      //     new AppMenuItem(
      //       'ShipmentTracking',
      //       'Pages',
      //       '',
      //       '/app/main/tracking',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
      //     ),
      //     //end of shipment tracking
      //     // TODO this Hole Component need To be removed Later
      //     // new AppMenuItem('waybills', undefined, 'flaticon-more', '/app/admin/waybills/waybills'),
      //   ],
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Carrier')
      // ),

      // new AppMenuItem(
      //   'PricePackages',
      //   '',
      //   'shopping, shop, ecommerce, commerce, clipboard, finance.svg',
      //   '',
      //   [],
      //   [new AppMenuItem('PricePackages', 'Pages.NormalPricePackages', '', '/app/main/pricePackages/normalPricePackages')],
      //   //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
      //   undefined,
      //   undefined,
      //   () => (this.isEnabled('App.TachyonDealer') || this.isEnabled('App.Carrier')) && this.isEnabled('App.NormalPricePackage')
      // ),

      // end carrier menu

      //end of requests

      // new AppMenuItem(
      //   'TMS',
      //   '',
      //   'logistic, delivery, warehouse, storage, empty, vacant.svg',
      //   '',
      //   [],
      //   [
      //     new AppMenuItem('Drivers', 'Pages.Administration.Users', '', '/app/admin/drivers', undefined, undefined, undefined, undefined, undefined),
      //     new AppMenuItem(
      //       'Trucks',
      //       'Pages.Trucks',
      //       '',
      //       '/app/main/trucks/trucks',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
      //     ),
      //     new AppMenuItem('VasPrices', 'Pages.VasPrices', '', '/app/main/vases/vasPrices', undefined, undefined, undefined, undefined),
      //   ],
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
      // ),

      // new AppMenuItem(
      //   'UserManagement',
      //   '',
      //   'flaticon-user-settings',
      //   '',
      //   [],
      //   [
      //     new AppMenuItem('Roles', 'Pages.Administration.Roles', 'flaticon-suitcase', '/app/admin/roles'),
      //     new AppMenuItem('Users', 'Pages.Administration.Users', 'flaticon-users', '/app/admin/users'),
      //   ],
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Carrier')
      // ),

      // new AppMenuItem('VasPrices', 'Pages.VasPrices', 'flaticon-more', '/app/main/vases/vasPrices', undefined, undefined, undefined, undefined, () =>
      // this.isEnabled('App.Carrier')
      // ),

      // new AppMenuItem(
      //   'UserManagement',
      //   '',
      //   'flaticon-user-settings',
      //   '',
      //   [],
      //   [
      //     new AppMenuItem('Roles', 'Pages.Administration.Roles', 'flaticon-suitcase', '/app/admin/roles'),
      //     new AppMenuItem('Users', 'Pages.Administration.Users', 'flaticon-users', '/app/admin/users'),
      //   ],
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Shipper')
      // ),

      // new AppMenuItem(
      //   'Settings',
      //   'Pages.Administration.Tenant.Settings',
      //   'user, interface, agent, usability, settings, options, preferences, gears.svg',
      //   '',
      //   [],
      //   [
      //     // new AppMenuItem('TMSSettings', '', '', '', [], [], undefined, [], () => this.isEnabled('App.Carrier')),
      //     new AppMenuItem('GeneralSettings', 'Pages.Administration.Tenant.Settings', '', '/app/admin/tenantSettings'),
      //   ]
      // ),

      // new AppMenuItem(
      //   'Requests',
      //   '',
      //   'flaticon-interface-8',
      //   '',
      //   [],
      //   [
      //     new AppMenuItem(
      //       'ShippingRequests',
      //       'Pages.ShippingRequests',
      //       'label label-danger label-dot',
      //       '/app/main/shippingRequests/shippingRequests',
      //       undefined,
      //       undefined,
      //       undefined,
      //       undefined,
      //       () =>
      //         this.isEnabled('App.shippingRequest') ||
      //         this.isEnabled('App.TachyonDealer') ||
      //         this.isEnabled('App.Broker')
      //     ),
      //   ]
      // ),

      // //Host
      // new AppMenuItem(
      //   'Shipment Settings',
      //   '',
      //   'flaticon-interface-8',
      //   '',
      //   [],
      //   [
      //     new AppMenuItem('Facilities', 'Pages.Facilities', 'label label-danger label-dot', '/app/main/addressBook/facilities'),
      //     new AppMenuItem('RoutTypes', 'Pages.RoutTypes', 'label label-danger label-dot', '/app/main/routTypes/routTypes'),
      //     new AppMenuItem('Ports', 'Pages.Ports', 'label label-danger label-dot', '/app/main/ports/ports'),

      //     new AppMenuItem('RoutTypes', 'Pages.RoutTypes', 'flaticon-more', '/app/main/routTypes/routTypes'),
      //     new AppMenuItem('Ports', 'Pages.Ports', 'flaticon-more', '/app/main/ports/ports'),
      //     new AppMenuItem('Facilities', 'Pages.Facilities', 'label label-danger label-dot', '/app/main/addressBook/facilities'),
      //     new AppMenuItem('RoutTypes', 'Pages.RoutTypes', 'label label-danger label-dot', '/app/main/routTypes/routTypes'),
      //     new AppMenuItem('Ports', 'Pages.Ports', 'label label-danger label-dot', '/app/main/ports/ports'),
      //   ]
      // ),

      //carrier
      //Host
      // new AppMenuItem(
      //   'Shipping Requests',
      //   '',
      //   'flaticon-interface-8',
      //   '',
      //   [],
      //   [new AppMenuItem('Marketplace', '', 'label label-danger label-dot', '/app/main/marketPlace/marketPlace')],
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Carrier')
      // ),
      //Host

      // new AppMenuItem(
      //   'UserManagement',
      //   '',
      //   'flaticon-user-settings',
      //   '',
      //   [],
      //   [
      //     new AppMenuItem('Roles', 'Pages.Administration.Roles', 'flaticon-suitcase', '/app/admin/roles'),
      //     new AppMenuItem('Users', 'Pages.Administration.Users', 'flaticon-users', '/app/admin/users'),
      //   ]
      // ),

      // Host
      // new AppMenuItem('Vas', 'Pages.Administration.Vases', 'label label-danger label-dot', '/app/admin/vases/vases'),
      // new AppMenuItem(
      //   'VasPrices',
      //   'Pages.VasPrices',
      //   'label label-danger label-dot',
      //   '/app/main/vases/vasPrices',
      //   undefined,
      //   undefined,
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Carrier')
      // ),

      // Host

      //   this.isEnabled('App.Carrier')
      // ),

      // new AppMenuItem(
      //   'OffersMarketPlace',
      //   'Pages.Offers',
      //   'flaticon2-digital-marketing',
      //   '/app/main/offers/offers',
      //   undefined,
      //   undefined,
      //   undefined,
      //   undefined,
      //   () => this.isEnabled('App.Carrier') || this.isEnabled('App.OffersMarketPlace')
      // ),

      //new AppMenuItem('DemoUiComponents', 'Pages.DemoUiComponents', 'flaticon-shapes', '/app/admin/demo-ui-components'),
    ]);
    // console.log('menu', JSON.stringify(menu));
    return menu;
  }
}
