import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-most-active-actor-shipper',
  templateUrl: './most-active-actor-shipper.component.html',
  styleUrls: ['./most-active-actor-shipper.component.scss'],
})
export class MostActiveActorShipperComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
