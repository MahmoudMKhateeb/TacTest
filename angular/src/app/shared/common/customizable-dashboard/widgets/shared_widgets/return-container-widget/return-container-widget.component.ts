import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ContainerReturnTrackerWidgetDataDto, TenantDashboardServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'return-container-widget',
  templateUrl: './return-container-widget.component.html',
  styleUrls: ['./return-container-widget.component.scss'],
})
export class ReturnContainerWidgetComponent extends AppComponentBase implements OnInit {
  loading = false;
  data: ContainerReturnTrackerWidgetDataDto;
  busy = false;

  constructor(private injector: Injector, private myService: TenantDashboardServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.busy = true;
    setTimeout(() => {
      this.getData();
    }, 3000);
  }

  getData() {
    this.myService.getContainerReturnTrackerWidgetData(3).subscribe((res) => {
      this.data = res;
      this.busy = false;
    });
  }
}
