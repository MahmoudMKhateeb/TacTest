import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { NormalPricePackageDto, NormalPricePackagesServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'viewNormalPricePackageModal',
  templateUrl: './view-normal-price-package-modal.component.html',
})
export class ViewNormalPricePackageModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  normalPricePackage: NormalPricePackageDto;

  constructor(injector: Injector, private _normalPricePackagesServiceProxy: NormalPricePackagesServiceProxy) {
    super(injector);
    this.normalPricePackage = new NormalPricePackageDto();
  }

  show(normalPricePackageId: number): void {
    this._normalPricePackagesServiceProxy.getNormalPricePackage(normalPricePackageId).subscribe((result) => {
      this.normalPricePackage = result;
    });
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
