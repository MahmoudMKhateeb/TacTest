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
      new AppMenuItem('Dashboard', 'Pages.Administration.Host.Dashboard', 'flaticon-line-graph', '/app/admin/hostDashboard'),
      new AppMenuItem('Dashboard', 'Pages.Tenant.Dashboard', 'flaticon-line-graph', '/app/main/dashboard'),
      // //Shipper
      new AppMenuItem(
        'Requests',
        '',
        'flaticon-interface-8',
        '',
        [],
        [
          new AppMenuItem(
            'ShippingRequests',
            'Pages.ShippingRequests',
            'flaticon-more',
            '/app/main/shippingRequests/shippingRequests',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this._featureCheckerService.isEnabled('App.Shipper')
          ),
        ],
        undefined,
        undefined,
        () => this._featureCheckerService.isEnabled('App.Shipper')
      ),

      // //Host
      // new AppMenuItem(
      //   'Shipment Settings',
      //   '',
      //   'flaticon-interface-8',
      //   '',
      //   [],
      //   [
      //     new AppMenuItem('Facilities', 'Pages.Facilities', 'flaticon-more', '/app/main/addressBook/facilities'),
      //     new AppMenuItem('RoutTypes', 'Pages.RoutTypes', 'flaticon-more', '/app/main/routTypes/routTypes'),
      //     new AppMenuItem('Counties', 'Pages.Counties', 'flaticon-more', '/app/main/countries/counties'),
      //     new AppMenuItem('Cities', 'Pages.Cities', 'flaticon-more', '/app/main/cities/cities'),
      //     new AppMenuItem('Ports', 'Pages.Ports', 'flaticon-more', '/app/main/ports/ports'),
      //   ]
      // ),

      //carrier
      new AppMenuItem(
        'Shipping Requests',
        '',
        'flaticon-interface-8',
        '',
        [],
        [new AppMenuItem('Marketplace', '', 'flaticon-more', '/app/main/marketPlace/marketPlace')],
        undefined,
        undefined,
        () => this._featureCheckerService.isEnabled('App.Carrier')
      ),
      //carrier
      new AppMenuItem('ShippingTypes', 'Pages.ShippingTypes', 'flaticon-more', '/app/main/shippingTypes/shippingTypes'),
      new AppMenuItem('PackingTypes', 'Pages.PackingTypes', 'flaticon-more', '/app/main/packingTypes/packingTypes'),
      new AppMenuItem('TripStatuses', 'Pages.TripStatuses', 'flaticon-more', '/app/main/tripStatuses/tripStatuses'),

      new AppMenuItem(
        'Documents',
        '',
        'flaticon-book',
        '',
        [],
        [
          new AppMenuItem('DocumentManagement', 'Pages.DocumentTypes', 'flaticon2-document', '/app/main/documentTypes/documentTypes'),
          new AppMenuItem('DocumentsEntities', 'Pages.DocumentsEntities', 'flaticon-doc', '/app/main/documentsEntities/documentsEntities'),
          new AppMenuItem('SubmittedDocuments', 'Pages.DocumentFiles', 'flaticon-file', '/app/main/documentFiles/documentFiles'),
          new AppMenuItem(
            'DocumentTypeTranslations',
            'Pages.DocumentTypeTranslations',
            'flaticon-file-1',
            '/app/main/documentTypeTranslations/documentTypeTranslations'
          ),
          // new AppMenuItem('TenantRequiredDocuments', '', 'flaticon-settings', '/app/admin/tenantRequiredDocuments'),
        ]
      ),

      //Host
      new AppMenuItem(
        'TMSSettings',
        '',
        'flaticon-cogwheel',
        '',
        [],
        [
          new AppMenuItem(
            'TruckTypes',
            '',
            'flaticon-truck',
            '',
            [],
            [
              new AppMenuItem('TransportTypes', 'Pages.TransportTypes', 'flaticon-more', '/app/main/transportTypes/transportTypes'),
              new AppMenuItem(
                'TransportTypesTranslations',
                'Pages.TransportTypesTranslations',
                'flaticon-more',
                '/app/main/transportTypesTranslations/transportTypesTranslations'
              ),
              new AppMenuItem('TransportSubTypes', 'Pages.TransportSubtypes', 'flaticon-more', '/app/main/transportSubtypes/transportSubtypes'),
              new AppMenuItem('TrucksTypes', 'Pages.TrucksTypes', 'flaticon-truck', '/app/main/trucksTypes/trucksTypes'),
              new AppMenuItem('TruckSubTypes', 'Pages.TruckSubtypes', 'flaticon-more', '/app/main/truckSubtypes/truckSubtypes'),
              new AppMenuItem('CapacityCategories', 'Pages.Capacities', 'flaticon-more', '/app/main/truckCapacities/capacities'),
            ]
          ),
          new AppMenuItem('TruckStatuses', 'Pages.Administration.TruckStatuses', 'flaticon-info', '/app/admin/trucks/truckStatuses'),
          // new AppMenuItem('PickingTypes', 'Pages.PickingTypes', 'flaticon2-telegram-logo', '/app/main/pickingTypes/pickingTypes'),
          // new AppMenuItem('TrailerTypes', 'Pages.TrailerTypes', 'flaticon2-delivery-truck', '/app/main/trailerTypes/trailerTypes'),
          // new AppMenuItem('PayloadMaxWeights', 'Pages.PayloadMaxWeights', 'flaticon2-download-1', '/app/main/payloadMaxWeight/payloadMaxWeights'),
          // new AppMenuItem('TrailerStatuses', 'Pages.TrailerStatuses', 'flaticon-dashboard', '/app/main/trailerStatuses/trailerStatuses'),
          new AppMenuItem('GoodCategories', 'Pages.GoodCategories', 'flaticon-interface-9', '/app/main/goodCategories/goodCategories'),
          // new AppMenuItem(
          //   'UnitOfMeasures',
          //   'Pages.Administration.UnitOfMeasures',
          //   'flaticon-pie-chart-1',
          //   '/app/admin/unitOfMeasures/unitOfMeasures'
          // ),
        ],
        undefined,
        undefined
        // () => this._featureCheckerService.isEnabled('App.Host')
      ),

      // Carrier
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

      // Host
      new AppMenuItem(
        'Administration',
        '',
        'flaticon-interface-8',
        '',
        [],
        [
          new AppMenuItem('Tenants', 'Pages.Tenants', 'flaticon-list-3', '/app/admin/tenants'),
          new AppMenuItem('Editions', 'Pages.Editions', 'flaticon-app', '/app/admin/editions'),
        ]
      ),

      // Host
      new AppMenuItem('Vases', 'Pages.Administration.Vases', 'flaticon-more', '/app/admin/vases/vases'),
      new AppMenuItem('VasPrices', 'Pages.VasPrices', 'flaticon-more', '/app/main/vases/vasPrices', undefined, undefined, undefined, undefined, () =>
        this._featureCheckerService.isEnabled('App.Carrier')
      ),
      // Host
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

      // new AppMenuItem('Routes', 'Pages.Routes', 'flaticon-map-location', '/app/main/routs/routes', undefined, undefined, undefined, undefined, () =>
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
      new AppMenuItem('Nationalities', 'Pages.Nationalities', 'flaticon-more', '/app/main/nationalities/nationalities'),

      new AppMenuItem(
        'NationalityTranslations',
        'Pages.NationalityTranslations',
        'flaticon-more',
        '/app/main/nationalitiesTranslation/nationalityTranslations'
      ),

      new AppMenuItem(
        'Settings',
        'Pages.Administration',
        'flaticon-interface-8',
        '',
        [],
        [
          // new AppMenuItem('OrganizationUnits', 'Pages.Administration.OrganizationUnits', 'flaticon-map', '/app/admin/organization-units'),
          //  new AppMenuItem(
          //    'ShippingRequestStatuses',
          //    'Pages.Administration.ShippingRequestStatuses',
          //    'flaticon-more',
          //    '/app/admin/shippingRequestStatuses/shippingRequestStatuses'
          //  ),

          new AppMenuItem('Languages', 'Pages.Administration.Host.Languages', 'flaticon-tabs', '/app/admin/languages', [
            '/app/admin/languages/{name}/texts',
          ]),
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
          new AppMenuItem('Settings', 'Pages.Administration.Host.Settings', 'flaticon-settings', '/app/admin/hostSettings'),
          new AppMenuItem('Settings', 'Pages.Administration.Tenant.Settings', 'flaticon-settings', '/app/admin/tenantSettings'),
          new AppMenuItem('TermAndConditions', 'Pages.TermAndConditions', 'flaticon-more', '/app/main/termsAndConditions/termAndConditions'),
          new AppMenuItem(
            'TermAndConditionTranslations',
            'Pages.Administration.TermAndConditionTranslations',
            'flaticon-more',
            '/app/admin/termsAndConditions/termAndConditionTranslations'
          ),
        ]
      ),

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
