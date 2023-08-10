import { Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ShipperDashboardServiceProxy,
  TrackingMapDto,
  WaybillsServiceProxy,
  ShippingRequestRouteType,
  CapacitiesServiceProxy,
  ISelectItemDto,
  TenantServiceProxy,
  FacilitiesServiceProxy,
  FacilityCityLookupTableDto,
  PagedResultDtoOfTrackingMapDto,
} from '@shared/service-proxies/service-proxies';
import { AngularFireDatabase, AngularFireList } from '@angular/fire/compat/database';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { FirebaseHelperClass, trackingIconsList } from '@app/main/shippingRequests/shippingRequests/tracking/firebaseHelper.class';
import { LazyLoadEvent } from 'primeng/api';
import { Table } from 'primeng/table';
import { Paginator } from 'primeng/paginator';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { finalize } from 'rxjs/operators';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import * as _moment from 'moment';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'app-tracking-map',
  templateUrl: './tracking-map.component.html',
  styleUrls: ['./tracking-map.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class TrackingMapComponent extends AppComponentBase implements OnInit {
  route1: any;
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  source1 = {
    lng: 21,
    lat: 22,
  };
  dest1: any;
  directions: any[];
  loading: boolean = false;
  zoom: 12;
  private fireDB: AngularFireList<unknown>;
  allDrivers = [];
  map: any;
  tripsToggle = true;
  driversToggle = true;
  waybillDownloading = false;
  trackingIconsList = trackingIconsList;
  advancedFiltersAreShown = true;
  waybillNumber: string;
  TruckType: number;
  RouteType: number;
  driverName: string;
  ShippingRequestRouteType = this.enumToArray.transform(ShippingRequestRouteType).map((item) => {
    item.value = this.l(item.value);
    return item;
  });

  allTruckTypes: ISelectItemDto[];
  cityFillter: number;
  destCityFilter: number;
  citiesList: FacilityCityLookupTableDto[];
  containerNumber: string;
  private records: PagedResultDtoOfTrackingMapDto;

  constructor(
    private injector: Injector,
    private _shipperDashboardServiceProxy: ShipperDashboardServiceProxy,
    private _waybillsServiceProxy: WaybillsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _db: AngularFireDatabase,
    private enumToArray: EnumToArrayPipe,
    private facilitiesServiceProxy: FacilitiesServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getallTruckTypes();
    this.getAllCities();
  }

  getallTruckTypes() {
    this._shipperDashboardServiceProxy.getAllTruckTypesForDropdown().subscribe((res) => {
      this.allTruckTypes = res;
      const allFacLookup = new ISelectItemDto();
      allFacLookup.displayName = this.l('All');
      (allFacLookup.id as any) = '';
      this.allTruckTypes.unshift(allFacLookup);
    });
  }

  /**
   * get all cities
   */
  getAllCities() {
    this.facilitiesServiceProxy.getAllCityForTableDropdown().subscribe((res) => {
      this.citiesList = res;
      const allFacLookup = new FacilityCityLookupTableDto();
      allFacLookup.displayName = this.l('All');
      (allFacLookup.id as any) = '';
      this.citiesList.unshift(allFacLookup);
    });
  }

  getTableRecords(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }
    this.primengTableHelper.showLoadingIndicator();

    this._shipperDashboardServiceProxy
      .getTrackingMap(
        this.TruckType,
        this.RouteType,
        this.waybillNumber,
        this.driverName,
        this.cityFillter,
        this.destCityFilter,
        this.containerNumber,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.records = result;
        this.getDriverLiveLocation();

        this.directions = [];
        for (let i = 0; i < result.items.length; i++) {
          let r = result.items[i];
          let renderOptions: google.maps.DirectionsRendererOptions = { polylineOptions: { strokeColor: '#344440' } };
          let color = this.getRandomColor();
          let direction: Direction = {
            origin: undefined,
            destination: undefined,
            waypoints: [],
            renderOptions: {
              polylineOptions: {
                strokeWeight: 6,
                strokeOpacity: 0.55,
                strokeColor: color,
              },
            },
            trackingMapDto: r,
            show: true,
            color: color,
          };
          for (let j = 0; j < r.routPoints.length; j++) {
            let x = r.routPoints[j];
            console.log('a Query Request Was Fired');
            if (r.routPoints.indexOf(x) === 0) {
              direction.origin = new google.maps.LatLng(x.latitude, x.longitude);
            } else if (r.routPoints.indexOf(x) === r.routPoints.length - 1) {
              direction.destination = new google.maps.LatLng(x.latitude, x.longitude);
            } else {
              direction.waypoints.push({
                location: new google.maps.LatLng(x.latitude, x.longitude),
              });
            }
          }
          this.directions.push(direction);
        }
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = this.directions;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }

  /**
   * get live Tracking Of Drivers For Tenant and Host
   */
  getDriverLiveLocation() {
    this.loading = true;
    let helper = new FirebaseHelperClass(this._db);
    if (this.isCarrier || this.hasShipperClients || this.hasCarrierClients) {
      helper
        .getAllActiveDriversLiveLocationByTenantId(this.appSession.tenantId)
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe((res) => {
          this.allDrivers = res;
        });
    } else if (this.isShipper) {
      combineLatest(this.records.items.map((x) => helper.getDriverLocationLiveByWayBillNumber(Number(x.wayBillNumber)))).subscribe(
        (responses) => {
          this.allDrivers = [].concat(...responses);
        },
        (error) => {
          console.error('Error in combineLatest:', error);
        }
      );
    } else if (!this.appSession.tenant.id || this.isTachyonDealer) {
      helper.getAllActiveDriversLocationsInTheSystem().subscribe((res) => {
        this.allDrivers = res;
      });
    }
  }

  mapReady(event: any) {
    this.map = event;
    this.map.controls[google.maps.ControlPosition.TOP_RIGHT].push(document.getElementById('Settings'));
  }

  /**
   * Downloads A single Drop Wayboll
   * @param id
   * @constructor
   */
  DownloadSingleDropWaybillPdf(id: number): void {
    this.waybillDownloading = true;
    this._waybillsServiceProxy.getSingleDropOrMasterWaybillPdf(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
      this.waybillDownloading = false;
    });
  }

  /**
   * show or hide drivers  from the map
   */
  driverToggle() {
    this.driversToggle = !this.driversToggle;
  }

  /**
   * show or hide trips from the map
   */
  tripToggle() {
    this.tripsToggle = !this.tripsToggle;
  }

  /**
   * check if trip Delayed or on time
   */
  isTripDelayed(tripExpectedArrivalDate): string {
    if (tripExpectedArrivalDate === '') {
      return this.l('Unknown');
    }
    const todayMoment = _moment();
    const momentExpected = _moment(tripExpectedArrivalDate);
    if (momentExpected.isSameOrAfter(todayMoment)) {
      return this.l('OnTime');
    } else if (momentExpected.isBefore(todayMoment)) {
      return this.l('Delayed');
    }
  }

  getRandomColor(): string {
    let color = '#'; // <-----------
    let letters = '0123456789ABCDEF';

    for (var i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

  getDate(date): Date {
    return !!date ? _moment(date).toDate() : null;
  }
}

export interface Direction {
  origin: google.maps.LatLng;
  destination: google.maps.LatLng;
  waypoints: google.maps.DirectionsWaypoint[];
  renderOptions?: google.maps.DirectionsRendererOptions;
  trackingMapDto: TrackingMapDto;
  show: boolean;
  color: string;
}
