import { AfterViewInit, Component, Injector, Input, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'app-view-rating',
  templateUrl: './view-rating.component.html',
  styleUrls: ['./view-rating.component.css'],
})
export class ViewRatingComponent extends AppComponentBase implements AfterViewInit {
  @Input() rate: number;
  @Input() max: number = 5;
  @Input() rateNumber: number;

  constructor(injector: Injector) {
    super(injector);
  }

  ngAfterViewInit(): void {
    throw new Error('Method not implemented.');
  }
}
