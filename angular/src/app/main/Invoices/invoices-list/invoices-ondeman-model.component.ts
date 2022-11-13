import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import { InvoiceServiceProxy, CommonLookupServiceProxy, ISelectItemDto, SelectItemDto } from '@shared/service-proxies/service-proxies';
@Component({
  selector: 'invoices-ondeman-model',
  templateUrl: './invoices-ondeman-model.component.html',
})
export class InvoiceDemandModelComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;

  active = false;
  saving = false;
  Tenant: ISelectItemDto;
  Tenants: ISelectItemDto[];
  Waybills: SelectItemDto[];
  SelectedWaybills: SelectItemDto[];
  InvoiceTypeOptions: any[];
  invoiceTypeValue: number = 1;
  editions: string[];
  selectedEdition: string;

  constructor(injector: Injector, private _currentSrv: InvoiceServiceProxy, private _CommonServ: CommonLookupServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
      this.InvoiceTypeOptions = [
          {label: 'Invoice', value: '1'},
          {label: 'PenaltyInvoice', value: '2'},
      ];
      this.invoiceTypeValue = 1;
      this.editions = ['shipper', 'broker'];
  }

  show(): void {
    this.Tenant = undefined;
    this.Waybills = undefined;
    this.SelectedWaybills = undefined;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;

    if (this.Tenant?.id) {
    } else {
      this.Tenant = undefined;
    }
    if (!this.Tenant?.id) return;

    //check if normal invoice
    if (this.invoiceTypeValue == 1) {
      this._currentSrv
        .onDemand(parseInt(this.Tenant.id), this.SelectedWaybills)
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

    //if Penalty invoice
    else if (this.invoiceTypeValue == 2) {
      this._currentSrv
        .generatePenaltyInvoiceOnDemand(parseInt(this.Tenant.id), null)
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
  }

  close(): void {
    this.active = false;
    this.modal.hide();
  }

  search(event) {
    this._CommonServ.getAutoCompleteTenants(event.query, this.selectedEdition).subscribe((result) => {
      this.Tenants = result;
    });
  }

  LoadWaybills(event): void {
    this._currentSrv.getUnInvoicedWaybillsByTenant(parseInt(this.Tenant.id)).subscribe((res) => {
      this.Waybills = res;
    });
  }
}
