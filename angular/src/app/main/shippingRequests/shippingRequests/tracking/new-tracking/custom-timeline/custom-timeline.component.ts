import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PointTransactionDto, ShippingTypeEnum, TrackingRoutePointDto } from '@shared/service-proxies/service-proxies';
import { CustomStep } from '@app/main/shippingRequests/shippingRequests/tracking/new-tracking/custom-timeline/custom-step';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  selector: 'app-custom-timeline',
  templateUrl: './custom-timeline.component.html',
  styleUrls: ['./custom-timeline.component.scss'],
})
export class CustomTimelineComponent extends AppComponentBase implements OnInit {
  @Output() invokeStatus: EventEmitter<{ point: TrackingRoutePointDto; transaction: PointTransactionDto; isUploadStep: boolean }> = new EventEmitter<{
    point: TrackingRoutePointDto;
    transaction: PointTransactionDto;
    isUploadStep: boolean;
  }>();
  @Input('steps') steps: CustomStep[];
  @Input('point') point: TrackingRoutePointDto;
  @Input('shippingType') shippingType: ShippingTypeEnum;
  @Input('saving') saving: boolean;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}

  isClickable(point: TrackingRoutePointDto, event: CustomStep): { class: string; canClick: boolean; isUploadStep?: boolean } {
    if (this.shippingType != ShippingTypeEnum.ImportPortMovements && this.shippingType != ShippingTypeEnum.ExportPortMovements) {
      return { class: '', canClick: false };
    }
    if (!point.availableTransactions.length) {
      return { class: '', canClick: false };
    }
    if (point.availableTransactions.length > 0) {
      const currentStatus = point.statues[event.index - 1];
      for (let i = 0; i < point.availableTransactions.length; i++) {
        const transaction = point.availableTransactions[i];
        if (
          event.index === point.statues.length &&
          point.isHasAdditionalSteps &&
          isNotNullOrUndefined(point.availableSteps) &&
          point.availableSteps?.length > 0
        ) {
          return { class: 'active-status clickable-item', canClick: true, isUploadStep: true };
        }
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
    this.invokeStatus.emit(value);
  }

  itemTrackBy(index, item: CustomStep) {
    return item.index;
  }
}
