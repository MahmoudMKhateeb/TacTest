import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from '../app-menu';
import { AppMenuItem } from '../app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { AppBaseNavigationService } from './app-base-navigation.service';

@Injectable({ providedIn: 'root' })
export class AppCarrierNavigationService extends AppBaseNavigationService {
  constructor(
    private _permissionCheckerService: PermissionCheckerService,
    private _appSessionService: AppSessionService,
    private _featureCheckerService: FeatureCheckerService
  ) {
    super(_permissionCheckerService, _appSessionService, _featureCheckerService);
  }

  getMenu(): AppMenu {
    console.log('AppCarrierNavigationService');
    let menu = new AppMenu('MainMenu', 'MainMenu', [
      new AppMenuItem(
        'Dashboard',
        '',
        'interaction, interact, preferences, preformance, computer, online, rating, review.svg',
        '/app/main/dashboard'
      ),
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Operations
      new AppMenuItem(
        'Operations',
        'Pages.ShippingRequests',
        'map, navigation, location, navigate, book, bookmark, pin.svg',
        '/app/main/comingSoon',
        [],
        [
          new AppMenuItem('MyShippingRequests', 'Pages.ShippingRequests', '', '/app/main/shippingRequests/shippingRequests'),
          new AppMenuItem('Marketplace', '', '', '/app/main/marketplace/list', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.Carrier')
          ),
          new AppMenuItem(
            'SavedTemplates',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/requestsTemplates',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Carrier')
          ),
          new AppMenuItem('DirectShippingRequests', '', '', '/app/main/directrequest/list', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.Carrier')
          ),
          new AppMenuItem('ShipmentTracking', 'Pages', '', '/app/main/tracking', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.Carrier')
          ),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Carrier')
      ),
      // end of Operations
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of PricePackages
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
      // end of PricePackages
      // ---------------------------------------------------------------------------------------------------------------------
      // start of TMS
      new AppMenuItem(
        'TMS',
        '',
        'logistic, delivery, warehouse, storage, empty, vacant.svg',
        '',
        [],
        [
          new AppMenuItem('Drivers', 'Pages.Administration.Users', '', '/app/admin/drivers', undefined, undefined, undefined, undefined, undefined),
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
          new AppMenuItem('VasPrices', 'Pages.VasPrices', '', '/app/main/vases/vasPrices', undefined, undefined, undefined, undefined),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Carrier') || this.isEnabled('App.TachyonDealer')
      ),
      // end of TMS
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Invoices
      new AppMenuItem(
        'Financials',
        'Pages.Invoices',
        'shopping, shop, ecommerce, commerce, clipboard, finance.svg',
        '',
        [],
        [
          new AppMenuItem('PenaltiesList', 'Pages.Invoices', '', '/app/main/penalties/view'),
          new AppMenuItem('InvoicesNoteList', 'Pages.Invoices', '', '/app/main/invoicenote/view'),
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
          new AppMenuItem('FinancialTransActionMenu', 'Pages.Invoices.Transaction', '', '/app/main/invoices/transaction'),
        ]
      ),
      // end of  Invoices
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of Documents
      new AppMenuItem(
        'DocumentManagement',
        '',
        'interaction, interact, preferences, preformance, customer, rating, rate, questions.svg',
        '',
        [],
        [
          new AppMenuItem('SubmittedDocuments', 'Pages.DocumentFiles', '', '/app/main/documentFiles/documentFiles'),
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
        ],
        undefined,
        undefined
      ),
      //end of Documents
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of Settings
      new AppMenuItem(
        'Settings',
        'Pages.Administration.Tenant.Settings',
        'user, interface, agent, usability, settings, options, preferences, gears.svg',
        '',
        [],
        [new AppMenuItem('GeneralSettings', 'Pages.Administration.Tenant.Settings', '', '/app/admin/tenantSettings')]
      ),
      //end of Settings
      //  ---------------------------------------------------------------------------------------------------------------------
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
      //  ---------------------------------------------------------------------------------------------------------------------
      // todo: add Information Hub menu item
    ]);
    return menu;
  }
}
