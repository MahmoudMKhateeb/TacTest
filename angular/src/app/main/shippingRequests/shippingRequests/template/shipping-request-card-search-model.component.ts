import { Component, OnInit, Injector, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';
import * as moment from 'moment';
import {
  PriceOfferServiceProxy,
  GetShippingRequestSearchListDto,
  ComboboxItemDto,
  ShippingRequestRouteType,
  PriceOfferChannel,
  ShippingRequestType,
  ShippingRequestForPriceOfferGetAllInput,
  ActorTypesEnum,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { finalize } from 'rxjs/operators';

@Component({
  templateUrl: './shipping-request-card-search-model.component.html',
  // styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'shipping-request-card-search-model',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class ShippingRequestCardSearchModelComponent extends AppComponentBase implements OnInit {
  @Output() modalsearch: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  isLoad = false;
  active = false;
  saving = false;
  isDataLoading = false;
  input: ShippingRequestForPriceOfferGetAllInput = new ShippingRequestForPriceOfferGetAllInput();
  direction: string;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  pickupDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive = false;
  pickupDateRangeActive = false;
  searchList: GetShippingRequestSearchListDto;
  cites: ComboboxItemDto[] = [];
  truckTypes: ComboboxItemDto[] = [];
  statusData: { displayText: string; value: number }[] = [];
  routeTypes: any;
  requestTypes: any;
  AllActorTypes = this.enumToArray.transform(ActorTypesEnum);
  constructor(injector: Injector, private _currentSrv: PriceOfferServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }
  ngOnInit(): void {
    this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType).map((item) => {
      item.value = this.l(item.value);
      return item;
    });
    this.requestTypes = this.enumToArray.transform(ShippingRequestType).map((item) => {
      item.value = this.l(item.value);
      return item;
    });
    const obj = {
      value: this.l('All'),
      key: '',
    };
    this.routeTypes.unshift(obj);
    this.requestTypes.unshift(obj);
  }

  getData() {
    if (this.isLoad) return;
    this.isLoad = true;
    this.isDataLoading = true;
    this._currentSrv
      .getAllListForSearch()
      .pipe(finalize(() => (this.isDataLoading = false)))
      .subscribe((result) => {
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
        const combo = new ComboboxItemDto();
        combo.displayText = this.l('All');
        combo.value = '';
        this.cites.unshift(combo);
        this.truckTypes.unshift(combo);
      });
  }
  show(Input: ShippingRequestForPriceOfferGetAllInput): void {
    this.input = Input;
    this.getData();
    this.getRequestStatus();
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

    this.modalsearch.emit(this.input);
    this.close();
  }

  getRequestStatus() {
    this.statusData = [];
    if (this.input.channel == PriceOfferChannel.MarketPlace) {
      if (this.isShipper || this.isTachyonDealerOrHost) {
        this.statusData.push(
          { displayText: this.l('New'), value: 0 },
          { displayText: this.l('PriceSubmitted'), value: 1 },
          { displayText: this.l('Confirmed'), value: 2 },
          { displayText: this.l('Cancled'), value: 3 }
        );
      } else {
        this.statusData.push(
          { displayText: this.l('PriceSubmitted'), value: 1 },
          { displayText: this.l('Confirmed'), value: 2 },
          { displayText: this.l('Cancled'), value: 3 }
        );
      }
    } else if (this.input.channel == PriceOfferChannel.DirectRequest) {
      if (this.isShipper || this.isTachyonDealerOrHost) {
        this.statusData.push(
          { displayText: this.l('New'), value: 0 },
          { displayText: this.l('Response'), value: 1 },
          { displayText: this.l('Accepted'), value: 2 },
          { displayText: this.l('Rejected'), value: 3 },
          { displayText: this.l('DeclinedOfPricing'), value: 4 },
          { displayText: this.l('Pending'), value: 5 }
        );
      } else {
        this.statusData.push(
          { displayText: this.l('New'), value: 0 },
          { displayText: this.l('WaitingForResponse'), value: 1 },
          { displayText: this.l('Accepted'), value: 2 },
          { displayText: this.l('Rejected'), value: 3 },
          { displayText: this.l('DeclinedOfPricing'), value: 4 },
          { displayText: this.l('Pending'), value: 5 }
        );
      }
    } else if (this.input.channel == PriceOfferChannel.Offers) {
      this.statusData.push(
        { displayText: this.l('New'), value: 0 },
        { displayText: this.l('Accepted'), value: 1 },
        { displayText: this.l('Rejected'), value: 2 }
      );
      if (this.isTachyonDealerOrHost) {
        this.statusData.push(
          { displayText: this.l('AcceptedAndWaitingForCarrier'), value: 3 },
          { displayText: this.l('AcceptedAndWaitingForShipper'), value: 4 },
          { displayText: this.l('Pending'), value: 6 }
        );
      } else if (this.isCarrier) {
        this.statusData.push({ displayText: this.l('Pending'), value: 6 });
      }
    } else if (!this.input.channel) {
      if (this.isShipper || this.isTachyonDealerOrHost) {
        this.statusData.push(
          { displayText: this.l('New'), value: 0 },
          { displayText: this.l('Confiremed'), value: 1 },
          { displayText: this.l('NeedsAction'), value: 2 },
          { displayText: this.l('Expired'), value: 3 },
          { displayText: this.l('Cancled'), value: 4 },
          { displayText: this.l('Completed'), value: 5 }
        );
        if (this.isTachyonDealerOrHost) {
          this.statusData.push({ displayText: this.l('AcceptedAndWaitingCarrier'), value: 6 });
        }
      } else {
        this.statusData.push(
          { displayText: this.l('New'), value: 0 },
          { displayText: this.l('Confiremed'), value: 1 },
          { displayText: this.l('Cancled'), value: 4 },
          { displayText: this.l('Completed'), value: 5 }
        );
      }
    }
  }
}
