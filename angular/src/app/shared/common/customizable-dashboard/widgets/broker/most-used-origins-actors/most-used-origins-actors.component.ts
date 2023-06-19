import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-most-used-origins-actors',
  templateUrl: './most-used-origins-actors.component.html',
  styleUrls: ['./most-used-origins-actors.component.scss'],
})
export class MostUsedOriginsActorsComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
