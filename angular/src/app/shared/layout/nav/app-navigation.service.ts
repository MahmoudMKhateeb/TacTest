import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from './app-menu';
import { AppMenuItem } from './app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';

@Injectable()
export class AppNavigationService {
  constructor(
    private _permissionCheckerService: PermissionCheckerService,
    private _appSessionService: AppSessionService,
    private _featureCheckerService: FeatureCheckerService
  ) {}

  getMenu(): AppMenu {
    let menu = new AppMenu('MainMenu', 'MainMenu', [
      new AppMenuItem(
        'Dashboard',
        '',
        'interaction, interact, preferences, preformance, computer, online, rating, review.svg',
        '/app/main/dashboard'
      ),
      // ! host item with shared Sub-menu
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
          new AppMenuItem(
            'TrucksSubmittedDocuments',
            'Pages.DocumentFiles',
            '',
            '/app/main/documentFiles/TrucksSubmittedDocuments',
            [],
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
          ),
          new AppMenuItem(
            'DriversSubmittedDocuments',
            'Pages.DocumentFiles',
            '',
            '/app/main/documentFiles/DriversSubmittedDocuments',
            [],
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
          ),
          //TODO: the contracts subMenu Need Permission and Route
          new AppMenuItem(
            'contracts',
            'flaticon2-sheet',
            'flaticon-file',
            '/app/main/comingSoon',
            [],
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
          ),

          // new AppMenuItem('TenantRequiredDocuments', '', 'flaticon-settings', '/app/admin/tenantRequiredDocuments'),
        ],
        undefined,
        undefined
      ),
      //end of Documents
      //TODO: the operations menu and subMenus Need Permission !important
      //start of operations
      new AppMenuItem(
        'operations',
        'Pages',
        'digital marketing, marketing, content marketing, launch, startup, laptop.svg',
        '',
        [],
        [
          new AppMenuItem('TachyonManageService', 'Pages', '', '/app/main/tms/shippingRequests'),
          new AppMenuItem(
            'Marketplace',
            'Pages',
            '',
            '/app/main/marketplace/list',
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.MarketPlace') || !this._appSessionService.tenantId
          ),
          new AppMenuItem('Offers', 'Pages', '', '/app/main/offers'),
          new AppMenuItem('ShipmentTracking', 'Pages', '', '/app/main/tracking'),
          new AppMenuItem(
            'ShipmentHistory',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/ShipmentHistory',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.TachyonDealer')
          ),
          new AppMenuItem('Requests', 'Pages', '', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem(
            'DirectShippingRequests',
            'Pages',
            '',
            '/app/main/directrequest/list',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.SendDirectRequest') || !this._appSessionService.tenantId
          ),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
      ),
      //end of operations
      //start of requests
      //start shipper menu
      new AppMenuItem(
        'Requests',
        'Pages.ShippingRequests',
        'map, navigation, location, navigate, book, bookmark, pin.svg',
        '/app/main/comingSoon',
        [],

        //TODO: the CreateNewRequest subMenu Need Permission and Route
        [
          new AppMenuItem(
            'CreateNewRequest',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/shippingRequestWizard',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Shipper')
          ),
          new AppMenuItem('MyShippingRequests', 'Pages.ShippingRequests', '', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem(
            'ShipmentHistory',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/ShipmentHistory',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
          ),
          new AppMenuItem('Marketplace', '', '', '/app/main/marketplace/list', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.MarketPlace')
          ),
          new AppMenuItem(
            'Offers',
            '',
            '',
            '/app/main/offers',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
          ),
          new AppMenuItem('DirectShippingRequests', '', '', '/app/main/directrequest/list', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.Carrier')
          ),
          // TODO this Hole Component need To be removed Later
          // new AppMenuItem('waybills', undefined, 'flaticon-more', '/app/admin/waybills/waybills'),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Shipper')
      ),
      // end shipper menu
      //start carrier menu
      new AppMenuItem(
        'ShippingRequests',
        '',
        'map, navigation, location, navigate, book, bookmark, pin.svg',
        '/app/main/comingSoon',
        [],

        //TODO: the CreateNewRequest subMenu Need Permission and Route
        [
          new AppMenuItem(
            'CreateNewRequest',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/shippingRequestWizard',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Shipper') || this.isEnabled('App.CarrierAsASaas')
          ),
          new AppMenuItem('MyShippingRequests', 'Pages.ShippingRequests', '', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem('Marketplace', '', '', '/app/main/marketplace/list', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.MarketPlace')
          ),
          new AppMenuItem(
            'ShipmentHistory',
            'Pages',
            '',
            '/app/main/shippingRequests/ShipmentHistory',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier')
          ),

          new AppMenuItem(
            'Offers',
            '',
            '',
            '/app/main/offers',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
          ),
          new AppMenuItem(
            'DirectShippingRequests',
            '',
            '',
            '/app/main/directrequest/list',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.SendDirectRequest')
          ),
          // TODO this Hole Component need To be removed Later
          // new AppMenuItem('waybills', undefined, 'flaticon-more', '/app/admin/waybills/waybills'),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Carrier')
      ),
      // end carrier menu

      //end of requests
      //start of shipment tracking
      //TODO: shipmentTracking Carrier Menu item need Permission and Route(Component)
      new AppMenuItem(
        'shipmentTracking',
        'Pages',
        'map, navigation, location, navigate, pointer.svg',
        '/app/main/tracking',
        undefined,
        undefined,
        undefined,
        undefined,
        () => this.isEnabled('App.Carrier') || this.isEnabled('App.Shipper')
      ),
      //end of shipment tracking
      new AppMenuItem(
        'TMS',
        '',
        'logistic, delivery, warehouse, storage, empty, vacant.svg',
        '',
        [],
        [
          new AppMenuItem(
            'Drivers',
            'Pages.Administration.Users',
            'logistic, delivery, man, package, box.svg',
            '/app/admin/drivers',
            undefined,
            undefined,
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem(
            'Trucks',
            'Pages.Trucks',
            'logistic, delivery, transport, transportation, truck, free.svg',
            '/app/main/trucks/trucks',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
      ),

      //TODO: not all of these are visable to the TachyonDealer Need to Fix the Permisions in order for it to work
      //start of Invoices
      new AppMenuItem(
        'Invoices',
        'Pages.Invoices',
        'shopping, shop, ecommerce, commerce, clipboard, finance.svg',
        '',
        [],
        [
          new AppMenuItem('InvoicesList', 'Pages.Invoices', '', '/app/main/invoices/view'),
          new AppMenuItem(
            'BillingInterval',
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
          new AppMenuItem(
            'SubmitInvoiceForHost',
            //todo make it Pages.Administration.invoices
            '',
            '',
            '/app/main/invoices/submitinvoice',
            undefined,
            undefined,
            undefined,
            undefined,
            () => !this._appSessionService.tenantId
          ),

          new AppMenuItem(
            'SubmitInvoice',
            'Pages.Invoices',

            '',
            '/app/main/invoices/submitinvoice',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
          ),

          // new AppMenuItem(
          //   'InvoicesProformas',
          //   'Pages.Invoices',
          //   '',
          //   '/app/main/invoices/proformas',
          //   undefined,
          //   undefined,
          //   undefined,
          //   undefined,
          //   () => this.isEnabled('App.Shipper') || this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          // ),
          new AppMenuItem('FinancialTransActionMenu', 'Pages.Invoices.Transaction', '', '/app/main/invoices/transaction'),
        ]
        // undefined,
        // undefined,
        // () => !this.isEnabled('App.TachyonDealer')
      ),
      //end of  Invoices

      //start of reports
      //for host only
      //TODO: Need Permission
      new AppMenuItem(
        'reports',
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

      //start of TMSSettings
      //for host only
      new AppMenuItem(
        'TMSSettings',
        '',
        'digital marketing, marketing, content marketing, puzzle, piece, strategy.svg',
        '',
        [],
        [
          new AppMenuItem('TransportTypes', 'Pages.TransportTypes', '', '/app/main/transportTypes/transportTypes'),
          new AppMenuItem('TrucksTypes', 'Pages.TrucksTypes', '', '/app/main/trucksTypes/trucksTypes'),
          new AppMenuItem('CapacityCategories', 'Pages.Capacities', '', '/app/main/truckCapacities/capacities'),
          new AppMenuItem('PlateTypes', 'Pages.Capacities', '', '/app/main/plateTypes/plateTypes'),
          new AppMenuItem('TruckStatuses', 'Pages.Administration.TruckStatuses', '', '/app/admin/trucks/truckStatuses'),
          new AppMenuItem('DriverLicenseTypes', 'Pages.DriverLicenseTypes', '', '/app/main/driverLicenseTypes/driverLicenseTypes'),

          //   new AppMenuItem(
          //     'TruckTypes',
          //     '',
          //     'flaticon-truck',
          //     '',
          //     [],
          //     [
          //       new AppMenuItem(
          //         'TransportTypesTranslations',
          //         'Pages.TransportTypesTranslations',
          //         'label label-danger label-dot',
          //         '/app/main/transportTypesTranslations/transportTypesTranslations'
          //       ),
          //
          //       new AppMenuItem(
          //         'TrucksTypesTranslations',
          //         'Pages.TrucksTypesTranslations',
          //         'label label-danger label-dot',
          //         '/app/main/trucksTypesTranslations/trucksTypesTranslations'
          //       ),
          //       new AppMenuItem('TruckSubTypes', 'Pages.TruckSubtypes', 'label label-danger label-dot', '/app/main/truckSubtypes/truckSubtypes'),
          //       new AppMenuItem(
          //         'TruckCapacitiesTranslations',
          //         'Pages.TruckCapacitiesTranslations',
          //         'label label-danger label-dot',
          //         '/app/main/truckCapacitiesTranslations/truckCapacitiesTranslations'
          //       ),
          //     ]
          //   ),
          //
          //   new AppMenuItem(
          //     'TruckStatusesTranslations',
          //     'Pages.TruckStatusesTranslations',
          //     'label label-danger label-dot',
          //     '/app/main/truckStatusesTranslations/truckStatusesTranslations'
          //   ),
          //   // new AppMenuItem('PickingTypes', 'Pages.PickingTypes', 'flaticon2-telegram-logo', '/app/main/pickingTypes/pickingTypes'),
          //   // new AppMenuItem('TrailerTypes', 'Pages.TrailerTypes', 'flaticon2-delivery-truck', '/app/main/trailerTypes/trailerTypes'),
          //   // new AppMenuItem('PayloadMaxWeights', 'Pages.PayloadMaxWeights', 'flaticon2-download-1', '/app/main/payloadMaxWeight/payloadMaxWeights'),
          //   // new AppMenuItem('TrailerStatuses', 'Pages.TrailerStatuses', 'flaticon-dashboard', '/app/main/trailerStatuses/trailerStatuses'),
          //   // new AppMenuItem('GoodCategories', 'Pages.GoodCategories', 'flaticon-interface-9', '/app/main/goodCategories/goodCategories'),
          //   // new AppMenuItem(
          //   //   'UnitOfMeasures',
          //   //   'Pages.Administration.UnitOfMeasures',
          //   //   'flaticon-pie-chart-1',
          //   //   '/app/admin/unitOfMeasures/unitOfMeasures'
          //   // ),
        ],
        undefined,
        undefined
        // () => this.isEnabled('App.Host')
      ),
      //end of  TMSsettings

      //start of shippmentsettings
      //for host only
      new AppMenuItem(
        'ShippingSettings',
        '',
        'content marketing, digital marketing, marketing, settings, hierarchy.svg',
        '',
        [],
        [
          // new AppMenuItem('ShippingTypes', 'Pages.ShippingTypes', '', '/app/main/shippingTypes/shippingTypes'),
          new AppMenuItem('Routes', 'Pages.RoutTypes', '', '/app/main/routs/routes'),
          new AppMenuItem('GoodCategories', 'Pages.GoodCategories', '', '/app/main/goodCategories/goodCategories'),
          new AppMenuItem('DangerousGoodTypes', 'Pages.DangerousGoodTypes', '', '/app/main/goods/dangerousGoodTypes'),
          new AppMenuItem('PackingTypes', 'Pages.PackingTypes', '', '/app/main/packingTypes/packingTypes'),
          new AppMenuItem('UnitOfMeasures', 'Pages.Administration.UnitOfMeasures', '', '/app/admin/unitOfMeasures/unitOfMeasures'),
          new AppMenuItem('Vas', 'Pages.Administration.Vases', '', '/app/admin/vases/vases'),
        ]
      ),
      //end of shippmentsettings

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
          // new AppMenuItem('TripStatuses', 'Pages.TripStatuses', 'flaticon-more', '/app/main/tripStatuses/tripStatuses'),
          // new AppMenuItem('Vases', 'Pages.Administration.Vases', 'flaticon-more', '/app/admin/vases/vases'),

          new AppMenuItem(
            'AppLocalization',
            'Pages.AppLocalizations',
            '',
            '',
            undefined,
            [
              // new AppMenuItem('PlatformTerminologies', 'Pages.AppLocalizations', 'flaticon-clipboard', '/app/main/lanaguages/applocalizations'),
              new AppMenuItem('Translations', 'Pages.AppLocalizations', 'flaticon2-edit', '', undefined, [
                new AppMenuItem(
                  'NationalityTranslations',
                  'Pages.NationalityTranslations',
                  '',
                  '/app/main/nationalitiesTranslation/nationalityTranslations'
                ),

                new AppMenuItem('CountriesTranslations', 'Pages.CountriesTranslations', '', '/app/main/countriesTranslations/countriesTranslations'),
                // new AppMenuItem('CitiesTranslations', 'Pages.CitiesTranslations', '', '/app/main/citiesTranslations/citiesTranslations'),
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
          // new AppMenuItem(
          //   'Subscription',
          //   'Pages.Administration.Tenant.SubscriptionManagement',
          //   'flaticon-refresh',
          //   '/app/admin/subscription-management'
          // ),
          // new AppMenuItem('VisualSettings', 'Pages.Administration.UiCustomization', '', '/app/admin/ui-customization'),
          //new AppMenuItem('WebhookSubscriptions', 'Pages.Administration.WebhookSubscription', '', '/app/admin/webhook-subscriptions'),
          //  new AppMenuItem(
          //    'DynamicParameters',
          //    '',
          //    'flaticon-interface-8',
          //    '',
          //    [],
          //    [
          //      new AppMenuItem('Definitions', 'Pages.Administration.DynamicParameters', '', '/app/admin/dynamic-parameter'),
          //      new AppMenuItem('EntityDynamicParameters', 'Pages.Administration.EntityDynamicParameters', '', '/app/admin/entity-dynamic-parameter'),
          //    ]

          //  ),

          new AppMenuItem('GeneralSettings', 'Pages.Administration.Host.Settings', '', '/app/admin/hostSettings'),

          // new AppMenuItem('Settings', 'Pages.Administration.Tenant.Settings', 'flaticon-settings', '/app/admin/tenantSettings'),
        ]
      ),
      //end of Settings
      // Host
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
      //end of Administration

      //start Of user Manegment
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
      //end of user Manegment
      //host

      new AppMenuItem(
        'Settings',
        'Pages.Administration.Tenant.Settings',
        'user, interface, agent, usability, settings, options, preferences, gears.svg',
        '',
        [],
        [
          new AppMenuItem(
            'TMSSettings',
            '',
            'flaticon-cogwheel',
            '',
            [],
            [
              new AppMenuItem(
                'VasPrices',
                'Pages.VasPrices',
                'flaticon-interface-9',
                '/app/main/vases/vasPrices',
                undefined,
                undefined,
                undefined,
                undefined
              ),
            ],
            undefined,
            [],
            () => this.isEnabled('App.Carrier')
          ),
          new AppMenuItem(
            'GeneralSettings',
            'Pages.Administration.Tenant.Settings',
            'user, interface, agent, usability, settings, options, preferences, gears.svg',
            '/app/admin/tenantSettings'
          ),
        ]
      ),
      //End Of User Manegment

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

  checkChildMenuItemPermission(menuItem): boolean {
    for (let i = 0; i < menuItem.items.length; i++) {
      let subMenuItem = menuItem.items[i];

      if (subMenuItem.permissionName === '' || subMenuItem.permissionName === null) {
        if (subMenuItem.route) {
          return true;
        }
      } else if (this._permissionCheckerService.isGranted(subMenuItem.permissionName)) {
        return true;
      }

      if (subMenuItem.items && subMenuItem.items.length) {
        let isAnyChildItemActive = this.checkChildMenuItemPermission(subMenuItem);
        if (isAnyChildItemActive) {
          return true;
        }
      }
    }
    return false;
  }

  showMenuItem(menuItem: AppMenuItem): boolean {
    if (
      menuItem.permissionName === 'Pages.Administration.Tenant.SubscriptionManagement' &&
      this._appSessionService.tenant &&
      !this._appSessionService.tenant.edition
    ) {
      return false;
    }

    let hideMenuItem = false;

    if (menuItem.requiresAuthentication && !this._appSessionService.user) {
      hideMenuItem = true;
    }

    if (menuItem.permissionName && !this._permissionCheckerService.isGranted(menuItem.permissionName)) {
      hideMenuItem = true;
    }

    if (this._appSessionService.tenant || !abp.multiTenancy.ignoreFeatureCheckForHostUsers) {
      if (menuItem.hasFeatureDependency() && !menuItem.featureDependencySatisfied()) {
        hideMenuItem = true;
      }
    }

    if (!hideMenuItem && menuItem.items && menuItem.items.length) {
      return this.checkChildMenuItemPermission(menuItem);
    }

    return !hideMenuItem;
  }

  /**
   * Returns all menu items recursively
   */
  getAllMenuItems(): AppMenuItem[] {
    let menu = this.getMenu();

    let allMenuItems: AppMenuItem[] = [];
    menu.items.forEach((menuItem) => {
      allMenuItems = allMenuItems.concat(this.getAllMenuItemsRecursive(menuItem));
    });

    return allMenuItems;
  }

  private getAllMenuItemsRecursive(menuItem: AppMenuItem): AppMenuItem[] {
    if (!menuItem.items) {
      return [menuItem];
    }

    let menuItems = [menuItem];
    menuItem.items.forEach((subMenu) => {
      menuItems = menuItems.concat(this.getAllMenuItemsRecursive(subMenu));
    });

    return menuItems;
  }

  isEnabled(featureName: string) {
    return this._featureCheckerService.isEnabled(featureName);
  }
}
