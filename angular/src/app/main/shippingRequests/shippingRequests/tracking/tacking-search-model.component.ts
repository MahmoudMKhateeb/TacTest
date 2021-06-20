import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import * as moment from 'moment';
import { TrackingSearchInput } from '../../../../shared/common/search/TrackingSearchInput';
import {
  PriceOfferServiceProxy,
  GetShippingRequestSearchListDto,
  ComboboxItemDto,
  ShippingRequestRouteType,
  PriceOfferChannel,
  ShippingRequestType,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';

@Component({
  templateUrl: './tacking-search-model.component.html',
  styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'tacking-search-model',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class TrackinSearchModelComponent extends AppComponentBase implements OnInit {
  @Output() modalsearch: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  isLoad: boolean = false;
  active = false;
  saving = false;
  input: TrackingSearchInput = new TrackingSearchInput();
  direction: string;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  pickupDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive: boolean;
  pickupDateRangeActive: boolean;
  searchList: GetShippingRequestSearchListDto;
  cites: ComboboxItemDto[] = [];
  truckTypes: ComboboxItemDto[] = [];
  statusData: object[] = [];
  routeTypes: any;
  requestTypes: any;
  constructor(injector: Injector, private _currentSrv: PriceOfferServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }
  ngOnInit(): void {
    this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType);
    this.requestTypes = this.enumToArray.transform(ShippingRequestType);
    this.getRequestStatus();
  }

  getData() {
    if (this.isLoad) return;
    this.isLoad = true;
    this._currentSrv.getAllListForSearch().subscribe((result) => {
      this.cites = result.cities.map((x) => {
        let item: ComboboxItemDto = new ComboboxItemDto();
        item.displayText = x.displayName;
        item.value = x.id.toString();
        return item;
      });
      this.truckTypes = result.trucksTypes.map((x) => {
        let item: ComboboxItemDto = new ComboboxItemDto();
        item.displayText = x.translatedDisplayName;
        item.value = x.id.toString();
        return item;
      });
    });
  }
  show(Input: TrackingSearchInput): void {
    this.input = Input;
    this.getData();
    this.direction = document.getElementsByTagName('html')[0].getAttribute('dir');
    this.active = true;
    this.modal.show();
  }
  close(): void {
    this.active = false;
    this.modal.hide();
  }

  search(): void {
    if (this.creationDateRangeActive) {
      this.input.fromDate = moment(this.creationDateRange[0]);
      this.input.toDate = moment(this.creationDateRange[1]);
    } else {
      this.input.fromDate = null;
      this.input.toDate = null;
    }

    if (this.pickupDateRangeActive) {
      this.input.pickupFromDate = moment(this.pickupDateRange[0]);
      this.input.pickupToDate = moment(this.pickupDateRange[1]);
    } else {
      this.input.pickupFromDate = null;
      this.input.pickupToDate = null;
    }

    this.modalsearch.emit(null);
    this.close();
  }

  getRequestStatus() {
    this.statusData.push(
      { displayText: this.l('New'), value: '0' },
      { displayText: this.l('Intransit'), value: '1' },
      { displayText: this.l('Canceled'), value: '2' },
      { displayText: this.l('Delivered'), value: '3' }
    );
  }
}
