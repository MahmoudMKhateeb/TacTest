import { AfterContentInit, AfterViewChecked, AfterViewInit, Component, Injector, OnInit, ViewChild } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, ActivatedRouteSnapshot, Router } from '@angular/router';
import { ReplaceTrucksAndDriversModalComponent } from '@app/main/shippingRequests/shippingRequests/request-templates/replace-trucks-and-drivers-modal/replace-trucks-and-drivers-modal.component';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  templateUrl: './shippingrequestslist.component.html',
  animations: [appModuleAnimation()],
})
export class ShippingRequestsListComponent extends AppComponentBase implements AfterViewInit {
  @ViewChild('replaceTrucksAndDriversModal', { static: true }) replaceTrucksAndDriversModal: ReplaceTrucksAndDriversModalComponent;
  dedicatedTruckId: string;
  dedicatedDriverId: string;
  shippingRequestId: string;
  truckTypeId: string;

  constructor(injector: Injector, private route: ActivatedRoute) {
    super(injector);
    console.log('route', route);
  }

  ngAfterViewInit() {
    this.shippingRequestId = this.route.snapshot.queryParamMap.get('shippingRequestId');
    this.dedicatedTruckId = this.route.snapshot.queryParamMap.get('dedicatedTruckId');
    this.dedicatedDriverId = this.route.snapshot.queryParamMap.get('dedicatedDriverId');
    this.truckTypeId = this.route.snapshot.queryParamMap.get('truckTypeId');
    if (isNotNullOrUndefined(this.shippingRequestId)) {
      this.replaceTrucksAndDriversModal.showFromNotification(
        Number(this.shippingRequestId),
        isNotNullOrUndefined(this.dedicatedTruckId),
        Number(this.dedicatedDriverId),
        Number(this.dedicatedTruckId),
        Number(this.truckTypeId)
      );
    }
  }
}
