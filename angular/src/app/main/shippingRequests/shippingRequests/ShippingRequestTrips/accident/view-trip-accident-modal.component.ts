import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { Table } from 'primeng/table';

import {
  CreateOrEditShippingRequestTripAccidentResolveDto,
  ShippingRequestsTripListDto,
  ShippingRequestsTripServiceProxy,
  ShippingRequestTripAccidentListDto,
  ShippingRequestTripAccidentServiceProxy,
  ShippingRequestTripStatus,
  TripAccidentResolveType,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { Paginator } from '@node_modules/primeng/paginator';
import { finalize } from '@node_modules/rxjs/operators';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Component({
  templateUrl: './view-trip-accident-modal.component.html',
  selector: 'view-trip-accident-modal',
  styleUrls: ['./view-trip-accident-modal.component.scss'],
})
export class ViewTripAccidentModelComponent extends AppComponentBase implements OnInit {
  @ViewChild('dataTable', { static: true }) dataTable: Table;
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('paginator', { static: true }) paginator: Paginator;
  @Output('incidentResolved') tripChangedEvent: EventEmitter<any> = new EventEmitter<any>();
  incidentsDataSource: any;
  currentTrip: ShippingRequestsTripListDto = new ShippingRequestsTripListDto();
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
  selectedIncidents: any[];
  currentIncident;
  continueTripLoading: boolean;

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


  show(trip: ShippingRequestsTripListDto) {
    this.currentTrip = trip;
    this.intiDataSource(trip.id);
    this.active = true;
    this.modal.show();
  }

  private intiDataSource(tripId: number) {
    let self = this;
    this.incidentsDataSource = {};
    this.incidentsDataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        loadOptions.filter = [];
        (loadOptions.filter as any[]).push(['tripId', '=', tripId]);
        return self._ServiceProxy
          .getAll(JSON.stringify(loadOptions))
          .toPromise()
          .then((response) => {
            return {
              data: response.data,
              totalCount: response.totalCount,
              summary: response.summary,
              groupCount: response.groupCount,
            };
          })
          .catch((error) => {
            throw new Error('Data Loading Error');
          });
      },
    });
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
    this.incidentsDataSource = {};
  }

  refreshTable() {
    this.intiDataSource(this.currentTrip.id);
  }



  approveResolve() {
    if (this.canApproveResolve()) {
      this._ServiceProxy.applyResolveChanges(this.currentIncident.resolveListDto.id).subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.refreshTable();
      });
    }
  }



  canApproveResolve(): boolean {
    if (
      isNotNullOrUndefined(this.currentIncident?.resolveListDto?.id) &&
      !this.currentIncident.resolveListDto.isAppliedResolve &&
      !this.currentIncident.isResolve
    ) {
      return (
        (this.isShipper && !this.currentIncident.resolveListDto.approvedByShipper) ||
        (this.isCarrier && !this.currentIncident.resolveListDto.approvedByCarrier)
      );
    }

    return false;
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

  fillCurrentIncident() {
    this.currentIncident = this.selectedIncidents[0];
  }

  ngOnInit(): void {
    this.selectedIncidents = [];
    this.continueTripLoading = false;
  }

  continueTrip() {
    this.continueTripLoading = true;
    this._ServiceProxy
      .continueTrip(this.currentIncident.id)
      .pipe(finalize(() => (this.continueTripLoading = false)))
      .subscribe(() => {
        this.notify.success(this.l('SavedSuccessfully'));
        this.refreshTable();
      });
  }
}
