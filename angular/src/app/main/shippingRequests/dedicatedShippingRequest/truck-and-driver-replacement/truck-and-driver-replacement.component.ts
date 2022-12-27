import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { DedicatedShippingRequestsServiceProxy, RequestReplaceDriverInput, RequestReplaceTruckInput } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-truck-and-driver-replacement',
  templateUrl: './truck-and-driver-replacement.component.html',
  styleUrls: ['./truck-and-driver-replacement.component.css'],
})
export class TruckAndDriverReplacementComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  loading = false;
  isForTruck: boolean;
  private dedicatedTruckOrDriverId: number;
  public replacementReason: string;
  public replacementIntervalInDays: number;
  public get shouldDisable(): boolean {
    return !this.replacementIntervalInDays?.toString() || this.replacementIntervalInDays?.toString().length === 0;
  }

  constructor(injector: Injector, private _dedicatedShippingRequestsServiceProxy: DedicatedShippingRequestsServiceProxy) {
    super(injector);
  }

  ngOnInit() {}

  show(isForTruck: boolean, dedicatedTruckOrDriverId?: number): void {
    this.isForTruck = isForTruck;
    this.dedicatedTruckOrDriverId = dedicatedTruckOrDriverId;
    this.modal.show();
  }

  close(): void {
    this.isForTruck = false;
    this.dedicatedTruckOrDriverId = null;
    this.replacementReason = null;
    this.replacementIntervalInDays = null;
    this.modal.hide();
  }

  save() {
    this.loading = true;
    if (this.isForTruck) {
      const requestReplaceTruckInput = new RequestReplaceTruckInput({
        dedicatedTruckId: this.dedicatedTruckOrDriverId,
        replacementReason: this.replacementReason,
        replacementIntervalInDays: this.replacementIntervalInDays,
      });
      this._dedicatedShippingRequestsServiceProxy.requestReplaceTruck(requestReplaceTruckInput).subscribe((res) => {
        this.loading = false;
        this.close();
      });
    } else {
      const requestReplaceDriverInput = new RequestReplaceDriverInput({
        dedicatedDriverId: this.dedicatedTruckOrDriverId,
        replacementReason: this.replacementReason,
        replacementIntervalInDays: this.replacementIntervalInDays,
      });
      this._dedicatedShippingRequestsServiceProxy.requestReplaceDriver(requestReplaceDriverInput).subscribe((res) => {
        this.loading = false;
        this.close();
      });
    }
  }
}
