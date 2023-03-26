import { AfterViewInit, Component, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  templateUrl: './tms-request-list.component.html',
  animations: [appModuleAnimation()],
})
export class TMSRequestListComponent extends AppComponentBase implements AfterViewInit {
  showNormalView = true;
  constructor(injector: Injector, private route: ActivatedRoute, private router: Router) {
    super(injector);
    console.log('route', route);
  }

  ngAfterViewInit() {
    this.router.events.subscribe((event) => {
      console.log('event', event);
      if (event instanceof NavigationEnd) {
        this.showNormalView = this.route.snapshot.queryParamMap.get('showType')
          ? this.route.snapshot.queryParamMap.get('showType').toLowerCase() != '1'
          : true;
      }
    });
    // this.shippingRequestId = this.route.snapshot.queryParamMap.get('shippingRequestId');
    // this.dedicatedTruckId = this.route.snapshot.queryParamMap.get('dedicatedTruckId');
    // this.dedicatedDriverId = this.route.snapshot.queryParamMap.get('dedicatedDriverId');
    // this.truckTypeId = this.route.snapshot.queryParamMap.get('truckTypeId');
    this.showNormalView = this.route.snapshot.queryParamMap.get('showType')
      ? this.route.snapshot.queryParamMap.get('showType').toLowerCase() != '1'
      : true;
    // if (isNotNullOrUndefined(this.shippingRequestId)) {
    //     this.replaceTrucksAndDriversModal.showFromNotification(
    //         Number(this.shippingRequestId),
    //         isNotNullOrUndefined(this.dedicatedTruckId),
    //         Number(this.dedicatedDriverId),
    //         Number(this.dedicatedTruckId),
    //         Number(this.truckTypeId)
    //     );
    // }
  }
}
