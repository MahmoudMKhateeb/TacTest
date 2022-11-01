import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from '../app-menu';
import { AppMenuItem } from '../app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { AppBaseNavigationService } from './app-base-navigation.service';

@Injectable({ providedIn: 'root' })
export class AppTMSNavigationService extends AppBaseNavigationService {
  constructor(
    private _permissionCheckerService: PermissionCheckerService,
    private _appSessionService: AppSessionService,
    private _featureCheckerService: FeatureCheckerService
  ) {
    super(_permissionCheckerService, _appSessionService, _featureCheckerService);
  }

  getMenu(): AppMenu {
    console.log('AppTMSNavigationService');
    let menu = new AppMenu('MainMenu', 'MainMenu', [
      new AppMenuItem(
        'Dashboard',
        '',
        'interaction, interact, preferences, preformance, computer, online, rating, review.svg',
        '/app/main/dashboard'
      ),
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Invoices
      new AppMenuItem(
        'Financials',
        'Pages.Invoices',
        'shopping, shop, ecommerce, commerce, clipboard, finance.svg',
        '',
        [],
        [
          //  todo: add Shipper Invoices and Carrier Invoices menu items
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
            'DynamicInvoice',
            'Pages.DynamicInvoices',
            '',
            '/app/main/invoices/dynamic',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.TachyonDealer') || !isNotNullOrUndefined(this._appSessionService.tenantId)
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
            undefined
          ),
        ]
      ),
      // end of  Invoices
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Documents
      new AppMenuItem(
        'DocumentManagement',
        '',
        'interaction, interact, preferences, preformance, customer, rating, rate, questions.svg',
        '',
        [],
        [
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
      // end of Documents
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Administration
      new AppMenuItem(
        'Administration',
        'Pages.Tenants',
        'content marketing, digital marketing, marketing, settings, options, key.svg',
        '',
        [],
        [
          new AppMenuItem('Tenants', 'Pages.Tenants', '', '/app/admin/tenants'),
          //  new AppMenuItem('Editions', 'Pages.Editions', '', '/app/admin/editions'),
        ]
      ),
      // end of Administration
      //  ---------------------------------------------------------------------------------------------------------------------
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
            undefined
            //  () => !this.isEnabled('App.Shipper') || !this._appSessionService.tenantId
          ),

          //  new AppMenuItem(
          //    'DirectShippingRequests',
          //    'Pages',
          //    '',
          //    '/app/main/directrequest/list',
          //    undefined,
          //    undefined,
          //    undefined,
          //    undefined,
          //    () => this.isEnabled('App.SendDirectRequest') || !this._appSessionService.tenantId
          //  ),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.TachyonDealer') || !this._appSessionService.tenantId
      ),
      // end of operations
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of PricePackages
      new AppMenuItem(
        'PricePackages',
        '',
        'shopping, shop, ecommerce, commerce, clipboard, finance.svg',
        '',
        [],
        [
          new AppMenuItem('PricePackages', 'Pages.NormalPricePackages', '', '/app/main/pricePackages/normalPricePackages'),
          new AppMenuItem('TMS Price Packages', 'Pages.TmsPricePackages', '', '/app/main/pricePackages/tmsPricePackages'),
          new AppMenuItem('Price Packages Proposal', 'Pages.TmsPricePackages', '', '/app/main/pricePackages/pricePackagesProposal'),
          new AppMenuItem('Price Package Appendices', 'Pages.PricePackageAppendix', '', '/app/main/pricePackages/pricePackageAppendices'),
        ],
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
        undefined,
        undefined,
        () => (this.isEnabled('App.TachyonDealer') || this.isEnabled('App.Carrier')) && this.isEnabled('App.NormalPricePackage')
      ),
      // end of PricePackages
      // ---------------------------------------------------------------------------------------------------------------------
      // start of TMSSettings
      // for host only
      new AppMenuItem(
        'TMSSettings',
        '',
        'digital marketing, marketing, content marketing, puzzle, piece, strategy.svg',
        '',
        [],
        [
          new AppMenuItem('Drivers', 'Pages.Administration.Users', '', '/app/admin/drivers', undefined, undefined, undefined, undefined, undefined),
          new AppMenuItem('Trucks', 'Pages.Trucks', '', '/app/main/trucks/trucks', undefined, undefined, undefined, undefined),
        ],
        undefined,
        undefined
        //  () => this.isEnabled('App.Host')
      ),
      // end of  TMSsettings
      //  ---------------------------------------------------------------------------------------------------------------------
      // start Of AddressBook "Facilities Management"
      new AppMenuItem(
        'AddressBook',
        '',
        'map, navigation, location, navigate, book, bookmark, pin.svg',
        '',
        [],
        [
          new AppMenuItem('FacilitiesSetup', 'Pages.Facilities', '', '/app/main/addressBook/facilities'),
          new AppMenuItem('ReceiversSetup', 'Pages.Facilities', '', '/app/main/receivers/receivers', undefined, undefined, undefined, undefined),
        ],
        undefined,
        undefined
      ),
      // end Of AddressBook "Facilities Management"
      //  ---------------------------------------------------------------------------------------------------------------------
      //  todo: add Information Hub menu item
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Settings
      new AppMenuItem(
        'Settings',
        'Pages.Administration.Tenant.Settings',
        'user, interface, agent, usability, settings, options, preferences, gears.svg',
        '',
        [],
        [new AppMenuItem('GeneralSettings', 'Pages.Administration.Tenant.Settings', '', '/app/admin/tenantSettings')]
      ),
      // end of Settings
      //  ---------------------------------------------------------------------------------------------------------------------
      // Start Of User Manegment

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

      // End Of User Manegment
      //  ---------------------------------------------------------------------------------------------------------------------
    ]);
    return menu;
  }
}
