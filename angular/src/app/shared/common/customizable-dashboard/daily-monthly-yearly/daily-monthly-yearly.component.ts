import { Component, EventEmitter, Injector, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { FilterDatePeriod } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-daily-monthly-yearly',
  templateUrl: './daily-monthly-yearly.component.html',
  styleUrls: ['./daily-monthly-yearly.component.scss'],
})
export class DailyMonthlyYearlyComponent extends AppComponentBase implements OnInit {
  @Output() optionSelected: EventEmitter<FilterDatePeriod> = new EventEmitter<FilterDatePeriod>(null);
  options: { key: any; value: any }[] = this._enumService.transform(FilterDatePeriod);
  selectedOption = FilterDatePeriod.Monthly;

  constructor(injector: Injector, private _enumService: EnumToArrayPipe) {
    super(injector);
  }

  ngOnInit(): void {
    this.selectOption();
  }

  selectOption() {
    this.optionSelected.emit(this.selectedOption);
  }
}
