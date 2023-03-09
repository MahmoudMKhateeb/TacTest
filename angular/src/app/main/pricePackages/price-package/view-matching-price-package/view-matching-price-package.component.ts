import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { LazyLoadEvent } from 'primeng/api';
import { ShippingRequestRouteType, PricePackageForViewDto, PricePackageServiceProxy } from '@shared/service-proxies/service-proxies';
import { Paginator } from '@node_modules/primeng/paginator';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-view-matching-price-package',
  templateUrl: './view-matching-price-package.component.html',
  styleUrls: ['./view-matching-price-package.component.css'],
})
export class ViewMatchingPricePackageComponent extends AppComponentBase implements OnInit {
  isDataRequested: boolean;
  @Input() shippingRequestId: number;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  routeTypes;
  loadingPricePackageId: string;

  constructor(private injector: Injector, private _enumToArray: EnumToArrayPipe, private _pricePackageServiceProxy: PricePackageServiceProxy) {
    super(injector);
  }

  getMatchingTmsPricePackages($event?: LazyLoadEvent) {
    this.primengTableHelper.showLoadingIndicator();

    this._pricePackageServiceProxy
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
    this.loadingPricePackageId = undefined;
    this.routeTypes = this._enumToArray.transform(ShippingRequestRouteType);
  }

  getRouteTypeDisplayName(key): string {
    return this.routeTypes?.find((x) => x.key == key)?.value;
  }

  handlePricePackageAction(pricePackage: PricePackageForViewDto) {
    this.loadingPricePackageId = pricePackage.pricePackageId;
    this._pricePackageServiceProxy
      .applyPricePackage(pricePackage.id, this.shippingRequestId)
      .pipe(finalize(() => (this.loadingPricePackageId = undefined)))
      .subscribe(() => {
        this.notify.success('SentSuccessfully');
        this.getMatchingTmsPricePackages({});
      });
  }
  /*
  acceptOnBehalfCarrier(pricePackage: TmsPricePackageForViewDto) {
    this.loadingPricePackageId = pricePackage.pricePackageId;
    this._tmsPricePackageServiceProxy
      .acknowledgeOnBehalfCarrier(pricePackage.id, this.shippingRequestId, pricePackage.isTmsPricePackage)
      .pipe(finalize(() => (this.loadingPricePackageId = undefined)))
      .subscribe(() => {
        this.notify.success('SentSuccessfully');
        this.getMatchingTmsPricePackages({});
      });
  }*/
}
