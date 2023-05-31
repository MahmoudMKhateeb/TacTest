import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ImportGoodsDetailsDto, ImportShipmentFromExcelServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { DxPopoverComponent } from '@node_modules/devextreme-angular/ui/popover';

@Component({
  selector: 'app-view-imported-good-details-from-excel-modal',
  templateUrl: './view-imported-good-details-from-excel-modal.component.html',
  styleUrls: ['./view-imported-good-details-from-excel-modal.component.css'],
})
export class ViewImportedGoodDetailsFromExcelModalComponent extends AppComponentBase {
  @Input() ImportedGoodDetailsList: ImportGoodsDetailsDto[];
  active = false;
  saving = false;
  @ViewChild('ViewImportedGoodDetailsModal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('popOver', { static: false }) popOver: DxPopoverComponent;
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
    this.ImportShipmentFromExcelService.createGoodsDetailsFromDto(this.ImportedGoodDetailsList)
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
