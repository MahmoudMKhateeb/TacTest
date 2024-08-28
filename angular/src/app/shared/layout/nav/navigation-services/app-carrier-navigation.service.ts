import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from '../app-menu';
import { AppMenuItem } from '../app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';
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
      new AppMenuItem('Dashboard', '', 'Dashboards.svg', '/app/main/dashboard'),
      // start of reporting
      // ---------------------------------------------------------------------------------------------------------------------
      //TODO: Need Permission
      new AppMenuItem(
        'Reporting',
        'Pages.Reports',
        'report.svg',
        '',
        [],
        [
          new AppMenuItem('CreateReport', 'Pages.Reports.Create', '', '/app/main/reporting/generate-report'),
          new AppMenuItem('MyReports', '', '', '/app/main/reporting/all-reports'),
        ]
        // undefined,
        // undefined,
        // () => !this.isEnabled('App.TachyonDealer')
      ),
      //end of reporting
      // ---------------------------------------------------------------------------------------------------------------------
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Operations
      new AppMenuItem(
        'Operations',
        'Pages.ShippingRequests',
        '6 Opertation.svg',
        '/app/main/comingSoon',
        [],
        [
          new AppMenuItem(
            'MyShippingRequests',
            'Pages.ShippingRequests',
            '',
            '/app/main/shippingRequests/shippingRequests',
            undefined,
            undefined,
            undefined,
            {
              showType: 1,
            }
          ),
          // new AppMenuItem('Marketplace', '', '', '/app/main/marketplace/list', undefined, undefined, undefined, undefined, () =>
          //   this.isEnabled('App.Carrier')
          // ),
          new AppMenuItem(
            'SavedTemplates',
            'Pages.EntityTemplate',
            '',
            '/app/main/shippingRequests/requestsTemplates',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.CarrierAsASaas') && this.isEnabled('App.SaveTemplateFeature')
          ),
          new AppMenuItem('DirectShippingRequests', '', '', '/app/main/directrequest/list', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.Carrier')
          ),
          new AppMenuItem(
            'ShipmentTracking',
            'Pages',
            '',
            '/app/main/tracking/shipmentTracking',
            undefined,
            undefined,
            undefined,
            {
              showType: 2,
            },
            () => this.isEnabled('App.Carrier')
          ),
        ],
        undefined,
        undefined,
        () => this.isEnabled('App.Carrier')
      ),
      // end of Operations
      //  ---------------------------------------------------------------------------------------------------------------------
      //Start Of AddressBook "Facilities Management"
      new AppMenuItem(
        'AddressBook',
        '',
        '9 Facilities managment.svg',
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
        () => this.isEnabled('App.ShipperClients') || this.isEnabled('App.CarrierAsASaas')
      ),
      //end  Of AddressBook  "Facilities Management"

      new AppMenuItem(
        'directShipments',
        '',
        'logistic, delivery, truck, vehicle, transportation, transport, speed, express.svg',
        '/app/main/directShipments',
        [],
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
        undefined,
        undefined,
        undefined,
        () => this.isEnabled('App.CarrierAsASaas')
      ),

      // start of PricePackages
      new AppMenuItem(
        'PricePackages',
        '',
        '7 Price Packging.svg',
        '',
        [],
        [
          new AppMenuItem('Price Packages', 'Pages.PricePackages', '', '/app/main/pricePackages/pricePackages'),
          new AppMenuItem('Price Package Appendices', 'Pages.PricePackageAppendix', '', '/app/main/pricePackages/pricePackageAppendices'),
        ]
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
      ),
      // end of PricePackages
      // ---------------------------------------------------------------------------------------------------------------------
      // start of TMS
      new AppMenuItem(
        'TMS',
        '',
        '8 TMS Managment.svg',
        '',
        [],
        [
          new AppMenuItem('Drivers', 'Pages.Administration.Drivers', '', '/app/admin/drivers', undefined, undefined, undefined, undefined, undefined),
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
        '2 Financials.svg',
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
          new AppMenuItem(
            'InvoicesList',
            'Pages.Invoices',

            '',
            '/app/main/invoices/view',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.CarrierAsASaas')
          ),
          new AppMenuItem('FinancialTransActionMenu', 'Pages.Invoices.Transaction', '', '/app/main/invoices/transaction'),
          new AppMenuItem(
            'ActorInvoicesList',
            'Pages.Administration.ActorsInvoice',
            '',
            '/app/main/actors/invoices',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.ShipperClients')
          ),
          new AppMenuItem(
            'ActorCarrierInvoicesList',
            'Pages.Administration.ActorsInvoice',
            '',
            '/app/main/actors/invoices',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.CarrierClients')
          ),
          new AppMenuItem('ClientsDedicatedInvoices', '', '', '/app/main/invoices/dedicatedClients', undefined, undefined, undefined, undefined, () =>
            this.isEnabled('App.ShipperClients')
          ),
        ]
      ),
      // end of  Invoices
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of Documents
      new AppMenuItem(
        'DocumentManagement',
        '',
        '4 Documents managment.svg',
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
          new AppMenuItem(
            'ActorsSubmittedDocuments',
            'Pages.DocumentFiles',
            '',
            '/app/main/documentFiles/ActorsSubmittedDocuments',
            [],
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.ShipperClients')
          ),
        ],
        undefined,
        undefined
      ),
      /////Start of Actors
      new AppMenuItem('Actors', 'Pages.Administration.Actors', 'User Management.svg', '/app/main/actors/actors', [], [], undefined, undefined, () =>
        this.isEnabled('App.ShipperClients')
      ),
      //end of actors
      //end of Documents
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of Settings
      new AppMenuItem(
        'Settings',
        'Pages.Administration.Tenant.Settings',
        '10 Setting.svg',
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
        '11 User managment.svg',
        '',
        [],
        [
          new AppMenuItem('Roles', 'Pages.Administration.Roles', '', '/app/admin/roles'),
          new AppMenuItem('Users', 'Pages.Administration.Users.View', '', '/app/admin/users'),
        ]
      ),

      //End Of User Manegment
      //  ---------------------------------------------------------------------------------------------------------------------
      // todo: add Information Hub menu item
    ]);
    return menu;
  }
}
