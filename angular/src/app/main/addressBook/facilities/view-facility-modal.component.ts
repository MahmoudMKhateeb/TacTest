import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { GetFacilityForViewOutput, FacilityDto, FacilityWorkingHourDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ViewWorkingHoursComponent } from '@app/shared/common/workingHours/view-working-hours/view-working-hours.component';
import { WeekDay } from '@angular/common';

@Component({
  selector: 'viewFacilityModal',
  templateUrl: './view-facility-modal.component.html',
})
export class ViewFacilityModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('ViewWorkingHoursComponent', { static: true }) ViewWorkingHoursComponent: ViewWorkingHoursComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  FacilityWorkingHours: any[];

  active = false;
  saving = false;
  days = WeekDay;
  item: GetFacilityForViewOutput;

  constructor(injector: Injector) {
    super(injector);
    this.item = new GetFacilityForViewOutput();
    this.item.facility = new FacilityDto();
  }

  show(item: GetFacilityForViewOutput): void {
    if (item.facilityWorkingHours.length > 0) {
      this.FacilityWorkingHours = this.getDays(item.facilityWorkingHours);
    }
    this.item = item;
    this.active = true;
    this.modal.show();
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  getDays(items: FacilityWorkingHourDto[]) {
    const result = [];
    for (const item of items) {
      if (!Number.isNaN(Number(item.dayOfWeek)) && result.filter((r) => r.dayOfWeek == item.dayOfWeek).length > 1) {
        continue;
      }
      if (item && result.filter((r) => r.dayOfWeek === item.dayOfWeek).length == 0) {
        result.push({
          dayOfWeek: item.dayOfWeek,
          name: this.days[item.dayOfWeek],
          startTime: item.startTime,
          endTime: item.endTime,
          hasTime: true,
          id: item.id,
          facilityId: item.facilityId,
        });
      } else {
        result.push({ dayOfWeek: item.dayOfWeek, name: this.days[item.dayOfWeek], hasTime: false, facilityId: items[0].facilityId });
      }
    }
    return result;
  }
}
