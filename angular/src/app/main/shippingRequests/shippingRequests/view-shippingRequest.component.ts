import { ChangeDetectorRef, Component, Inject, Injector, OnInit, ViewChild } from '@angular/core';
import {
  GetShippingRequestForViewOutput,
  ShippingRequestBidsServiceProxy,
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  CancelBidShippingRequestInput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, NavigationEnd, Router, RouterEvent } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { Paginator } from 'primeng/paginator';
import { Table } from '@node_modules/primeng/table';
import { filter } from '@node_modules/rxjs/internal/operators';
import { DOCUMENT } from '@angular/common';
import { PricingOfferComponent } from '@app/main/shippingRequests/shippingRequests/tachyonDeal/pricingOffer/pricingOffer.component';
import { GetAllDirectRequestsTableComponent } from '@app/main/shippingRequests/shippingRequests/directShippingRequest/getAllDirectRequestsTable.component';
import { SendDirectRequestModalComponent } from '@app/main/shippingRequests/shippingRequests/directShippingRequest/sendDirectRequestsModal/sendDirectRequestModal.component';
@Component({
  templateUrl: './view-shippingRequest.component.html',
  styleUrls: ['./view-shippingRequest.component.scss'],
  animations: [appModuleAnimation()],
})
export class ViewShippingRequestComponent extends AppComponentBase implements OnInit {
  @ViewChild('pricingOffer', { static: false }) pricingOffer: PricingOfferComponent;
  @ViewChild('GetAllDirectRequestsTable', { static: false }) GetAllDirectRequestsTable: GetAllDirectRequestsTableComponent;
  @ViewChild('dataTableChild', { static: false })
  dataTable: Table;
  @ViewChild('paginatorChild', { static: false }) paginator: Paginator;
  active = false;
  saving = false;
  loading = true;
  CancelBidShippingRequest: CancelBidShippingRequestInput = new CancelBidShippingRequestInput();
  shippingRequestforView: GetShippingRequestForViewOutput;
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
    private _shippingRequestBidsServiceProxy: ShippingRequestBidsServiceProxy,
    private changeDetectorRef: ChangeDetectorRef,
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
      this.reloadPage();
    });
    // this.GetAllDirectRequestsTable.sendDirectRequestsModal.shippingRequestId = this._activatedRoute.snapshot.queryParams['id'];
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy.getShippingRequestForView(shippingRequestId).subscribe((result) => {
      this.shippingRequestforView = result;
      this.activeShippingRequestId = this.shippingRequestforView.shippingRequest.id;
      this.active = true;
      this.loading = false;
    });
  }

  reloadCurrentPage() {
    this._document.defaultView.location.reload();
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  /**
   * this function validates who Can See And Access the Bidding List in ViewShippingRequest
   */
  canSeeShippingRequestBids() {
    if (
      this.feature.isEnabled('App.Shipper') &&
      !this.shippingRequestforView.shippingRequest.isTachyonDeal &&
      (this.shippingRequestforView.shippingRequest.status === 0 || this.shippingRequestforView.shippingRequest.status === 2)
    ) {
      return true;
    } else if (
      this.feature.isEnabled('App.TachyonDealer') &&
      this.shippingRequestforView.shippingRequest.isBid &&
      (this.shippingRequestforView.shippingRequest.status === 0 || this.shippingRequestforView.shippingRequest.status === 2)
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
      return `${this.l('TachyonManageService')},${this.l('Marketplace')} `;
    } else if (this.shippingRequestforView.shippingRequest.isTachyonDeal) {
      return this.l('TachyonManageService');
    } else if (this.shippingRequestforView.shippingRequest.isBid) {
      return this.l('Marketplace');
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
    //if there is an active shipping Request id and the user is TachyonDealer and there still no Carrier assigned to this shipping Reqeust
    if (
      this.activeShippingRequestId &&
      this.feature.isEnabled('App.SendDirectRequest') &&
      ((!this.shippingRequestforView.shippingRequest.isTachyonDeal && !this.shippingRequestforView.shippingRequest.isBid) ||
        (this.feature.isEnabled('App.TachyonDealer') && this.shippingRequestforView.shippingRequest.isTachyonDeal)) &&
      !this.shippingRequestforView.shippingRequest.carrierTenantId
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
      this.GetAllDirectRequestsTable.sendDirectRequestsModal.show();
    }, 1000);
  }
}
