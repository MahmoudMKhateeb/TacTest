import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-actor-upcoming-trips',
  templateUrl: './actor-upcoming-trips.component.html',
  styleUrls: ['./actor-upcoming-trips.component.scss'],
})
export class ActorUpcomingTripsComponent extends AppComponentBase {
  constructor(injector: Injector) {
    super(injector);
  }
}
