import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PointTransactionDto, TrackingRoutePointDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-custom-marker',
  templateUrl: './custom-marker.component.html',
  styleUrls: ['./custom-marker.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class CustomMarkerComponent extends AppComponentBase implements OnInit {
  @Output() invokeStatus: EventEmitter<{ point: TrackingRoutePointDto; transaction: PointTransactionDto }> = new EventEmitter<{
    point: TrackingRoutePointDto;
    transaction: PointTransactionDto;
  }>();
  @Input('styleClass') styleClass: string;
  @Input('canClick') canClick: boolean;
  @Input('isDone') isDone: boolean;
  @Input('index') index: any;
  @Input('color') color: string;
  @Input('point') point: TrackingRoutePointDto;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}

  clickedOnStep(event: any) {
    console.log('clickedOnStep canClick', this.canClick);
    console.log('clickedOnStep point', this.point);
    event.preventDefault();
    event.stopPropagation();
    if (!this.canClick) {
      return;
    }
    this.invokeStatus.emit({ point: this.point, transaction: this.point.availableTransactions[0] });
  }
}
