import { Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  templateUrl: './direct-request-view.component.html',
  animations: [appModuleAnimation()],
})
export class DirectRequestViewComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
