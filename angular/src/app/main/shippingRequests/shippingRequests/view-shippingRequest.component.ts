import { AfterViewChecked, ChangeDetectorRef, Component, Inject, Injector, OnInit, ViewChild } from '@angular/core';
import {
  GetShippingRequestForViewOutput,
  ShippingRequestDto,
  ShippingRequestsServiceProxy,
  GetShippingRequestVasForViewDto,
  ShippingRequestStatus,
  ShippingRequestType,
  SavedEntityType,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, NavigationEnd, Router, RouterEvent } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
import { filter } from '@node_modules/rxjs/internal/operators';
import { DOCUMENT } from '@angular/common';
import { TripService } from '@app/main/shippingRequests/shippingRequests/ShippingRequestTrips/trip.service';
import { DirectRequestComponent } from '@app/main/shippingRequests/shippingRequests/directrequest/direct-request.component';
import { finalize, retry } from 'rxjs/operators';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { NotesComponent } from './notes/notes.component';

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
  shippingRequestforView: GetShippingRequestForViewOutput;
  vases: GetShippingRequestVasForViewDto[];
  activeShippingRequestId: number;
  bidsloading = false;
  entityTypes = SavedEntityType;
  type = 'ShippingRequest';
  breadcrumbs: BreadcrumbItem[] = [new BreadcrumbItem(this.l('ShippingRequests'), '/app/main/shippingRequests/shippingRequests')];

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
    // this._ShippingRequestDDService.ngOnInit();
  }

  ngAfterViewChecked() {
    //update the Trips Shared Service With RouteType
    //routeType id is not reterned from backend
    this._trip.updateShippingRequest(this.shippingRequestforView);
  }

  show(shippingRequestId: number): void {
    this._shippingRequestsServiceProxy
      .getShippingRequestForView(shippingRequestId)
      .pipe(retry(3))
      .subscribe((result) => {
        this.shippingRequestforView = result;
        this.vases = result.shippingRequestVasDtoList;
        this.breadcrumbs.push(new BreadcrumbItem('' + result.referenceNumber));
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
      console.log('false');
      return false;
    } else if (this.feature.isEnabled('App.TachyonDealer')) {
      //if Tachyon Dealer
      console.log('true');

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
    if (this.feature.isEnabled('App.TachyonDealer') && this.shippingRequestforView.shippingRequest.isTachyonDeal) {
      return true;
    }
    if (
      this.feature.isEnabled('App.Shipper') &&
      this.feature.isEnabled('App.SendDirectRequest') &&
      this.shippingRequestforView.shippingRequest.isDirectRequest
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
    let isNotMarketPlaceRequest = this.shippingRequestforView.shippingRequest.requestType !== ShippingRequestType.Marketplace;
    let isRequestStatusPrePrice = this.shippingRequestforView.shippingRequest.status === ShippingRequestStatus.PrePrice;
    let isRequestStatusNeedsAction = this.shippingRequestforView.shippingRequest.status === ShippingRequestStatus.NeedsAction;
    let isRequestTypeDirectRequest = this.shippingRequestforView.shippingRequest.requestType === ShippingRequestType.DirectRequest;
    let isRequestTypeTachyonManageService = this.shippingRequestforView.shippingRequest.requestType === ShippingRequestType.TachyonManageService;
    // if the user is carrier
    if (
      !this.isCarrier &&
      isNotMarketPlaceRequest &&
      (isRequestStatusPrePrice ||
        (isRequestStatusNeedsAction && isRequestTypeDirectRequest) ||
        (this.isTachyonDealer && isRequestTypeTachyonManageService))
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
}
