import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { ShippingRequestRouteType, TmsPricePackageServiceProxy } from '@shared/service-proxies/service-proxies';
import { Paginator } from '@node_modules/primeng/paginator';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'app-view-matching-tms-price-package',
  templateUrl: './view-matching-tms-price-package.component.html',
  styleUrls: ['./view-matching-tms-price-package.component.css'],
})
export class ViewMatchingTmsPricePackageComponent extends AppComponentBase implements OnInit {
  isDataRequested: boolean;
  @Input() shippingRequestId: number;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  routeTypes;

  constructor(private injector: Injector, private _enumToArray: EnumToArrayPipe, private _tmsPricePackageServiceProxy: TmsPricePackageServiceProxy) {
    super(injector);
  }

  getMatchingTmsPricePackages($event?: LazyLoadEvent) {
    this.primengTableHelper.showLoadingIndicator();

    this._tmsPricePackageServiceProxy
      .getMatchingPricePackages(
        this.shippingRequestId,
        this.primengTableHelper.getMaxResultCount(this.paginator, $event),
        this.primengTableHelper.getSkipCount(this.paginator, $event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.primengTableHelper.hideLoadingIndicator();
        this.isDataRequested = true;
      });
  }

  ngOnInit(): void {
    this.isDataRequested = false;
    this.routeTypes = this._enumToArray.transform(ShippingRequestRouteType);
  }

  getRouteTypeDisplayName(key): string {
    return this.routeTypes?.find((x) => x.key == key)?.value;
  }
}
