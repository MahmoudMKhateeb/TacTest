import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from '../app-menu';
import { AppMenuItem } from '../app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { AppBaseNavigationService } from './app-base-navigation.service';

@Injectable({ providedIn: 'root' })
export class AppShipperNavigationService extends AppBaseNavigationService {
  constructor(
    private _permissionCheckerService: PermissionCheckerService,
    private _appSessionService: AppSessionService,
    private _featureCheckerService: FeatureCheckerService
  ) {
    super(_permissionCheckerService, _appSessionService, _featureCheckerService);
  }

  getMenu(): AppMenu {
    console.log('AppShipperNavigationService');
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
            'SavedTemplates',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/requestsTemplates',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.Shipper')
          ),
          new AppMenuItem('ShipmentTracking', 'Pages', '', '/app/main/tracking', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.Shipper')
          ),
          // TODO this Hole Component need To be removed Later
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Shipper')
      ),
      // end of Operations
      //  ---------------------------------------------------------------------------------------------------------------------
      //Start Of AddressBook "Facilities Management"
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
        () => this.isEnabled('App.Shipper')
      ),
      //end  Of AddressBook  "Facilities Management"
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of Invoices
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
            () => this.isEnabled('App.Shipper')
          ),
          new AppMenuItem('FinancialTransActionMenu', 'Pages.Invoices.Transaction', '', '/app/main/invoices/transaction'),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Shipper')
      ),
      //end of  Invoices
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of Documents
      new AppMenuItem(
        'DocumentManagement',
        '',
        'interaction, interact, preferences, preformance, customer, rating, rate, questions.svg',
        '',
        [],
        [
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
        ],
        undefined,
        undefined
      ),
      //end of Documents
      // ---------------------------------------------------------------------------------------------------------------------
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
