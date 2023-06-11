import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-most-used-destinations-actors',
  templateUrl: './most-used-destinations-actors.component.html',
  styleUrls: ['./most-used-destinations-actors.component.scss'],
})
export class MostUsedDestinationsActorsComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
