import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-carrier-dashboard',
  templateUrl: './carrier-dashboard.component.html',
  styleUrls: ['./carrier-dashboard.component.css'],
})
export class CarrierDashboardComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
