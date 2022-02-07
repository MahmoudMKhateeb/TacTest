import { Component, ElementRef, Injector, OnInit, ViewChild } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { RemarksInputDto, ShippingRequestsTripServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ModalDirective } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './CreateOrEditRemarks.component.html',
  animations: [appModuleAnimation()],
})
export class CreateOrEditRemarksComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('roundTripInput', { static: false }) roundTripInput: ElementRef;
  saving = false;
  active = false;
  remarksInput: RemarksInputDto;

  constructor(injector: Injector, private _shippingRequestTripsService: ShippingRequestsTripServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {}
  onShown(): void {
    this.roundTripInput.nativeElement.focus();
  }
  show(id: number): void {
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
        this.notify.info(this.l('CreatedSuccessfully'));
      });
  }
}
