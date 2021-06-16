import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  templateUrl: './marketplacelist.component.html',
  animations: [appModuleAnimation()],
})
export class MarketPlaceListComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
