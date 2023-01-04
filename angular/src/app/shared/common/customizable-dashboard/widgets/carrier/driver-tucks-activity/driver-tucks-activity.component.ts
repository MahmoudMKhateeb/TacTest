import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivityItemsDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-driver-tucks-activity',
  templateUrl: './driver-tucks-activity.component.html',
  styleUrls: ['./driver-tucks-activity.component.css'],
})
export class DriverTucksActivityComponent extends AppComponentBase {
  items: ActivityItemsDto;

  constructor(private injector: Injector) {
    super(injector);
  }
}
