import { Component, Injector, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class ProfileComponent extends AppComponentBase implements OnInit {
  constructor(private _Activatedroute: ActivatedRoute, injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
