import { AfterViewChecked, ChangeDetectorRef, Component, Inject, Injector, OnInit, ViewChild } from '@angular/core';
import {
  GetShippingRequestForViewOutput,
  GetShippingRequestVasForViewDto,
  SavedEntityType,
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  ShippingRequestStatus,
  ShippingRequestType,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, NavigationEnd, Router, RouterEvent } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { filter } from '@node_modules/rxjs/internal/operators';
import { DOCUMENT } from '@angular/common';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { DirectRequestComponent } from '@app/main/shippingRequests/shippingRequests/directrequest/direct-request.component';
import { retry } from 'rxjs/operators';
import { NotesComponent } from './notes/notes.component';
import * as moment from '@node_modules/moment';

@Component({
  templateUrl: './view-shippingRequest.component.html',
  styleUrls: ['./view-shippingRequest.component.scss'],
  animations: [appModuleAnimation()],
})
export class ViewShippingRequestComponent extends AppComponentBase implements OnInit, AfterViewChecked {
  @ViewChild('directRequestComponent') public directRequestComponent: DirectRequestComponent;
  @ViewChild('NotesComponent') public NotesComponent: NotesComponent;
  active = false;
  saving = false;
  loading = true;
  //shippingRequestforView: GetShippingRequestForViewOutput;
  vases: GetShippingRequestVasForViewDto[];
  activeShippingRequestId: number;
  bidsloading = false;
  entityTypes = SavedEntityType;
  type = 'ShippingRequest';
  breadcrumbs: BreadcrumbItem[] = [new BreadcrumbItem(this.l('ShippingRequests'), '/app/main/shippingRequests/shippingRequests')];
  rentalRange: { rentalStartDate: moment.Moment; rentalEndDate: moment.Moment } = null;

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private changeDetectorRef: ChangeDetectorRef,
    public _trip: TripService,
    @Inject(DOCUMENT) private _document: Document
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._trip.GetShippingRequestForViewOutput = new GetShippingRequestForViewOutput();
    this._trip.GetShippingRequestForViewOutput.shippingRequest = new ShippingRequestDto();
    this._trip.GetShippingRequestForViewOutput.shippingRequest.init();
    this.activeShippingRequestId = this._activatedRoute.snapshot.queryParams['id'];
    this.show(this._activatedRoute.snapshot.queryParams['id']);
    this._router.events.pipe(filter((event: RouterEvent) => event instanceof NavigationEnd)).subscribe(() => {
      this.show(this._activatedRoute.snapshot.queryParams['id']);
    });
    // this.GetAllDirectRequestsTable.sendDirectRequestsModal.shippingRequestId = this._activatedRoute.snapshot.queryParams['id'];
    // this._ShippingRequestDDService.ngOnInit();
  }

  ngAfterViewChecked() {
    // update the Trips Shared Service With RouteType
    // routeType id is not reterned from backend
    // this._trip.updateShippingRequest(this.shippingRequestforView);
    // this.changeDetectorRef.detectChanges();
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy
      .getShippingRequestForView(shippingRequestId)
      .pipe(retry(3))
      .subscribe((result) => {
        this._trip.GetShippingRequestForViewOutput = result;
        this._trip.GetShippingRequestForViewOutput.rentalStartDate = moment(this._trip.GetShippingRequestForViewOutput?.rentalStartDate);
        this._trip.GetShippingRequestForViewOutput.rentalEndDate = moment(this._trip.GetShippingRequestForViewOutput?.rentalEndDate);
        this.rentalRange = {
          rentalStartDate: this._trip.GetShippingRequestForViewOutput?.rentalStartDate,
          rentalEndDate: this._trip.GetShippingRequestForViewOutput?.rentalEndDate,
        };
        this.vases = result.shippingRequestVasDtoList;
        this.breadcrumbs.push(new BreadcrumbItem('' + result.referenceNumber));
        this.activeShippingRequestId = this._trip.GetShippingRequestForViewOutput.shippingRequest.id;
        this.active = true;
        this.loading = false;
      });
  }

  reloadCurrentPage() {
    this._document.defaultView.location.reload();
  }

  /**
   * this function validates who Can See And Access the Bidding List in ViewShippingRequest
   */
  canSeeShippingRequestBids() {
    if (
      this.feature.isEnabled('App.Shipper') &&
      this._trip.GetShippingRequestForViewOutput.shippingRequest.requestType === ShippingRequestType.Marketplace &&
      (this._trip.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.PrePrice ||
        this._trip.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.NeedsAction)
    ) {
      return true;
    } else if (
      this.feature.isEnabled('App.TachyonDealer') &&
      this._trip.GetShippingRequestForViewOutput.shippingRequest.isBid &&
      (this._trip.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.PrePrice ||
        this._trip.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.NeedsAction)
    ) {
      return true;
    } else {
      return false;
    }
  }

  /**
   * Binds the Value if Shipping Request Type in View Shipping Request Base on User Feature
   */
  shipppingRequestType() {
    if (
      this.feature.isEnabled('App.TachyonDealer') &&
      this._trip.GetShippingRequestForViewOutput.shippingRequest.isBid &&
      this._trip.GetShippingRequestForViewOutput.shippingRequest.isTachyonDeal
    ) {
      return `${this.l('TachyonManageService')},${this.l('TachyonManageService')} `;
    } else if (this._trip.GetShippingRequestForViewOutput.shippingRequest.isTachyonDeal) {
      return this.l('TachyonManageService');
    } else if (this._trip.GetShippingRequestForViewOutput.shippingRequest.isBid) {
      return this.l('Marketplace');
    } else if (this._trip.GetShippingRequestForViewOutput.shippingRequest.isDirectRequest) {
      return this.l('DirectRequest');
    }
  }

  canSeeShippingRequestTrips() {
    //if there is no carrierTenantId  and the current user in not a carrier Hide Trips Section
    if (this.feature.isEnabled('App.Carrier') && !this._trip.GetShippingRequestForViewOutput.shippingRequest.carrierTenantId) {
      console.log('false');
      return false;
    } else if (this.feature.isEnabled('App.TachyonDealer')) {
      //if Tachyon Dealer

      return true;
    }
    //By Default
    // console.log('true');
    return true;
  }

  /**
   * this function validates who Can See And Access the DirectRequests List in ViewShippingRequest
   */
  canSeeDirectRequests() {
    if (!this.feature.isEnabled('App.SendDirectRequest')) {
      return false;
    }
    if (this.feature.isEnabled('App.TachyonDealer') && this._trip.GetShippingRequestForViewOutput.shippingRequest.isTachyonDeal) {
      return true;
    }
    if (
      this.feature.isEnabled('App.Shipper') &&
      this.feature.isEnabled('App.SendDirectRequest') &&
      this._trip.GetShippingRequestForViewOutput.shippingRequest.isDirectRequest
    ) {
      return true;
    }

    return false;
  }

  /**
   * this function validates who Can See Price Offers
   */
  canSeePriceOffers() {
    // if the user is carrier
    if (this.feature.isEnabled('App.Carrier')) {
      return false;
    }
    return true;
  }
  canSeePricePackages() {
    let isNotMarketPlaceRequest = this._trip.GetShippingRequestForViewOutput.shippingRequest.requestType !== ShippingRequestType.Marketplace;
    let isRequestStatusPrePrice = this._trip.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.PrePrice;
    let isRequestStatusNeedsAction = this._trip.GetShippingRequestForViewOutput.shippingRequest.status === ShippingRequestStatus.NeedsAction;
    let isRequestTypeDirectRequest = this._trip.GetShippingRequestForViewOutput.shippingRequest.requestType === ShippingRequestType.DirectRequest;
    let isRequestTypeTachyonManageService =
      this._trip.GetShippingRequestForViewOutput.shippingRequest.requestType === ShippingRequestType.TachyonManageService;

    if (
      this._trip.GetShippingRequestForViewOutput.shippingRequestFlag === 0 &&
      (this.isCarrier || (this.isTachyonDealer && isRequestTypeTachyonManageService)) &&
      isNotMarketPlaceRequest &&
      (isRequestStatusPrePrice || isRequestStatusNeedsAction)
    ) {
      return true;
    }
    return false;
  }
  /* canSeePriceOffers() {
      // if the user is carrier
      if (this.feature.isEnabled('App.Carrier') || this.shippingRequestforView.shippingRequest.price) {
        return false;
      }
      return true;
    }
    */
  /**
   * this function scrolls to a the direct Requests table and opens up the Send Direct Requests Modal for Tachyon Dealer
   */
  scrollToDirectRequests() {
    let el = document.getElementById('directRequests');
    el.scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'center' });
    setTimeout(() => {
      this.directRequestComponent.DirectRequestTenantModel.show();
    }, 1000);
  }

  updateShippingRequestInvoiceFlag() {
    this._shippingRequestsServiceProxy
      .updateShippingRequestInvoiceFlag(
        this._trip.GetShippingRequestForViewOutput.shippingRequest.id,
        this._trip.GetShippingRequestForViewOutput.shippingRequest.splitInvoiceFlag
      )
      .subscribe(() => {
        this.notify.success(this.l('success'));
      });
  }
}
