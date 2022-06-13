import { Component, EventEmitter, Injector, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Table } from 'primeng/table';

import {
  CreateOrEditShippingRequestTripAccidentResolveDto,
  IShippingRequestTripAccidentListDto,
  ShippingRequestsTripListDto,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripAccidentListDto,
  ShippingRequestTripAccidentServiceProxy,
  ShippingRequestTripStatus,
  TripAccidentResolveType,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { LazyLoadEvent } from '@node_modules/primeng/api';
import { Paginator } from '@node_modules/primeng/paginator';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';
import { finalize } from '@node_modules/rxjs/operators';

@Component({
  templateUrl: './view-trip-accident-modal.component.html',
  selector: 'view-trip-accident-modal',
  styleUrls: ['./view-trip-accident-modal.component.scss'],
})
export class ViewTripAccidentModelComponent extends AppComponentBase {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Output('incidentResolved') tripChangedEvent: EventEmitter<any> = new EventEmitter<any>();
  Accident: IShippingRequestTripAccidentListDto[] = [];
  Trip: ShippingRequestsTripListDto = new ShippingRequestsTripListDto();
  Trsss: TripAccidentResolveType;
  active: boolean = false;
  IsStartSearch = false;
  allReasons: any;
  CancelationStatus: string =
    this.feature.isEnabled('App.Shipper') || this.feature.isEnabled('App.Carrier') ? this.l('Cancel') : this.l('ApproveCancel');
  changeDriver;
  changeTruck;
  changeDriverAndTruck;
  cancel;
  saving = false;
  tripHasAnyAccidentResolveNotApplied = false;
  tripHasAnyAccidentWithoutResolve = false;
  resolveNotAppliedAtLeastOnce = false;
  readonly defaultRecordsCountPerPage = 5;

  constructor(
    injector: Injector,
    private _ServiceProxy: ShippingRequestTripAccidentServiceProxy,
    private _tripServ: ShippingRequestsTripServiceProxy,
    private _fileDownloadService: FileDownloadService
  ) {
    super(injector);
    this.changeDriver = TripAccidentResolveType.ChangeDriver;
    this.changeTruck = TripAccidentResolveType.ChangeTruck;
    this.changeDriverAndTruck = TripAccidentResolveType.ChangeDriverAndTruck;
    this.cancel = TripAccidentResolveType.CancelTrip;
  }

  getAll(trip: ShippingRequestsTripListDto): void {
    this.primengTableHelper.showLoadingIndicator();
    this.Trip = trip;

    const defaultSorting = 'CreationTime desc';

    this._ServiceProxy.getAll(undefined, this.Trip.id, undefined, defaultSorting, this.defaultRecordsCountPerPage, undefined).subscribe((result) => {
      this.IsStartSearch = true;
      this.primengTableHelper.totalRecordsCount = result.totalCount;
      this.primengTableHelper.records = result.items;
      this.checkTripAccidentsHasAnyUnAppliedResolve(result.items);
      this.primengTableHelper.hideLoadingIndicator();
      this.active = true;
      this.modal.show();
    });
  }
  canShowCancelButton(): boolean {
    if (this.Trip.status == ShippingRequestTripStatus.Delivered) return false;
    else if (this.Trip.status == ShippingRequestTripStatus.Canceled) return false;
    else if (this.feature.isEnabled('App.Shipper')) {
      return !this.Trip.isApproveCancledByShipper;
    } else if (this.feature.isEnabled('App.Carrier')) {
      return !this.Trip.isApproveCancledByCarrier;
    } else return true;
  }
  downloadDocument(id: number): void {
    this._ServiceProxy.getFile(id).subscribe((result) => {
      this._fileDownloadService.downloadTempFile(result);
    });
  }
  close(): void {
    this.modal.hide();
    this.active = false;
    if (!this.tripHasAnyAccidentResolveNotApplied && this.resolveNotAppliedAtLeastOnce) {
      // that's mean trip has accident not resolve in past and solved now
      // so we need to refresh tripsForViewShippingRequest component
      this.tripChangedEvent.emit();
    }
  }

  refreshTable() {
    this.paginator.changePage(this.paginator.getPage());
  }

  resolveWithCancelTrip(accident: ShippingRequestTripAccidentListDto): void {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        let dto = new CreateOrEditShippingRequestTripAccidentResolveDto();
        dto.accidentId = accident.id;
        if (accident.resolveListDto.id) {
          dto.id = accident.resolveListDto.id;
        }

        dto.resolveType = TripAccidentResolveType.CancelTrip;
        this._ServiceProxy.createOrEditResolve(dto).subscribe(() => {
          abp.notify.success(this.l('SavedSuccessfully'));
          this.refreshTable();
        });
      }
    });
  }

  approveResolve(accident: ShippingRequestTripAccidentListDto) {
    if (accident.resolveListDto.id) {
      this._ServiceProxy.applyResolveChanges(accident.resolveListDto.id).subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.refreshTable();
      });
    }
  }

  canViewResolve(accident: ShippingRequestTripAccidentListDto): boolean {
    return (
      accident.resolveListDto.resolveType === this.changeDriver ||
      accident.resolveListDto.resolveType === this.changeTruck ||
      accident.resolveListDto.resolveType === this.changeDriverAndTruck
    );
  }

  canResolveAccident(accident: ShippingRequestTripAccidentListDto): boolean {
    // note: authorization not added here and this method not created for like this use
    return !accident.isResolve || !accident.resolveListDto.id || !accident.resolveListDto.isAppliedResolve;
  }

  getAccidents(event?: LazyLoadEvent) {
    if (this.primengTableHelper.shouldResetPaging(event)) {
      this.paginator.changePage(0);
      return;
    }

    this.primengTableHelper.showLoadingIndicator();

    this._ServiceProxy
      .getAll(
        undefined,
        this.Trip.id,
        undefined,
        this.primengTableHelper.getSorting(this.dataTable),
        this.primengTableHelper.getMaxResultCount(this.paginator, event),
        this.primengTableHelper.getSkipCount(this.paginator, event)
      )
      .subscribe((result) => {
        this.primengTableHelper.totalRecordsCount = result.totalCount;
        this.primengTableHelper.records = result.items;
        this.checkTripAccidentsHasAnyUnAppliedResolve(result.items);
        this.primengTableHelper.hideLoadingIndicator();
      });
  }

  resolveWithNoAction(accident: ShippingRequestTripAccidentListDto) {
    this.message.confirm('', this.l('AreYouSure'), (isConfirmed) => {
      if (isConfirmed) {
        let dto = new CreateOrEditShippingRequestTripAccidentResolveDto();
        dto.accidentId = accident.id;
        if (accident.resolveListDto.id) {
          dto.id = accident.resolveListDto.id;
        }

        dto.resolveType = TripAccidentResolveType.NoActionNeeded;
        this._ServiceProxy.createOrEditResolve(dto).subscribe(() => {
          abp.notify.success(this.l('SavedSuccessfully'));
          this.refreshTable();
        });
      }
    });
  }

  checkTripAccidentsHasAnyUnAppliedResolve(accidents: ShippingRequestTripAccidentListDto[]) {
    this.tripHasAnyAccidentWithoutResolve = accidents.some((x) => !x.resolveListDto.id);

    let hasNotAppliedResolve = accidents.some((x) => x.resolveListDto.id && !x.resolveListDto.isAppliedResolve);
    if (hasNotAppliedResolve) {
      this.tripHasAnyAccidentResolveNotApplied = true;
      this.resolveNotAppliedAtLeastOnce = true;
      return;
    }
    this.tripHasAnyAccidentResolveNotApplied = false;
  }

  enforceChange(tripId: number) {
    this.saving = true;
    this._ServiceProxy
      .enforceApplyChanges(tripId)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        abp.notify.success(this.l('SavedSuccessfully'));
        this.refreshTable();
      });
  }

  approveChange(tripId: number) {} // todo
}
