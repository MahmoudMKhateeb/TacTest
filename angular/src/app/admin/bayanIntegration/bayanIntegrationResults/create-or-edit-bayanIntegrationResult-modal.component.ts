import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { BayanIntegrationResultsServiceProxy, CreateOrEditBayanIntegrationResultDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';

import { BayanIntegrationResultShippingRequestTripLookupTableModalComponent } from './bayanIntegrationResult-shippingRequestTrip-lookup-table-modal.component';

@Component({
  selector: 'createOrEditBayanIntegrationResultModal',
  templateUrl: './create-or-edit-bayanIntegrationResult-modal.component.html',
})
export class CreateOrEditBayanIntegrationResultModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @ViewChild('bayanIntegrationResultShippingRequestTripLookupTableModal', { static: true })
  bayanIntegrationResultShippingRequestTripLookupTableModal: BayanIntegrationResultShippingRequestTripLookupTableModalComponent;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;

  bayanIntegrationResult: CreateOrEditBayanIntegrationResultDto = new CreateOrEditBayanIntegrationResultDto();

  shippingRequestTripContainerNumber = '';

  constructor(injector: Injector, private _bayanIntegrationResultsServiceProxy: BayanIntegrationResultsServiceProxy) {
    super(injector);
  }

  show(bayanIntegrationResultId?: number): void {
    if (!bayanIntegrationResultId) {
      this.bayanIntegrationResult = new CreateOrEditBayanIntegrationResultDto();
      this.bayanIntegrationResult.id = bayanIntegrationResultId;
      this.shippingRequestTripContainerNumber = '';

      this.active = true;
      this.modal.show();
    } else {
      this._bayanIntegrationResultsServiceProxy.getBayanIntegrationResultForEdit(bayanIntegrationResultId).subscribe((result) => {
        this.bayanIntegrationResult = result.bayanIntegrationResult;

        this.shippingRequestTripContainerNumber = result.shippingRequestTripContainerNumber;

        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this._bayanIntegrationResultsServiceProxy
      .createOrEdit(this.bayanIntegrationResult)
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

  openSelectShippingRequestTripModal() {
    this.bayanIntegrationResultShippingRequestTripLookupTableModal.id = this.bayanIntegrationResult.shippingRequestTripId;
    this.bayanIntegrationResultShippingRequestTripLookupTableModal.displayName = this.shippingRequestTripContainerNumber;
    this.bayanIntegrationResultShippingRequestTripLookupTableModal.show();
  }

  setShippingRequestTripIdNull() {
    this.bayanIntegrationResult.shippingRequestTripId = null;
    this.shippingRequestTripContainerNumber = '';
  }

  getNewShippingRequestTripId() {
    this.bayanIntegrationResult.shippingRequestTripId = this.bayanIntegrationResultShippingRequestTripLookupTableModal.id;
    this.shippingRequestTripContainerNumber = this.bayanIntegrationResultShippingRequestTripLookupTableModal.displayName;
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  ngOnInit(): void {}
}
