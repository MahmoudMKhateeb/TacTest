import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-number-of-regesterd-shippers',
  templateUrl: './number-of-regesterd-shippers.component.html',
  styles: [],
})
export class NumberOfRegesterdShippersComponent extends AppComponentBase implements OnInit {
  @Input() shippersCount: number;
  loading = false;

  constructor(private injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
