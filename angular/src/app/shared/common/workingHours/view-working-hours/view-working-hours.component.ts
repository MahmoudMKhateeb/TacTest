import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-view-working-hours',
  templateUrl: './view-working-hours.component.html',
  styleUrls: ['./view-working-hours.component.css'],
})
export class ViewWorkingHoursComponent extends AppComponentBase implements OnInit {
  @Input() FacilityWorkingHoursInput: any[];

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
