import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  templateUrl: './shippingrequestslist.component.html',
  animations: [appModuleAnimation()],
})
export class ShippingRequestsListComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
