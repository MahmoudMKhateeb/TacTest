import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { RoutesServiceProxy, GetRouteForViewDto, RouteDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { BreadcrumbItem } from '@app/shared/common/sub-header/sub-header.component';
@Component({
  templateUrl: './view-route.component.html',
  animations: [appModuleAnimation()],
})
export class ViewRouteComponent extends AppComponentBase implements OnInit {
  active = false;
  saving = false;

  item: GetRouteForViewDto;

  breadcrumbs: BreadcrumbItem[] = [
    new BreadcrumbItem(this.l('Route'), '/app/main/routs/routes'),
    new BreadcrumbItem(this.l('Routes') + '' + this.l('Details')),
  ];
  constructor(injector: Injector, private _activatedRoute: ActivatedRoute, private _routesServiceProxy: RoutesServiceProxy) {
    super(injector);
    this.item = new GetRouteForViewDto();
    this.item.route = new RouteDto();
  }

  ngOnInit(): void {
    this.show(this._activatedRoute.snapshot.queryParams['id']);
  }

  show(routeId: number): void {
    this._routesServiceProxy.getRouteForView(routeId).subscribe((result) => {
      this.item = result;
      this.active = true;
    });
  }
}
