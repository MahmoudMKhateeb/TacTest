import { Component, EventEmitter, Injector, OnDestroy, OnInit, Output } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from '@node_modules/moment';

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
  isDropdownOpen = false;
  showCustomStartEnd = false;
  start: Date;
  end: Date;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnDestroy(): void {}

  ngOnInit(): void {
    // this.selectOption(2);
    const startDate = moment().subtract(12, 'months').startOf('month');
    const endDate = moment();
    this.optionSelected.emit({ start: startDate, end: endDate });
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
      default:
        break;
    }
  }

  emitCustomDate() {
    this.optionSelected.emit({ start: moment(this.start), end: moment(this.end) });
    this.toggleDropdown();
  }
}
