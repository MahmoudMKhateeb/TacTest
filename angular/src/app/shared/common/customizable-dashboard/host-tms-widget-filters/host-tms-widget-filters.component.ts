import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from '@node_modules/moment';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { AllSaasTruckAggregationFilterEnum } from '@app/shared/common/customizable-dashboard/host-tms-widget-filters/all-saas-truckaggregation-filter-enum';
import { NormalSaasHomeDeliveryEnum } from '@app/shared/common/customizable-dashboard/host-tms-widget-filters/normal-saas-homedelivery-enum';

@Component({
  selector: 'app-host-tms-widget-filters',
  templateUrl: './host-tms-widget-filters.component.html',
  styleUrls: ['./host-tms-widget-filters.component.scss'],
})
export class HostTmsWidgetFiltersComponent extends AppComponentBase implements OnInit, OnDestroy {
  @Output() optionSelected: EventEmitter<{ start: moment.Moment; end: moment.Moment }> = new EventEmitter<{
    start: moment.Moment;
    end: moment.Moment;
  }>(null);
  @Output() filterSelected: EventEmitter<number> = new EventEmitter<number>();
  @Input() isDateDropDown = true;
  @Input() isForTripType = false;
  @Input() showCurrentMonth = false;
  isDropdownOpen = false;
  showCustomStartEnd = false;
  start: Date;
  end: Date;
  filtersArray: any[] = [];

  constructor(injector: Injector, private enumService: EnumToArrayPipe) {
    super(injector);
  }

  ngOnDestroy(): void {}

  ngOnInit(): void {
    if (this.isDateDropDown) {
      const startDate = moment().subtract(10, 'months').startOf('month');
      const endDate = moment();
      const normalizedStartDate = moment({ year: startDate.year(), month: startDate.month(), day: 1 });
      const normalizedEndDate = moment({ year: endDate.year(), month: endDate.month(), day: endDate.daysInMonth() });
      this.optionSelected.emit({ start: normalizedStartDate, end: normalizedEndDate });
      return;
    }
    if (this.isForTripType) {
      this.filtersArray = this.enumService.transform(NormalSaasHomeDeliveryEnum).map((item) => {
        item.key = Number(item.key);
        return item;
      });
      this.filterSelected.emit(NormalSaasHomeDeliveryEnum.Normal);
      return;
    }
    this.filtersArray = this.enumService.transform(AllSaasTruckAggregationFilterEnum).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.filterSelected.emit(AllSaasTruckAggregationFilterEnum.All);
  }

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  selectOption(option: number) {
    console.log('Selected option:', option);

    const emitDatesAndToggle = (start: moment.Moment, end: moment.Moment) => {
      this.optionSelected.emit({ start, end });
      this.showCustomStartEnd = false;
      this.toggleDropdown();
    };

    switch (option) {
      case 1: {
        const startDate = moment().subtract(4, 'months').startOf('month');
        const endDate = moment();
        emitDatesAndToggle(
          moment({ year: startDate.year(), month: startDate.month(), day: 1 }),
          moment({ year: endDate.year(), month: endDate.month(), day: endDate.daysInMonth() })
        );
        break;
      }
      case 2: {
        const startDate = moment().subtract(10, 'months').startOf('month');
        const endDate = moment();
        emitDatesAndToggle(
          moment({ year: startDate.year(), month: startDate.month(), day: 1 }),
          moment({ year: endDate.year(), month: endDate.month(), day: endDate.daysInMonth() })
        );
        break;
      }
      case 3: {
        const startDate = moment().subtract(1, 'year').startOf('year').add(1, 'month');
        const endDate = moment().subtract(1, 'year').endOf('year');
        emitDatesAndToggle(
          moment({ year: startDate.year(), month: startDate.month(), day: 1 }),
          moment({ year: endDate.year(), month: endDate.month(), day: endDate.daysInMonth() })
        );
        break;
      }
      case 4: {
        this.showCustomStartEnd = !this.showCustomStartEnd;
        break;
      }
      case 5: {
        const startDate = moment().startOf('week');
        const endDate = moment().endOf('week');
        emitDatesAndToggle(startDate, endDate);
        break;
      }
      case 6:
      case 7: {
        const startOfMonth = moment().startOf('month');
        const endOfMonth = moment().endOf('month');
        emitDatesAndToggle(startOfMonth, endOfMonth);
        break;
      }
      default:
        break;
    }
  }

  emitCustomDate() {
    this.optionSelected.emit({ start: moment(this.start), end: moment(this.end) });
    this.toggleDropdown();
  }

  selectFilter(filter: number) {
    this.filterSelected.emit(filter);
    this.toggleDropdown();
  }
}
