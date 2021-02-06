import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { CreateOrEditRoutStepDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'RouteStepsForCreateShippingRequest',
  templateUrl: './RouteStepsForCreateShippingRequest.html',
  styleUrls: ['./RouteStepsForCreateShippingRequest.scss'],
})
export class RouteStepsForCreateShippingRequstComponent extends AppComponentBase {
  routStep: CreateOrEditRoutStepDto = new CreateOrEditRoutStepDto();

  constructor(injector: Injector) {
    super(injector);
  }

  changeDetector() {
    //   console.log(this.routStep);
  }

  changeDetectorz() {
    console.log('aaa');
  }
}
