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
      const startDate = moment().subtract(12, 'months').startOf('month');
      const endDate = moment();
      this.optionSelected.emit({ start: startDate, end: endDate });
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
    // Perform the desired action based on the selected option
    // For example, emit an event or update a variable
    // You can handle each option accordingly in your application logic
    switch (option) {
      case 1: {
        const startDate = moment().subtract(6, 'months').startOf('month');
        const endDate = moment();
        this.optionSelected.emit({ start: startDate, end: endDate });
        console.log('Start Date:', startDate);
        console.log('End Date:', endDate);
        this.showCustomStartEnd = false;
        this.toggleDropdown();
        break;
      }
      case 2: {
        const startDate = moment().subtract(12, 'months').startOf('month');
        const endDate = moment();
        this.optionSelected.emit({ start: startDate, end: endDate });
        console.log('Start Date:', startDate);
        console.log('End Date:', endDate);
        this.showCustomStartEnd = false;
        this.toggleDropdown();
        break;
      }
      case 3: {
        const startDate = moment().subtract(1, 'year').startOf('year');
        const endDate = moment().subtract(1, 'year').endOf('year');
        this.optionSelected.emit({ start: startDate, end: endDate });
        console.log('Start Date:', startDate);
        console.log('End Date:', endDate);
        this.showCustomStartEnd = false;
        this.toggleDropdown();
        break;
      }
      case 4: {
        this.showCustomStartEnd = !this.showCustomStartEnd;
        break;
      }
      case 5: {
        const startDate = moment().startOf('week');
        const endDate = moment().endOf('week');

        this.optionSelected.emit({ start: startDate, end: endDate });
        console.log('Start Date:', startDate);
        console.log('End Date:', endDate);
        this.showCustomStartEnd = false;
        this.toggleDropdown();
        break;
      }
      case 6: {
        const startDate = moment().startOf('month');
        const endDate = moment().endOf('month');

        this.optionSelected.emit({ start: startDate, end: endDate });
        console.log('Start Date:', startDate);
        console.log('End Date:', endDate);
        this.showCustomStartEnd = false;
        this.toggleDropdown();
        break;
      }
      case 7: {
        const startDate = moment().subtract(1, 'month').startOf('month');
        const endDate = moment().subtract(1, 'month').endOf('month');

        this.optionSelected.emit({ start: startDate, end: endDate });
        console.log('Start Date:', startDate);
        console.log('End Date:', endDate);
        this.showCustomStartEnd = false;
        this.toggleDropdown();
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
