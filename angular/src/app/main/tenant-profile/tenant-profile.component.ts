import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'tenant-profile',
  templateUrl: './tenant-profile.component.html',
  styleUrls: ['./tenant-profile.component.css'],
})
export class TenantProfileComponent implements OnInit {
  constructor(injector: Injector) {
    // super(injector);
  }
  ngOnInit(): void {
    console.log('Profiling Component Is Working');
  }
}
