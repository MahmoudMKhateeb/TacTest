import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  InvoicePeriodServiceProxy,
  InvoicePeriodDto,
  InvoicePeriodType,
  KeyValuePair,
  FrequencyRelativeInterval,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'invoice-periods-modal',
  templateUrl: './invoice-periods-modal.component.html',
  providers: [EnumToArrayPipe],
})
export class InvoicePeriodsModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('nameInput', { static: false }) nameInput: ElementRef;

  Period: InvoicePeriodDto;
  PeriodType: any;
  active: boolean = false;
  saving: boolean = false;
  FreqRecurrence: string[] | undefined;
  FreqRecurrenceWeekDays: number[] = [];
  FreqRecurrenceMonths: number[] = [];
  FreqIntervalMonthlyperday: number = 1;
  FreqIntervalMonthlyperweek: number = 1;
  FreRelativeInterval: any;
  MonthType: string = '1';

  Months: KeyValuePair[];
  Weeks: KeyValuePair[];
  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private enumToArray: EnumToArrayPipe, private _periodService: InvoicePeriodServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.PeriodType = this.enumToArray.transform(InvoicePeriodType);
    this.FreRelativeInterval = this.enumToArray.transform(FrequencyRelativeInterval);
  }

  show(period: InvoicePeriodDto | null): void {
    this._periodService.getAllCommon().subscribe((result) => {
      this.Months = result.months;
      this.Weeks = result.weeks;
    });
    if (!period) {
      this.active = true;
      this.Period = new InvoicePeriodDto();
      this.Period.freqInterval = 1;
      this.Period.periodType = InvoicePeriodType.PayInAdvance;
      this.Period.freqRelativeInterval = 1;
      this.Period.creditLimit = 1;

      this.modal.show();
    } else {
      this.Period = period;
      this.active = true;

      if (this.Period.periodType == InvoicePeriodType.Weekly) {
        this.FreqRecurrenceWeekDays = this.Period.freqRecurrence.split(',').map((x) => parseInt(x));
      } else if (this.Period.periodType == InvoicePeriodType.Monthly) {
        this.FreqRecurrenceMonths = this.Period.freqRecurrence.split(',').map((x) => parseInt(x));
      }
      this.modal.show();
    }
  }

  onShown(): void {
    this.nameInput.nativeElement.focus();
  }

  onChangePeriodType(periodtype: InvoicePeriodType): void {}

  save(): void {
    this.saving = true;
    if (this.Period.periodType == InvoicePeriodType.Daily) {
      this.Period.freqRelativeInterval = 0;
      this.Period.freqRecurrence = null;
    } else if (this.Period.periodType == InvoicePeriodType.Weekly) {
      this.Period.freqRelativeInterval = 0;
      this.Period.freqRecurrence = this.FreqRecurrenceWeekDays.join();
    } else if (this.Period.periodType == InvoicePeriodType.Monthly) {
      this.Period.freqRecurrence = this.FreqRecurrenceMonths.join();

      if (this.MonthType == '1') {
        this.Period.freqRelativeInterval = 0;
        this.Period.freqInterval = this.FreqIntervalMonthlyperday;
      } else {
        this.Period.freqInterval = this.FreqIntervalMonthlyperweek;
      }
    } else {
      this.Period.creditLimit = 0;
      this.Period.freqInterval = 0;
      this.Period.freqRelativeInterval = 0;
      this.Period.freqRecurrence = null;
      this.Period.freqRelativeInterval = 0;
    }
    this._periodService
      .createEdit(this.Period)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(this.Period);
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
