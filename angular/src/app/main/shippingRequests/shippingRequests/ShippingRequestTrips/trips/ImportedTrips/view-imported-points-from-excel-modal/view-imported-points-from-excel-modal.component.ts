import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ImportRoutePointDto, ImportShipmentFromExcelServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-view-imported-points-from-excel-modal',
  templateUrl: './view-imported-points-from-excel-modal.component.html',
  styleUrls: ['./view-imported-points-from-excel-modal.component.css'],
})
export class ViewImportedPointsFromExcelModalComponent extends AppComponentBase {
  @Input() ImportedPointsList: ImportRoutePointDto[];
  active = false;
  saving = false;
  @ViewChild('ViewImportedPointsModal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

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
    this.ImportShipmentFromExcelService.createPointsFromDto(this.ImportedPointsList)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }
}
