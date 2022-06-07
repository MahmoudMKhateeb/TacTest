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
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

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
  ShippingRequestRouteType = this.enumToArray.transform(ShippingRequestRouteType);

  allTruckTypes: ISelectItemDto[];
  cityFillter: number;
  destCityFilter: number;
  citiesList: FacilityCityLookupTableDto[];
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
    this.getDriverLiveLocation();
    this.getallTruckTypes();
    this.getAllCities();
  }

  getallTruckTypes() {
    this._shipperDashboardServiceProxy.getAllTruckTypesForDropdown().subscribe((res) => {
      this.allTruckTypes = res;
    });
  }

  /**
   * get all cities
   */
  getAllCities() {
    this.facilitiesServiceProxy.getAllCityForTableDropdown().subscribe((res) => {
      this.citiesList = res;
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
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.directions = [];
        result.items.forEach((r) => {
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

          r.routPoints.forEach((x) => {
            setTimeout(() => {
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
            }, 1000);
          });

          this.directions.push(direction);
        });

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
    if (this.isCarrier) {
      helper
        .getAllActiveDriversLiveLocationByTenantId(this.appSession.tenantId)
        .pipe(
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe((res) => {
          this.allDrivers = res;
          console.log('allDrivers', this.allDrivers);
        });
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
  isTripDelayed(tripExpectedArrivalDate): 'Delayed' | 'OnTime' | 'Unknown' {
    if (tripExpectedArrivalDate == '') return 'Unknown';
    let todayMoment = this.dateFormatterService.GetTodayGregorian();
    if (tripExpectedArrivalDate < todayMoment) {
      return 'OnTime';
    } else {
      return 'Delayed';
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
