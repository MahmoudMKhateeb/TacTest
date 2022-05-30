import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  PagedResultDtoOfTrackingByWaybillDto,
  TrackingByWaybillDto,
  TrackingByWaybillRoutPointDto,
  TrackingServiceProxy,
} from '@shared/service-proxies/service-proxies';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-tracking-outside-platform',
  templateUrl: './tracking-outside-platform.component.html',
  styleUrls: ['./tracking-outside-platform.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class TrackingOutsidePlatformComponent extends AppComponentBase implements OnInit {
  dropPoints: TrackingByWaybillDto[];
  subWabillData: [TrackingByWaybillRoutPointDto];
  constructor(injector: Injector, private _trackingservice: TrackingServiceProxy) {
    super(injector);
  }
  waybillNumber: number;
  loading: boolean;
  ngOnInit(): void {}

  trackByWaybillNumber() {
    this.loading = true;
    this._trackingservice.getDropsOffByMasterWaybill(this.waybillNumber).subscribe((res) => {
      this.dropPoints = res.items;
      this.loading = false;
    });
  }

  trackSubWaybill(subWaybill: any) {
    this._trackingservice.getDropOffBySubWaybill(subWaybill).subscribe((res) => {
      this.subWabillData = [res];
    });
  }
}
