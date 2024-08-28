import { Component, OnInit, Injector, ViewChild, Output, EventEmitter } from '@angular/core';
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
  ActorTypesEnum,
} from '@shared/service-proxies/service-proxies';
import { EnumToArrayPipe } from '@shared/common/pipes/enum-to-array.pipe';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  templateUrl: './tacking-search-model.component.html',
  // styleUrls: ['/assets/custom/css/model.scss'],
  selector: 'tacking-search-model',
  animations: [appModuleAnimation()],
  providers: [EnumToArrayPipe],
})
export class TrackinSearchModelComponent extends AppComponentBase implements OnInit {
  @Output() modalsearch: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  isLoad = false;
  active = false;
  saving = false;
  input: TrackingSearchInput = new TrackingSearchInput();
  direction: string;
  creationDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  pickupDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  deliveryDateRange: Date[] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
  creationDateRangeActive = false;
  pickupDateRangeActive = false;
  deliveryDateRangeActive = false;
  searchList: GetShippingRequestSearchListDto;
  cites: ComboboxItemDto[] = [];
  truckTypes: ComboboxItemDto[] = [];
  transportTypes: ComboboxItemDto[] = [];
  goodsCategories: ComboboxItemDto[] = [];
  packingTypes: ComboboxItemDto[] = [];
  truckCapacities: ComboboxItemDto[] = [];
  statusData: { displayText: string; value: number | string }[] = [];
  routeTypes: any[] = [];
  AllActorTypes = this.enumToArray.transform(ActorTypesEnum);

  constructor(injector: Injector, private _currentSrv: PriceOfferServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }
  ngOnInit(): void {
    this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType).map((item) => {
      item.value = this.l(item.value);
      return item;
    });
    const obj = {
      value: this.l('All'),
      key: '',
    };
    this.routeTypes.unshift(obj);
    this.getRoutTypes();
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
      this.addAllToTopOfTheArray(this.cites);
      this.truckTypes = result.trucksTypes.map((x) => {
        let item: ComboboxItemDto = new ComboboxItemDto();
        item.displayText = isNotNullOrUndefined(x.translatedDisplayName) ? x.translatedDisplayName : x.displayName;
        item.value = x.id.toString();
        return item;
      });
      this.addAllToTopOfTheArray(this.truckTypes);
      this.transportTypes = result.transportTypes.map((x) => {
        let item: ComboboxItemDto = new ComboboxItemDto();
        item.displayText = x.translatedDisplayName;
        item.value = x.id.toString();
        return item;
      });
      this.addAllToTopOfTheArray(this.transportTypes);
      this.truckCapacities = result.capacities.map((x) => {
        let item: ComboboxItemDto = new ComboboxItemDto();
        item.displayText = isNotNullOrUndefined(x.translatedDisplayName) ? x.translatedDisplayName : x.displayName;
        item.value = x.id.toString();
        return item;
      });
      this.addAllToTopOfTheArray(this.truckCapacities);
      this.goodsCategories = result.goodsCategories.map((x) => {
        let item: ComboboxItemDto = new ComboboxItemDto();
        item.displayText = x.displayName;
        item.value = x.id.toString();
        return item;
      });
      this.addAllToTopOfTheArray(this.goodsCategories);
      this.packingTypes = result.packingTypes.map((x) => {
        let item: ComboboxItemDto = new ComboboxItemDto();
        item.displayText = x.displayName;
        item.value = x.id.toString();
        return item;
      });
      this.addAllToTopOfTheArray(this.packingTypes);
    });
  }

  private addAllToTopOfTheArray(array: ComboboxItemDto[]) {
    array.unshift(
      ComboboxItemDto.fromJS({
        value: '',
        displayText: this.l('All'),
      })
    );
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

    if (this.deliveryDateRangeActive) {
      this.input.deliveryFromDate = moment(this.deliveryDateRange[0]);
      this.input.deliveryToDate = moment(this.deliveryDateRange[1]);
    } else {
      this.input.deliveryFromDate = null;
      this.input.deliveryToDate = null;
    }

    this.modalsearch.emit(this.input);
    this.close();
  }

  getRequestStatus() {
    this.statusData.push(
      { displayText: this.l('All'), value: '' },
      { displayText: this.l('New'), value: 0 },
      { displayText: this.l('InTransit'), value: 1 },
      { displayText: this.l('Cancled'), value: 2 },
      { displayText: this.l('Delivered'), value: 3 }
    );
  }

  private getRoutTypes() {
    this.routeTypes = this.enumToArray.transform(ShippingRequestRouteType).map((item) => {
      item.key = Number(item.key);
      return item;
    });
    this.routeTypes.unshift({
      key: '',
      value: this.l('All'),
    });
  }
}
