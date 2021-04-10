import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, Input } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ShippingRequestTripAccidentServiceProxy,
  CreateOrEditShippingRequestTripAccidentResolveDto,
  ShippingRequestsTripListDto,
  IShippingRequestTripAccidentListDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'trip-resolve-modal',
  templateUrl: './create-or-edit-resolve-trip-accident-modal.component.html',
})
export class CreateOrEditTripResolveAccidentModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() Trip: ShippingRequestsTripListDto;
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  resolve: CreateOrEditShippingRequestTripAccidentResolveDto;
  active: boolean = false;
  saving: boolean = false;
  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: ShippingRequestTripAccidentServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}

  public show(accident: IShippingRequestTripAccidentListDto, id: number): void {
    if (id == 0) {
      this.resolve = new CreateOrEditShippingRequestTripAccidentResolveDto();
      this.resolve.accidentId = accident.id;
      this.active = true;
      this.modal.show();
    } else {
      //this._Service.getForEdit(accidentId).subscribe((result) => {
      //    this.accident = result;
      //    this.reasonId = this.accident.reasoneId ? this.accident.reasoneId : '0';
      //    this.active = true;
      //    this.modal.show();
      //});
    }
  }

  save(): void {
    this.saving = true;

    this._Service
      .createOrEditResolve(this.resolve)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(this.Trip);
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }

  async fileChangeEvent(event: any): Promise<void> {
    let file = event.target.files[0];
    this.resolve.documentBase64 = String(await this.toBase64(file));
    this.resolve.documentContentType = file.type;
    this.resolve.documentName = file.name;
  }

  onChangeReason(reasonId: number | null): void {}
}
