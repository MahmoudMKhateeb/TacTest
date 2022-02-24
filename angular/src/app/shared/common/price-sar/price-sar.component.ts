import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'price-sar',
  templateUrl: './price-sar.component.html',
})
export class PriceSARComponent extends AppComponentBase implements OnInit {
  @Input() value: any;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
