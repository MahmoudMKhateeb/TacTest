import { AppComponentBase } from '@shared/common/app-component-base';
import { Component, Injector, OnInit } from '@angular/core';

@Component({
  selector: 'app-new-actors-this-month',
  templateUrl: './new-actors-this-month.component.html',
  styleUrls: ['./new-actors-this-month.component.scss'],
})
export class NewActorsThisMonthComponent extends AppComponentBase implements OnInit {
  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
