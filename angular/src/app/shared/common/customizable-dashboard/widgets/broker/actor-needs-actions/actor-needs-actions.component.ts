import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-actor-needs-actions',
  templateUrl: './actor-needs-actions.component.html',
  styleUrls: ['./actor-needs-actions.component.scss'],
})
export class ActorNeedsActionsComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
