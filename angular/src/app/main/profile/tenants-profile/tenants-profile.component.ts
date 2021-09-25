import { AfterViewInit, Component, Injector, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { AppConsts } from '@shared/AppConsts';
import { InvoicingInformationDto, PagedResultDtoOfFacilityLocationListDto, ProfileServiceProxy } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { result } from 'lodash-es';
import { Table } from '@node_modules/primeng/table';
import { Paginator } from '@node_modules/primeng/paginator';
import { LazyLoadEvent } from 'primeng/api';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  selector: 'tenants-profile',
  templateUrl: './tenants-profile.component.html',
  styleUrls: ['./tenants-profile.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class TenantsProfileComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('dataTable', { static: false }) dataTable: Table;
  @ViewChild('paginator', { static: false }) paginator: Paginator;
  public id: number;
  public loading = true;
  public shownLoginName: string;
  public tenancyName: string;
  public userName: string;
  public email: string;
  public phone: number;
  public givenId: number;
  sorting: string;
  skipCount: number;
  maxResultCount: number;
  shipperFacilities: any;
  InvoicingInfo: InvoicingInformationDto = new InvoicingInformationDto();
  profilePicture = AppConsts.appBaseUrl + '/assets/common/images/default-profile-picture.png';

  //Dummy data For Carrier Vases Table
  carrierVases = [
    { id: 1, vasName: 'Extra Driver', maxCount: 20, maxAmount: 6, price: 900 },
    { id: 2, vasName: 'Extra Truck', maxCount: 20, maxAmount: 6, price: 900 },
    { id: 3, vasName: 'Insurance', maxCount: 20, maxAmount: 6, price: 900 },
    { id: 4, vasName: 'Extra Packing', maxCount: 20, maxAmount: 6, price: 900 },
    { id: 5, vasName: 'Extra Driver', maxCount: 20, maxAmount: 6, price: 900 },
    { id: 6, vasName: 'Extra Truck', maxCount: 20, maxAmount: 6, price: 900 },
    { id: 7, vasName: 'Insurance', maxCount: 20, maxAmount: 6, price: 900 },
    { id: 8, vasName: 'Extra Packing', maxCount: 20, maxAmount: 6, price: 900 },
  ];
  //Dummy Carrier Service Areas

  carrierServiceAreas = [
    { id: 1, areaName: 'Area 1' },
    { id: 1, areaName: 'Area 2' },
    { id: 1, areaName: 'Area 3' },
    { id: 1, areaName: 'Area 4' },
    { id: 1, areaName: 'Area 5' },
    { id: 1, areaName: 'Area 1' },
    { id: 1, areaName: 'Area 2' },
    { id: 1, areaName: 'Area 3' },
  ];
  private active = false;
  selectedFacility: any;
  latitude: number;
  longitude: number;
  facilite: PagedResultDtoOfFacilityLocationListDto;

  constructor(
    injector: Injector,
    private _profileServiceProxy: ProfileServiceProxy,
    private _Activatedroute: ActivatedRoute,
    private _router: Router
  ) {
    super(injector);
  }
  get isCarrier(): boolean {
    return this.feature.isEnabled('App.Carrier');
  }
  get isShipper(): boolean {
    return this.feature.isEnabled('App.Shipper');
  }

  ngOnInit(): void {
    this.givenId = parseInt(this._Activatedroute.parent.snapshot.paramMap.get('id'));
    console.log(this._Activatedroute.snapshot.paramMap);
    this.getInvoicingInfo();
  }
  ngAfterViewInit() {
    this.active = true;
    this.getFacility();
  }

  getFacility(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._profileServiceProxy
      .getFacilitiesInformation(
        this.givenId,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getSkipCount(this.paginator, event),
        this.primengTableHelper.getMaxResultCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  reloadPage(): void {
    this.paginator.changePage(this.paginator.getPage());
  }
  getInvoicingInfo() {
    this._profileServiceProxy
      .getInvoicingInformation(this.givenId)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe(
        (result) => {
          this.InvoicingInfo = result;
        },
        (error) => {
          // this._router.navigate(['/app/main/dashboard']);
        }
      );
  }
}
