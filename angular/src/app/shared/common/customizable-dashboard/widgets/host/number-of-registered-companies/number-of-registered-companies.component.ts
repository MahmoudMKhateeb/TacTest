import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-number-of-registered-companies',
  templateUrl: './number-of-registered-companies.component.html',
  styleUrls: ['./number-of-registered-companies.component.scss'],
})
export class NumberOfRegisteredCompaniesComponent extends AppComponentBase implements OnInit {
  constructor(private injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
