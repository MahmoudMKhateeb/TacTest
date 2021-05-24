import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  templateUrl: './comingSoon.component.html',
})
export class ComingSoonComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
