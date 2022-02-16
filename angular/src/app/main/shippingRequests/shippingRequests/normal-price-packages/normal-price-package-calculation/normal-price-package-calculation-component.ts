import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

import { NormalPricePackagesServiceProxy, BidNormalPricePackageDto, BidNormalPricePackageItemDto } from '@shared/service-proxies/service-proxies';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './normal-price-package-calculation-component.html',
  // styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'normal-price-package-calculation',
  animations: [appModuleAnimation()],
})
export class NormalPricePackageCalculationComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<number> = new EventEmitter<number>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  offer: BidNormalPricePackageDto = new BidNormalPricePackageDto();
  Items: BidNormalPricePackageItemDto[] = [];
  priceOfferCommissionType: any;
  commissionTypeTitle: string;
  shippingRequestId: number;
  pricePackageId: number;
  constructor(injector: Injector, private _CurrentServ: NormalPricePackagesServiceProxy) {
    super(injector);
  }

  show(id: number, pricePackageId: number | undefined = undefined): void {
    this.shippingRequestId = id;
    this.pricePackageId = pricePackageId;
    this._CurrentServ.calculateShippingRequestPricePackage(pricePackageId, id).subscribe((result) => {
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
    });
  }
  Preview(id: number, pricePackageId: number | undefined = undefined): void {
    this.shippingRequestId = id;
    this.pricePackageId = pricePackageId;
    this._CurrentServ.getBidNormalPricePackage(pricePackageId, id).subscribe((result) => {
      this.offer = result;
      this.Items = this.offer.items;
      this.active = true;
      this.modal.show();
    });
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }
  submit(id: number, pricePackageId: number): void {
    this.saving = true;
    this._CurrentServ
      .submitBidByPricePackage(pricePackageId, id)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe((result) => {
        this.notify.info(this.l('SendSuccessfully'));
        this.close();
      });
  }
}
