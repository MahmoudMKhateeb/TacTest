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
    return new AppMenu('MainMenu', 'MainMenu', [
      new AppMenuItem('Dashboard', '', 'flaticon-line-graph', '/app/main/dashboard'),
      //host item with shared Sub-menu
      new AppMenuItem(
        'DocumentManagement',
        '',
        'flaticon2-document',
        '',
        [],
        [
          new AppMenuItem('DocumentManagement', 'Pages.DocumentTypes', 'flaticon2-medical-records-1', '/app/main/documentTypes/documentTypes'),
          new AppMenuItem(
            'DocumentsEntities',
            'Pages.DocumentsEntities',
            'flaticon2-medical-records-1',
            '/app/main/documentsEntities/documentsEntities'
          ),
          //TODO: the contracts subMenu Need Permission and Route
          new AppMenuItem(
            'NonMandatoryDocuments',
            'Pages.DocumentFiles',
            'flaticon2-crisp-icons',
            '/app/main/comingSoon',
            [],
            undefined,
            undefined,
            undefined,
            () => !this._featureCheckerService.isEnabled('App.Shipper') && !this._featureCheckerService.isEnabled('App.Carrier')
          ),
          new AppMenuItem('SubmittedDocuments', 'Pages.DocumentFiles', 'flaticon-file', '/app/main/documentFiles/documentFiles'),
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
            () => this._featureCheckerService.isEnabled('App.Carrier') || this._featureCheckerService.isEnabled('App.Shipper')
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
        'flaticon-interface-8',
        '',
        [],
        [
          new AppMenuItem('TachyonManagedServices', 'Pages.ShippingRequests', 'flaticon-delete-2', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem('Marketplace', 'Pages', 'flaticon2-shopping-cart-1', '/app/main/marketplace/list'),
          new AppMenuItem('Offers', 'Pages', 'label label-danger label-dot', '/app/main/offers'),
          new AppMenuItem('ShipmentTracking', 'Pages', 'flaticon-interface-9', '/app/main/tracking'),
          new AppMenuItem('Requests', 'Pages', 'label label-danger label-dot', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem(
            'TachyonManageService',
            'Pages.ShippingRequests',
            'label label-danger label-dot',
            '/app/main/shippingRequests/shippingRequests'
          ),
          new AppMenuItem('Marketplace', 'Pages', 'label label-danger label-dot', '/app/main/marketPlace/marketPlace'),
          new AppMenuItem(
            'DirectShippingRequests',
            'Pages',
            'flaticon2-send-1',
            '/app/main/directrequest/list',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.SendDirectRequest') || !this._appSessionService.tenantId
          ),
        ],
        undefined,
        undefined,
        () => this._featureCheckerService.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
      ),
      //end of operations
      //start of requests
      new AppMenuItem(
        'Requests',
        'Pages.ShippingRequests',
        'flaticon-interface-8',
        '/app/main/comingSoon',
        [],

        //TODO: the CreateNewRequest subMenu Need Permission and Route
        [
          new AppMenuItem(
            'CreateNewRequest',
            'Pages.ShippingRequests',
            'flaticon2-add',
            '/app/main/shippingRequests/shippingRequests/createOrEdit',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.Shipper')
          ),
          new AppMenuItem('ShippingRequests', 'Pages.ShippingRequests', 'flaticon2-cube', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem(
            'Marketplace',
            '',
            'flaticon2-shopping-cart-1',
            '/app/main/marketplace/list',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.Carrier') || this._featureCheckerService.isEnabled('App.Shipper')
          ),
          new AppMenuItem(
            'Offers',
            '',
            'flaticon-more',
            '/app/main/offers',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.Carrier') || this._featureCheckerService.isEnabled('App.Shipper')
          ),
          new AppMenuItem(
            'DirectShippingRequests',
            '',
            'flaticon2-rocket-1',
            '/app/main/directrequest/list',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.Carrier') || this._featureCheckerService.isEnabled('App.SendDirectRequest')
          ),
          // TODO this Hole Component need To be removed Later
          // new AppMenuItem('waybills', undefined, 'flaticon-more', '/app/admin/waybills/waybills'),
        ],
        undefined,
        undefined,
        () => this._featureCheckerService.isEnabled('App.Carrier') || this._featureCheckerService.isEnabled('App.Shipper')
      ),

      //end of requests
      //start of shipment tracking
      //TODO: shipmentTracking Carrier Menu item need Permission and Route(Component)
      new AppMenuItem(
        'shipmentTracking',
        'Pages',
        'flaticon-map-location',
        '/app/main/tracking',
        undefined,
        undefined,
        undefined,
        undefined,
        () => this._featureCheckerService.isEnabled('App.Carrier') || this._featureCheckerService.isEnabled('App.Shipper')
      ),
      //end of shipment tracking
      new AppMenuItem(
        'TMS',
        '',
        'flaticon-cogwheel',
        '',
        [],
        [
          new AppMenuItem(
            'Drivers',
            'Pages.Administration.Users',
            'flaticon-users',
            '/app/admin/drivers',
            undefined,
            undefined,
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem('Trucks', 'Pages.Trucks', 'flaticon-truck', '/app/main/trucks/trucks', undefined, undefined, undefined, undefined, () =>
            this._featureCheckerService.isEnabled('App.Carrier')
          ),
        ],
        undefined,
        undefined,
        () => this._featureCheckerService.isEnabled('App.Carrier')
      ),

      //TODO: not all of these are visable to the TachyonDealer Need to Fix the Permisions in order for it to work
      //start of Invoices
      new AppMenuItem(
        'Invoices',
        'Pages.Invoices',
        'fas fa-file-invoice-dollar',
        '',
        [],
        [
          new AppMenuItem('InvoicesList', 'Pages.Invoices', 'flaticon2-document', '/app/main/invoices/view'),
          new AppMenuItem(
            'BillingInterval',
            'Pages.Invoices',
            'flaticon2-list-2',
            '/app/main/invoices/periods',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),
          new AppMenuItem(
            'PaymentMethods',
            'Pages.Invoices',
            'flaticon-coins',
            '/app/main/invoices/paymentlist',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),
          new AppMenuItem(
            'BalnaceRecharges',
            'Pages.Invoices',
            'fas fa-dollar-sign',
            '/app/main/invoices/balnacerecharges',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
          ),
          new AppMenuItem(
            'SubmitInvoice',
            'Pages.Invoices',

            'flaticon-tool',
            '/app/main/invoices/submitinvoice',
            undefined,
            undefined,
            undefined,
            undefined,
            () =>
              this._featureCheckerService.isEnabled('App.Carrier') ||
              this._featureCheckerService.isEnabled('App.TachyonDealer') ||
              !this._appSessionService.tenantId
          ),
          new AppMenuItem(
            'InvoicesProformas',
            'Pages.Invoices',
            'flaticon2-document',
            '/app/main/invoices/proformas',
            undefined,
            undefined,
            undefined,
            undefined,
            () =>
              this._featureCheckerService.isEnabled('App.Shipper') ||
              this._featureCheckerService.isEnabled('App.TachyonDealer') ||
              !this._appSessionService.tenantId
          ),
          new AppMenuItem('FinancialTransActionMenu', 'Pages.Invoices.Transaction', 'flaticon-arrows', '/app/main/invoices/transaction'),
        ]
        // undefined,
        // undefined,
        // () => !this._featureCheckerService.isEnabled('App.TachyonDealer')
      ),
      //end of  Invoices

      //start of reports
      //for host only
      //TODO: Need Permission
      new AppMenuItem(
        'reports',
        'Pages.Administration.Host.Dashboard',

        'flaticon2-files-and-folders',
        '',
        [],
        [
          new AppMenuItem('AccidentReason', 'Pages.ShippingRequestResoneAccidents', 'flaticon-exclamation', '/app/main/accidents/reasons'),
          new AppMenuItem('TripRejectReason', 'Pages.ShippingRequestTrips.Reject.Reason', 'flaticon-info', '/app/main/trip/reject/reasons'),
        ]
        // undefined,
        // undefined,
        // () => !this._featureCheckerService.isEnabled('App.TachyonDealer')
      ),
      //end of report

      //start of TMSSettings
      //for host only
      new AppMenuItem(
        'TMSSettings',
        '',
        'flaticon-cogwheel',
        '',
        [],
        [
          new AppMenuItem('TransportTypes', 'Pages.TransportTypes', 'flaticon2-delivery-truck', '/app/main/transportTypes/transportTypes'),
          new AppMenuItem('TrucksTypes', 'Pages.TrucksTypes', 'flaticon2-lorry', '/app/main/trucksTypes/trucksTypes'),
          new AppMenuItem('CapacityCategories', 'Pages.Capacities', 'flaticon2-open-box', '/app/main/truckCapacities/capacities'),
          new AppMenuItem('PlateTypes', 'Pages.Capacities', 'flaticon2-browser', '/app/main/plateTypes/plateTypes'),
          new AppMenuItem('TruckStatuses', 'Pages.Administration.TruckStatuses', 'flaticon2-analytics-1', '/app/admin/trucks/truckStatuses'),

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
        // () => this._featureCheckerService.isEnabled('App.Host')
      ),
      //end of  TMSsettings

      //start of shippmentsettings
      //for host only
      new AppMenuItem(
        'ShippingSettings',
        '',
        'flaticon2-delivery-truck',
        '',
        [],
        [
          new AppMenuItem('ShippingTypes', 'Pages.ShippingTypes', 'flaticon2-lorry', '/app/main/shippingTypes/shippingTypes'),
          new AppMenuItem('Routes', 'Pages.RoutTypes', 'flaticon-map-location', '/app/main/routs/routes'),
          new AppMenuItem('GoodCategories', 'Pages.GoodCategories', 'flaticon-bag', '/app/main/goodCategories/goodCategories'),
          new AppMenuItem('PackingTypes', 'Pages.PackingTypes', 'flaticon-layers', '/app/main/packingTypes/packingTypes'),
          new AppMenuItem(
            'UnitOfMeasures',
            'Pages.Administration.UnitOfMeasures',
            'flaticon-pie-chart-1',
            '/app/admin/unitOfMeasures/unitOfMeasures'
          ),
          new AppMenuItem('Vas', 'Pages.Administration.Vases', 'flaticon-app', '/app/admin/vases/vases'),
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
      //   () => this._featureCheckerService.isEnabled('App.Carrier')
      // ),

      // new AppMenuItem('VasPrices', 'Pages.VasPrices', 'flaticon-more', '/app/main/vases/vasPrices', undefined, undefined, undefined, undefined, () =>
      // this._featureCheckerService.isEnabled('App.Carrier')
      // ),

      new AppMenuItem(
        'AddressBook',
        '',
        'flaticon-map-location',
        '',
        [],
        [
          new AppMenuItem('FacilitiesSetup', 'Pages.Facilities', 'flaticon-map-location', '/app/main/addressBook/facilities'),
          //TODO: Missing permission need to give host this permission Pages.Receivers
          new AppMenuItem(
            'ReceiversSetup',
            'Pages.Facilities',
            'flaticon-users',
            '/app/main/receivers/receivers',
            undefined,
            undefined,
            undefined,
            undefined
          ),
        ],
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
        undefined,
        undefined,
        () => !this._featureCheckerService.isEnabled('App.TachyonDealer') && !this._featureCheckerService.isEnabled('App.Carrier')
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
      //   () => this._featureCheckerService.isEnabled('App.Shipper')
      // ),

      //host
      //start of Settings
      new AppMenuItem(
        'Settings',
        'Pages.Administration',
        'flaticon-settings-1',
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
          new AppMenuItem('Countries', 'Pages.Counties', 'flaticon-map-location', '/app/main/countries/counties'),
          new AppMenuItem('Cities', 'Pages.Cities', 'flaticon-map-location', '/app/main/cities/cities'),
          new AppMenuItem('Nationalities', 'Pages.Nationalities', 'flaticon-notes', '/app/main/nationalities/nationalities'),

          new AppMenuItem('TermAndConditions', 'Pages.TermAndConditions', 'flaticon-interface-10', '/app/main/termsAndConditions/termAndConditions'),
          // new AppMenuItem('TripStatuses', 'Pages.TripStatuses', 'flaticon-more', '/app/main/tripStatuses/tripStatuses'),
          // new AppMenuItem('Vases', 'Pages.Administration.Vases', 'flaticon-more', '/app/admin/vases/vases'),

          new AppMenuItem(
            'AppLocalization',
            'Pages.AppLocalizations',
            'flaticon2-edit',
            '',
            undefined,
            [
              new AppMenuItem('PlatformTerminologies', 'Pages.AppLocalizations', 'flaticon-clipboard', '/app/main/lanaguages/applocalizations'),
              new AppMenuItem('Translations', 'Pages.AppLocalizations', 'flaticon2-edit', '', undefined, [
                new AppMenuItem(
                  'NationalityTranslations',
                  'Pages.NationalityTranslations',
                  'flaticon-clipboard',
                  '/app/main/nationalitiesTranslation/nationalityTranslations'
                ),

                new AppMenuItem(
                  'CountriesTranslations',
                  'Pages.CountriesTranslations',
                  'flaticon2-list-2',
                  '/app/main/countriesTranslations/countriesTranslations'
                ),
                new AppMenuItem(
                  'CitiesTranslations',
                  'Pages.CitiesTranslations',
                  'flaticon2-list-2',
                  '/app/main/citiesTranslations/citiesTranslations'
                ),
                new AppMenuItem(
                  'TermAndConditionTranslations',
                  'Pages.Administration.TermAndConditionTranslations',
                  'flaticon2-list-2',
                  '/app/admin/termsAndConditions/termAndConditionTranslations'
                ),
                new AppMenuItem(
                  'DocumentTypeTranslations',
                  'Pages.DocumentTypeTranslations',
                  'flaticon-file-1',
                  '/app/main/documentTypeTranslations/documentTypeTranslations'
                ),
              ]),
              new AppMenuItem('Languages', 'Pages.Administration.Host.Languages', 'flaticon-tabs', '/app/admin/languages', [
                '/app/admin/languages/{name}/texts',
              ]),
            ],
            undefined,
            undefined,
            undefined,
            undefined
          ),

          // new AppMenuItem('AuditLogs', 'Pages.Administration.AuditLogs', 'flaticon-folder-1', '/app/admin/auditLogs'),
          // new AppMenuItem('Maintenance', 'Pages.Administration.Host.Maintenance', 'flaticon-lock', '/app/admin/maintenance'),
          // new AppMenuItem(
          //   'Subscription',
          //   'Pages.Administration.Tenant.SubscriptionManagement',
          //   'flaticon-refresh',
          //   '/app/admin/subscription-management'
          // ),
          // new AppMenuItem('VisualSettings', 'Pages.Administration.UiCustomization', 'flaticon-medical', '/app/admin/ui-customization'),
          // new AppMenuItem('WebhookSubscriptions', 'Pages.Administration.WebhookSubscription', 'flaticon2-world', '/app/admin/webhook-subscriptions'),
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

          new AppMenuItem('GeneralSettings', 'Pages.Administration.Host.Settings', 'flaticon-settings', '/app/admin/hostSettings'),

          // new AppMenuItem('Settings', 'Pages.Administration.Tenant.Settings', 'flaticon-settings', '/app/admin/tenantSettings'),
        ]
      ),
      //end of Settings
      // Host
      //start of Administration
      new AppMenuItem(
        'Administration',
        '',
        'flaticon-map',
        '',
        [],
        [
          new AppMenuItem('Tenants', 'Pages.Tenants', 'flaticon-list-3', '/app/admin/tenants'),
          new AppMenuItem('Editions', 'Pages.Editions', 'flaticon-app', '/app/admin/editions'),
        ]
      ),
      //end of Administration

      //start Of user Manegment
      new AppMenuItem(
        'UserManagement',
        '',
        'flaticon-user-settings',
        '',
        [],
        [
          new AppMenuItem('Roles', 'Pages.Administration.Roles', 'flaticon-suitcase', '/app/admin/roles'),
          new AppMenuItem('Users', 'Pages.Administration.Users', 'flaticon-users', '/app/admin/users'),
        ]
      ),
      //end of user Manegment
      //host

      new AppMenuItem(
        'Settings',
        'Pages.Administration.Tenant.Settings',
        'flaticon-truck',
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
            () => this._featureCheckerService.isEnabled('App.Carrier')
          ),
          new AppMenuItem('GeneralSettings', 'Pages.Administration.Tenant.Settings', 'flaticon-settings', '/app/admin/tenantSettings'),
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
      //         this._featureCheckerService.isEnabled('App.shippingRequest') ||
      //         this._featureCheckerService.isEnabled('App.TachyonDealer') ||
      //         this._featureCheckerService.isEnabled('App.Broker')
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
      //   () => this._featureCheckerService.isEnabled('App.Carrier')
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
      //   () => this._featureCheckerService.isEnabled('App.Carrier')
      // ),

      // Host

      //   this._featureCheckerService.isEnabled('App.Carrier')
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
      //   () => this._featureCheckerService.isEnabled('App.Carrier') || this._featureCheckerService.isEnabled('App.OffersMarketPlace')
      // ),

      //new AppMenuItem('DemoUiComponents', 'Pages.DemoUiComponents', 'flaticon-shapes', '/app/admin/demo-ui-components'),
    ]);
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
}
