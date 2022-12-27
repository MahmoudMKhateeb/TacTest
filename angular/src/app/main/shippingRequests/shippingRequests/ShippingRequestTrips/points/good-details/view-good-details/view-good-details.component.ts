import { Component, Injector, Input, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from '@node_modules/ngx-bootstrap/modal';
import { GetAllUnitOfMeasureForDropDownOutput, GoodsDetailDto, ShippingRequestsServiceProxy } from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'view-good-details',
  templateUrl: './view-good-details.component.html',
  styleUrls: ['./view-good-details.component.css'],
})
export class ViewGoodDetailsComponent extends AppComponentBase {
  @ViewChild('viewGoodDetail', { static: false }) public modal: ModalDirective;
  @Input() allSubGoodCategorys: any;
  active = false;
  goodDetails: any;
  allUnitOfMeasure: GetAllUnitOfMeasureForDropDownOutput[];

  constructor(injector: Injector, private _shippingRequest: ShippingRequestsServiceProxy) {
    super(injector);
  }

  show(record: any) {
    this.goodDetails = record;
    this._shippingRequest.getAllUnitOfMeasuresForDropdown().subscribe((result) => {
      this.allUnitOfMeasure = result;
      if (!isNotNullOrUndefined(this.goodDetails.unitOfMeasure)) {
        const unitOfMeasureId = this.goodDetails.unitOfMeasureId;
        const foundUnit = result.find((item) => Number(item.id) === unitOfMeasureId);
        this.goodDetails.unitOfMeasure = isNotNullOrUndefined(foundUnit) ? foundUnit.displayName : null;
      }
    });
    console.log('this.goodDetails', this.goodDetails);
    this.active = true;
    this.modal.show();
  }

  close() {
    this.active = false;
    this.modal.hide();
  }
}
