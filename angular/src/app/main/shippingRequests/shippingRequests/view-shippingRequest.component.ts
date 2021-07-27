import { AfterViewChecked, ChangeDetectorRef, Component, Inject, Injector, OnInit, ViewChild } from '@angular/core';
import {
  GetShippingRequestForViewOutput,
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  GetShippingRequestVasForViewDto,
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

@Component({
  templateUrl: './view-shippingRequest.component.html',
  styleUrls: ['./view-shippingRequest.component.scss'],
  animations: [appModuleAnimation()],
})
export class ViewShippingRequestComponent extends AppComponentBase implements OnInit, AfterViewChecked {
  active = false;
  saving = false;
  loading = true;
  shippingRequestforView: GetShippingRequestForViewOutput;
  vases: GetShippingRequestVasForViewDto[];
  activeShippingRequestId: number;
  bidsloading = false;

  breadcrumbs: BreadcrumbItem[] = [
    new BreadcrumbItem(this.l('ShippingRequests'), '/app/main/shippingRequests/shippingRequests'),
    new BreadcrumbItem('' + this._activatedRoute.snapshot.queryParams['id']),
  ];

  constructor(
    injector: Injector,
    private _activatedRoute: ActivatedRoute,
    private _router: Router,
    private _shippingRequestsServiceProxy: ShippingRequestsServiceProxy,
    private changeDetectorRef: ChangeDetectorRef,
    private _trip: TripService,
    @Inject(DOCUMENT) private _document: Document
  ) {
    super(injector);
    this.shippingRequestforView = new GetShippingRequestForViewOutput();
    this.shippingRequestforView.shippingRequest = new ShippingRequestDto();
    this.activeShippingRequestId = this._activatedRoute.snapshot.queryParams['id'];
  }

  ngOnInit(): void {
    this.show(this._activatedRoute.snapshot.queryParams['id']);
    this._router.events.pipe(filter((event: RouterEvent) => event instanceof NavigationEnd)).subscribe(() => {
      this.show(this._activatedRoute.snapshot.queryParams['id']);
    });
    // this.GetAllDirectRequestsTable.sendDirectRequestsModal.shippingRequestId = this._activatedRoute.snapshot.queryParams['id'];
  }

  ngAfterViewChecked() {
    //update the Trips Shared Service With RouteType
    //routeType id is not reterned from backend
    this._trip.updateShippingRequest(this.shippingRequestforView);
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy.getShippingRequestForView(shippingRequestId).subscribe((result) => {
      this.shippingRequestforView = result;
      this.vases = result.shippingRequestVasDtoList;
      this.activeShippingRequestId = this.shippingRequestforView.shippingRequest.id;
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
      this.shippingRequestforView.shippingRequest.requestType === ShippingRequestType.Marketplace &&
      (this.shippingRequestforView.shippingRequest.status === ShippingRequestStatus.PrePrice ||
        this.shippingRequestforView.shippingRequest.status === ShippingRequestStatus.NeedsAction)
    ) {
      return true;
    } else if (
      this.feature.isEnabled('App.TachyonDealer') &&
      this.shippingRequestforView.shippingRequest.isBid &&
      (this.shippingRequestforView.shippingRequest.status === ShippingRequestStatus.PrePrice ||
        this.shippingRequestforView.shippingRequest.status === ShippingRequestStatus.NeedsAction)
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
      this.shippingRequestforView.shippingRequest.isBid &&
      this.shippingRequestforView.shippingRequest.isTachyonDeal
    ) {
      return `${this.l('TachyonManageService')},${this.l('TachyonManageService')} `;
    } else if (this.shippingRequestforView.shippingRequest.isTachyonDeal) {
      return this.l('TachyonManageService');
    } else if (this.shippingRequestforView.shippingRequest.isBid) {
      return this.l('Marketplace');
    } else if (this.shippingRequestforView.shippingRequest.isDirectRequest) {
      return this.l('DirectRequest');
    }
  }

  canSeeShippingRequestTrips() {
    //if there is no carrierTenantId  and the current user in not a carrier Hide Trips Section
    if (this.feature.isEnabled('App.Carrier') && !this.shippingRequestforView.shippingRequest.carrierTenantId) {
      return false;
    } else if (this.feature.isEnabled('App.TachyonDealer')) {
      //if Tachyon Dealer
      return true;
    }
    //By Default
    return true;
  }

  /**
   * this function validates who Can See And Access the DirectRequests List in ViewShippingRequest
   */
  canSeeDirectRequests() {
    if (!this.feature.isEnabled('App.SendDirectRequest')) return false;
    if (this.feature.isEnabled('App.TachyonDealer') && this.shippingRequestforView.shippingRequest.isTachyonDeal) {
      return true;
    }
    if (this.feature.isEnabled('App.Shipper') && !this.shippingRequestforView.shippingRequest.isBid) return true;

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
      // this.GetAllDirectRequestsTable.sendDirectRequestsModal.show();
    }, 1000);
  }
}
