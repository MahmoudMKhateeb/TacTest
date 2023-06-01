import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { GetRegisteredCompaniesNumberOutput, TMSAndHostDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-number-of-registered-companies',
  templateUrl: './number-of-registered-companies.component.html',
  styleUrls: ['./number-of-registered-companies.component.scss'],
})
export class NumberOfRegisteredCompaniesComponent extends AppComponentBase implements OnInit {
  registeredCompaniesNumber: GetRegisteredCompaniesNumberOutput;

  constructor(private injector: Injector, private _TMSAndHostDashboardServiceProxy: TMSAndHostDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.fetchData();
  }

  private fetchData() {
    this._TMSAndHostDashboardServiceProxy.getRegisteredCompaniesNumber().subscribe((res) => {
      this.registeredCompaniesNumber = res;
    });
  }
}
