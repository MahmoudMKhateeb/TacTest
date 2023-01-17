import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ImportTripDto, ImportShipmentFromExcelServiceProxy, ShippingRequestRouteType } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { DxPopoverComponent } from '@node_modules/devextreme-angular';

@Component({
  selector: 'app-view-imported-trips-from-excel-modal',
  templateUrl: './view-imported-trips-from-excel-modal.component.html',
  styleUrls: ['./view-imported-trips-from-excel-modal.component.css'],
})
export class ViewImportedTripsFromExcelModalComponent extends AppComponentBase {
  @Input() ImportedTripsList: ImportTripDto[];
  @Input() isSingleDdrop: boolean;
  @Input() isDedicatedRequest: boolean;
  active = false;
  saving = false;
  @ViewChild('ViewImportedTripsModal', { static: false }) modal: ModalDirective;
  @ViewChild('popOver', { static: false }) popOver: DxPopoverComponent;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  popoverTarget: any;
  popOverText: any;

  constructor(injector: Injector, private ImportShipmentFromExcelService: ImportShipmentFromExcelServiceProxy) {
    super(injector);
  }

  close(): void {
    this.modal.hide();
  }

  show() {
    this.modal.show();
  }

  save() {
    this.saving = true;
    this.ImportShipmentFromExcelService.createShipmentsFromDto(this.ImportedTripsList)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  onCellHoverChanged(e) {
    if (e.rowType === 'data' && e.column.dataField === 'exception' && e?.text?.length > 0 && e.eventType === 'mouseover') {
      this.popOverText = e.text;
      this.popoverTarget = e.cellElement;
      this.popOver.instance.show();
    }
    if (e.rowType === 'data' && e.eventType === 'mouseout') {
      this.popOver.instance.hide();
    }
  }
}
