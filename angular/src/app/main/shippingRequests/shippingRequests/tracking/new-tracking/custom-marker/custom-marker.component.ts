import {
  AfterContentChecked,
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Injector,
  Input,
  OnInit,
  Output,
  ViewEncapsulation,
} from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PointTransactionDto, TrackingRoutePointDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-custom-marker',
  templateUrl: './custom-marker.component.html',
  styleUrls: ['./custom-marker.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class CustomMarkerComponent extends AppComponentBase implements OnInit, AfterViewInit, AfterContentChecked {
  @Output() invokeStatus: EventEmitter<{ point: TrackingRoutePointDto; transaction: PointTransactionDto; isUploadStep: boolean }> = new EventEmitter<{
    point: TrackingRoutePointDto;
    transaction: PointTransactionDto;
    isUploadStep: boolean;
  }>();
  @Input('styleClass') styleClass: string;
  @Input('canClick') canClick: boolean;
  @Input('isDone') isDone: boolean;
  @Input('index') index: any;
  @Input('color') color: string;
  @Input('point') point: TrackingRoutePointDto;

  constructor(injector: Injector, private cdRef: ChangeDetectorRef) {
    super(injector);
  }

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.isDone = this.isDone;
    this.cdRef.detectChanges();
  }

  ngAfterContentChecked() {
    this.cdRef.detectChanges();
  }

  clickedOnStep(event: any) {
    console.log('clickedOnStep canClick', this.canClick);
    console.log('clickedOnStep point', this.point);
    event.preventDefault();
    event.stopPropagation();
    if (!this.canClick) {
      return;
    }
    this.invokeStatus.emit({ point: this.point, transaction: this.point.availableTransactions[0], isUploadStep: false });
  }
}
