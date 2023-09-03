import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-most-active-actor-carrier',
  templateUrl: './most-active-actor-carrier.component.html',
  styleUrls: ['./most-active-actor-carrier.component.scss'],
})
export class MostActiveActorCarrierComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
