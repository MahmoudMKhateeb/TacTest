import { Component, Injector, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ProfileServiceProxy, ServiceAreaListItemDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'carrier-service-areas',
  templateUrl: './carrier-service-areas.component.html',
  styleUrls: ['./carrier-service-areas.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class CarrierServiceAreasComponent extends AppComponentBase implements OnInit {
  @Input() givenUserId: number;
  serviceAreas: ServiceAreaListItemDto[];

  constructor(injector: Injector, private _profileServiceProxy: ProfileServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.getCarrierServiceAreas();
  }

  getCarrierServiceAreas() {
    this._profileServiceProxy.getTenantProfileInformationForView(this.givenUserId).subscribe((res) => {
      this.serviceAreas = res.serviceAreas;
    });
  }
}
