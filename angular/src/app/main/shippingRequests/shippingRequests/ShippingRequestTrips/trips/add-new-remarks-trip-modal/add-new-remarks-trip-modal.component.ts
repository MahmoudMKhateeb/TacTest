import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { RemarksInputDto, ShippingRequestsTripServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-add-new-remarks-trip-modal',
  templateUrl: './add-new-remarks-trip-modal.component.html',
  styleUrls: ['./add-new-remarks-trip-modal.component.css'],
})
export class AddNewRemarksTripModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  saving = false;
  active = false;
  remarksInput = new RemarksInputDto();

  constructor(injector: Injector, private _shippingRequestTripsService: ShippingRequestsTripServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {}
  show(id: number): void {
    this._shippingRequestTripsService.getRemarks(id).subscribe((result) => {
      this.remarksInput = result;
    });
    this.active = true;
    this.remarksInput.id = id;
    this.modal.show();
  }
  close(): void {
    this.modal.hide();
    this.remarksInput = new RemarksInputDto();
    this.active = false;
  }
  save(): void {
    this.saving = true;
    this._shippingRequestTripsService
      .addRemarks(this.remarksInput)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
      });
  }
}
