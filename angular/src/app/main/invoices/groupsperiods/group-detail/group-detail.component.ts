import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/common/app-component-base';
import { Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { GroupPeriodInfoDto } from '@shared/service-proxies/service-proxies';

@Component({
  animations: [appModuleAnimation()],
  templateUrl: './group-detail.component.html',
})
export class GroupDetailComponent extends AppComponentBase implements OnInit {
  Data: GroupPeriodInfoDto;
  constructor(injector: Injector, private route: ActivatedRoute, private router: Router) {
    super(injector);
    this.Data = this.route.snapshot.data.groupinfo;
  }

  ngOnInit(): void {}
}
