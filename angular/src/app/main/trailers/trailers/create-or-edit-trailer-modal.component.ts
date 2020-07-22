import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  TrailersServiceProxy,
  CreateOrEditTrailerDto,
  TrailerTrailerStatusLookupTableDto,
  TrailerTrailerTypeLookupTableDto,
  TrailerPayloadMaxWeightLookupTableDto,
  TrailerTruckLookupTableDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

@Component({
  selector: 'createOrEditTrailerModal',
  templateUrl: './create-or-edit-trailer-modal.component.html',
})
export class CreateOrEditTrailerModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  trailer: CreateOrEditTrailerDto = new CreateOrEditTrailerDto();

  trailerStatusDisplayName = '';
  trailerTypeDisplayName = '';
  payloadMaxWeightDisplayName = '';
  truckPlateNumber = '';

  allTrailerStatuss: TrailerTrailerStatusLookupTableDto[];
  allTrailerTypes: TrailerTrailerTypeLookupTableDto[];
  allPayloadMaxWeights: TrailerPayloadMaxWeightLookupTableDto[];
  allTrucks: TrailerTruckLookupTableDto[];

  constructor(injector: Injector, private _trailersServiceProxy: TrailersServiceProxy) {
    super(injector);
  }

  show(trailerId?: number): void {
    if (!trailerId) {
      this.trailer = new CreateOrEditTrailerDto();
      this.trailer.id = trailerId;
      this.trailerStatusDisplayName = '';
      this.trailerTypeDisplayName = '';
      this.payloadMaxWeightDisplayName = '';
      this.truckPlateNumber = '';

      this.active = true;
      this.modal.show();
    } else {
      this._trailersServiceProxy.getTrailerForEdit(trailerId).subscribe((result) => {
        this.trailer = result.trailer;

        this.trailerStatusDisplayName = result.trailerStatusDisplayName;
        this.trailerTypeDisplayName = result.trailerTypeDisplayName;
        this.payloadMaxWeightDisplayName = result.payloadMaxWeightDisplayName;
        this.truckPlateNumber = result.truckPlateNumber;

        this.active = true;
        this.modal.show();
      });
    }
    this._trailersServiceProxy.getAllTrailerStatusForTableDropdown().subscribe((result) => {
      this.allTrailerStatuss = result;
    });
    this._trailersServiceProxy.getAllTrailerTypeForTableDropdown().subscribe((result) => {
      this.allTrailerTypes = result;
    });
    this._trailersServiceProxy.getAllPayloadMaxWeightForTableDropdown().subscribe((result) => {
      this.allPayloadMaxWeights = result;
    });
    this._trailersServiceProxy.getAllTruckForTableDropdown().subscribe((result) => {
      this.allTrucks = result;
    });
  }

  save(): void {
    this.saving = true;

    this._trailersServiceProxy
      .createOrEdit(this.trailer)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }
}
