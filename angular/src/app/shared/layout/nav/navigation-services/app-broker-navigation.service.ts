import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from '../app-menu';
import { AppMenuItem } from '../app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';
import { AppBaseNavigationService } from './app-base-navigation.service';

@Injectable({ providedIn: 'root' })
export class AppBrokerNavigationService extends AppBaseNavigationService {
  constructor(
    private _permissionCheckerService: PermissionCheckerService,
    private _appSessionService: AppSessionService,
    private _featureCheckerService: FeatureCheckerService
  ) {
    super(_permissionCheckerService, _appSessionService, _featureCheckerService);
  }

  getMenu(): AppMenu {
    let menu = new AppMenu('MainMenu', 'MainMenu', [
      new AppMenuItem('Dashboard', 'Pages.Tenant.Dashboard', 'Dashboards.svg', '/app/main/dashboard'),
      // start of reporting
      // ---------------------------------------------------------------------------------------------------------------------

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
            'Shipments',
            '',
            '',
            '/app/main/directShipments',
            [],
            //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
            undefined,
            undefined,
            () => this.isEnabled('App.TachyonDealer')
          ),
          new AppMenuItem(
            'ShippingRequests',
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
          new AppMenuItem('Tracking', 'Pages.shipment.Tracking', '', '/app/main/tracking/shipmentTracking', undefined, undefined, undefined, {
            showType: 2,
          }),
          new AppMenuItem(
            'Templates',
            'Pages.EntityTemplate',
            '',
            '/app/main/shippingRequests/requestsTemplates',
            undefined,
            undefined,
            undefined,
            undefined,
            () => this.isEnabled('App.SaveTemplateFeature')
          ),
        ],
        undefined,
        undefined
      ),
      // end of Operations
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of TMS
      new AppMenuItem(
        'FleetManagement',
        '',
        '8 TMS Managment.svg',
        '',
        [],
        [
          new AppMenuItem('Trucks', 'Pages.Trucks', '', '/app/main/trucks/trucks', undefined, undefined, undefined, undefined),
          new AppMenuItem('Drivers', 'Pages.Administration.Drivers', '', '/app/admin/drivers', undefined, undefined, undefined, undefined, undefined),
          // new AppMenuItem('VasPrices', 'Pages.VasPrices', '', '/app/main/vases/vasPrices', undefined, undefined, undefined, undefined),
          new AppMenuItem(
            'DriversCommission',
            'Pages.Administration.Drivers',
            '',
            '/app/admin/driversCommission',
            undefined,
            undefined,
            undefined,
            undefined,
            undefined
          ),
        ],
        undefined,
        undefined
      ),
      // end of TMS
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of PricePackages
      new AppMenuItem(
        'PricePackages',
        '',
        '7 Price Packging.svg',
        '',
        [],
        [new AppMenuItem('Price Packages', 'Pages.PricePackages', '', '/app/main/saaSpricePackages/saaSpricePackages')]
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
      ),
      // end of PricePackages
      // ---------------------------------------------------------------------------------------------------------------------

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
          new AppMenuItem('ReceiversSetup', 'Pages.Receivers', '', '/app/main/receivers/receivers', undefined, undefined, undefined, undefined),
        ],
        //added these line because the tachyon dealer has the above permision and he suppose not to see this menu
        undefined,
        undefined
      ),
      //end  Of AddressBook  "Facilities Management"
      //  ---------------------------------------------------------------------------------------------------------------------
      // start of Invoices
      new AppMenuItem(
        'Financials',
        'Pages.Invoices',
        '2 Financials.svg',
        '',
        [],
        [
          new AppMenuItem('PenaltiesList', 'Pages.Penalties', '', '/app/main/penalties/view'),
          new AppMenuItem('InvoicesNoteList', 'Pages.Invoices.InvoiceNote', '', '/app/main/invoicenote/view'),
          new AppMenuItem(
            'SubmitInvoice',
            'Pages.Invoices.SubmitInvoices',
            '',
            '/app/main/invoices/submitinvoice',
            undefined,
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem('InvoicesList', 'Pages.Invoices.View', '', '/app/main/invoices/view', undefined, undefined, undefined, undefined),
          new AppMenuItem('FinancialTransActionMenu', 'Pages.Invoices.Transaction', '', '/app/main/invoices/transaction'),
          new AppMenuItem(
            'ActorInvoicesList',
            'Pages.Administration.ActorsInvoice',
            '',
            '/app/main/actors/invoices',
            undefined,
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem(
            'ActorCarrierInvoicesList',
            'Pages.Administration.SubmitActorsInvoice',
            '',
            '/app/main/actors/carrierInvoices',
            undefined,
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem(
            'ClientsDedicatedInvoices',
            'Pages.Invoices.View',
            '',
            '/app/main/invoices/dedicatedClients',
            undefined,
            undefined,
            undefined,
            undefined
          ),
        ]
      ),
      // end of  Invoices
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of TMS for shipper
      // new AppMenuItem(
      //   'TMSForShipper',
      //   'Pages.ShippingRequests.TmsForShipper',
      //   '3 TMS.svg',
      //   '/app/main/tmsforshipper',
      //   [],
      //   undefined,
      //   undefined,
      //   undefined
      // ),
      //end of TMS for shipper
      //  ---------------------------------------------------------------------------------------------------------------------
      //start of Documents
      new AppMenuItem(
        'DocumentManagement',
        '',
        '4 Documents managment.svg',
        '',
        [],
        [
          new AppMenuItem('SubmittedDocuments', 'Pages.DocumentFiles.Submitted', '', '/app/main/documentFiles/documentFiles'),
          new AppMenuItem(
            'NonMandatoryDocuments',
            'Pages.DocumentFiles.Additional',
            '',
            '/tenantNotRequiredDocuments/tenantNotRequiredDocuments',
            [],
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem(
            'TrucksSubmittedDocuments',
            'Pages.DocumentFiles.Trucks',
            '',
            '/app/main/documentFiles/TrucksSubmittedDocuments',
            [],
            undefined,
            undefined,
            undefined
          ),
          new AppMenuItem(
            'DriversSubmittedDocuments',
            'Pages.DocumentFiles.Drivers',
            '',
            '/app/main/documentFiles/DriversSubmittedDocuments',
            [],
            undefined,
            undefined,
            undefined
          ),
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

      new AppMenuItem(
        'TMSSettings',
        '',
        '8 TMS Managment.svg',
        '',
        [],
        [
          new AppMenuItem('TransportTypes', 'Pages.TransportTypes', '', '/app/main/transportTypes/transportTypes'),
          new AppMenuItem('TrucksTypes', 'Pages.TrucksTypes', '', '/app/main/trucksTypes/trucksTypes'),
          new AppMenuItem('CapacityCategories', 'Pages.Capacities', '', '/app/main/truckCapacities/capacities'),
          new AppMenuItem('PlateTypes', 'Pages.Capacities', '', '/app/main/plateTypes/plateTypes'),
          new AppMenuItem('TruckStatuses', 'Pages.Administration.TruckStatuses', '', '/app/admin/trucks/truckStatuses'),
          new AppMenuItem('DriverLicenseType', 'Pages.DriverLicenseTypes', '', '/app/main/driverLicenseTypes/driverLicenseTypes'),
        ],
        undefined,
        undefined
      ),
      //  ---------------------------------------------------------------------------------------------------------------------
      new AppMenuItem('ClientsAndTransporters', 'Pages.Administration.Actors', 'User Management.svg', '/app/main/actors/actors', [], []),

      //start of Settings
      new AppMenuItem(
        'Settings',
        'Pages.Administration.Tenant.Settings',
        '10 Setting.svg',
        '',
        [],
        [
          new AppMenuItem('GeneralSettings', 'Pages.Administration.Tenant.Settings', '', '/app/admin/tenantSettings'),
          new AppMenuItem('OrganizationUnits', 'Pages.Administration.OrganizationUnits', '', '/app/admin/organization-units'),
        ]
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
