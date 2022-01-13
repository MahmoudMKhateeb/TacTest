import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ShippingRequestTripAccidentServiceProxy,
  CreateOrEditShippingRequestTripAccidentDto,
  CommonLookupServiceProxy,
  CreateOrEditShippingRequestTripDto,
  ShippingRequestsTripListDto,
  ShippingRequestAccidentReasonLookupDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'trip-accident-modal',
  templateUrl: './create-or-edit-trip-accident-modal.component.html',
})
export class CreateOrEditTripAccidentModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() Trip: ShippingRequestsTripListDto;
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  accident: CreateOrEditShippingRequestTripAccidentDto;
  reasons: ShippingRequestAccidentReasonLookupDto[];
  active: boolean = false;
  saving: boolean = false;
  reasonId: any = '';
  Specifiedtime: Date = new Date();
  private tripId: number;
  constructor(injector: Injector, private _Service: ShippingRequestTripAccidentServiceProxy, private _CommonSrv: CommonLookupServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this._CommonSrv.getAccidentReason().subscribe((result) => {
      this.reasons = result;
    });
  }

  public show(tripid: number, accidentId: number | null): void {
    this.tripId = tripid;
    if (accidentId == null) {
      this.accident = new CreateOrEditShippingRequestTripAccidentDto();
      this.accident.tripId = tripid;
      this.reasonId = null;
      this.active = true;
      this.modal.show();
    } else {
      this._Service.getForEdit(accidentId).subscribe((result) => {
        this.accident = result;
        this.reasonId = this.accident.reasoneId ? this.accident.reasoneId : '0';
        this.active = true;
        this.modal.show();
      });
    }
  }

  save(): void {
    this.saving = true;

    this.accident.reasoneId = this.reasonId == 0 ? null : this.reasonId;
    this._Service
      .createOrEdit(this.accident)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(this.Trip);
        abp.event.trigger('TripReportedAccident', this.tripId);
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }

  async fileChangeEvent(event: any): Promise<void> {
    let file = event.target.files[0];
    this.accident.documentBase64 = String(await this.toBase64(file));
    this.accident.documentContentType = file.type;
    this.accident.documentName = file.name;
  }

  onChangeReason(reasonId: number | null): void {}
}
