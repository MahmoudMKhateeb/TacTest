import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import * as moment from 'moment';
import { ShippingRequestForPriceOfferGetAllInput } from '../../../../shared/common/search/ShippingRequestForPriceOfferGetAllInput';

@Component({
  templateUrl: './shipping-request-card-search-model.component.html',
  styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'shipping-request-card-search-model',
  animations: [appModuleAnimation()],
})
export class ShippingRequestCardSearchModelComponent extends AppComponentBase {
  @Output() modalsearch: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  input: ShippingRequestForPriceOfferGetAllInput = new ShippingRequestForPriceOfferGetAllInput();
  direction: string;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  pickupDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];

  creationDateRangeActive: boolean;
  pickupDateRangeActive: boolean;

  constructor(injector: Injector) {
    super(injector);
  }

  show(Input: ShippingRequestForPriceOfferGetAllInput): void {
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.input = Input;
    this.active = true;
    this.modal.show();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  search(): void {
    if (this.creationDateRangeActive) {
      this.input.fromDate = moment(this.creationDateRange[0]);
      this.input.toDate = moment(this.creationDateRange[1]);
    } else {
      this.input.fromDate = null;
      this.input.toDate = null;
    }

    if (this.pickupDateRangeActive) {
      this.input.pickupFromDate = moment(this.pickupDateRange[0]);
      this.input.pickupToDate = moment(this.pickupDateRange[1]);
    } else {
      this.input.pickupFromDate = null;
      this.input.pickupToDate = null;
    }

    this.modalsearch.emit(null);
    this.close();
  }
}
