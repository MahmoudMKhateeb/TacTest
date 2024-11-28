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
import { PointTransactionDto, RoutePointStatus, ShippingTypeEnum, TrackingRoutePointDto } from '@shared/service-proxies/service-proxies';
import { CustomStep } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/custom-timeline/custom-step';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-custom-timeline',
  templateUrl: './custom-timeline.component.html',
  styleUrls: ['./custom-timeline.component.scss'],
})
export class CustomTimelineComponent extends AppComponentBase implements OnInit, AfterViewInit, AfterContentChecked {
  @Output() invokeStatus: EventEmitter<{ point: TrackingRoutePointDto; transaction: PointTransactionDto; isUploadStep: boolean }> = new EventEmitter<{
    point: TrackingRoutePointDto;
    transaction: PointTransactionDto;
    isUploadStep: boolean;
  }>();
  @Input('steps') steps: CustomStep[];
  @Input('point') point: TrackingRoutePointDto;
  @Input('shippingType') shippingType: ShippingTypeEnum;
  @Input('busyPointId') busyPointId: number;
  @Input('saving') saving: boolean;

  constructor(injector: Injector, private cdRef: ChangeDetectorRef) {
    super(injector);
  }

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.cdRef.detectChanges();
  }

  ngAfterContentChecked() {
    this.cdRef.detectChanges();
  }

  isClickable(point: TrackingRoutePointDto, event: CustomStep): { class: string; canClick: boolean; isUploadStep?: boolean } {
    if (this.isShipperActor) {
      return { class: 'p-disabled', canClick: false };
    }
    if (this.shippingType != ShippingTypeEnum.ImportPortMovements && this.shippingType != ShippingTypeEnum.ExportPortMovements) {
      return { class: '', canClick: false };
    }
    if (!point.availableTransactions.length) {
      if (event.index === point.statues.length && point.isHasAdditionalSteps && isNotNullOrUndefined(point.availableSteps)) {
        // debugger;
        if (point.availableSteps?.length > 0 && point.status >= RoutePointStatus.FinishOffLoadShipment) {
          return { class: 'active-status clickable-item', canClick: true, isUploadStep: true };
        } else {
          event.isDone = true;
          return { class: 'p-disabled', canClick: false };
        }
      }
      return { class: '', canClick: false };
    }
    if (point.availableTransactions.length > 0) {
      const currentStatus = point.statues[event.index - 1];
      for (let i = 0; i < point.availableTransactions.length; i++) {
        const transaction = point.availableTransactions[i];
        if (transaction.toStatus === currentStatus.status) {
          return { class: 'active-status clickable-item', canClick: true };
        }
      }
    }
    return { class: '', canClick: false };
  }

  clickedOnStep(event: any, point: TrackingRoutePointDto, canClick: boolean, isUploadStep: boolean) {
    console.log('clickedOnStep event', event);
    event.preventDefault();
    event.stopPropagation();
    console.log('clickedOnStep canClick', canClick);
    console.log('clickedOnStep point', point);
    if (!canClick) {
      return;
    }
    this.statusInvoked({ point: point, transaction: point.availableTransactions[0], isUploadStep });
  }

  statusInvoked(value: { point: TrackingRoutePointDto; transaction: PointTransactionDto; isUploadStep: boolean }) {
    if ((!value.isUploadStep && !value.point.availableTransactions.length) || isNotNullOrUndefined(this.busyPointId)) {
      return;
    }
    this.invokeStatus.emit(value);
  }

  itemTrackBy(index, item: CustomStep) {
    return item.index;
  }
}
