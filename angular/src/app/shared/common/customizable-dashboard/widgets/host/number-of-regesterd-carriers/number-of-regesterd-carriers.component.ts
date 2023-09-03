import { Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-number-of-regesterd-carriers',
  templateUrl: './number-of-regesterd-carriers.component.html',
  styles: [],
})
export class NumberOfRegesterdCarriersComponent extends AppComponentBase implements OnInit {
  @Input() carriersCount: number;
  loading = false;

  constructor(private injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}
}
