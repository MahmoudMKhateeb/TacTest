import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { ControlContainer, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-create-or-edit-working-hours',
  templateUrl: './create-or-edit-working-hours.component.html',
  styleUrls: ['./create-or-edit-working-hours.component.css'],
  viewProviders: [{ provide: ControlContainer, useExisting: NgForm }],
})
export class CreateOrEditWorkingHoursComponent extends AppComponentBase implements OnInit {
  @Input() parentForm: NgForm;
  @Input() FacilityWorkingHoursInput: any[];

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}