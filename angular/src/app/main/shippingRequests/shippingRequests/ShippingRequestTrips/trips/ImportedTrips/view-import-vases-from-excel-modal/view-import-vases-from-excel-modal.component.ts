import { Component, EventEmitter, Injector, Input, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { ImportShipmentFromExcelServiceProxy, ImportTripVasesDto, ImportTripVasesFromExcelInput } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-view-import-vases-from-excel-modal',
  templateUrl: './view-import-vases-from-excel-modal.component.html',
  styleUrls: ['./view-import-vases-from-excel-modal.component.css'],
})
export class ViewImportVasesFromExcelModalComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @Input() ImportedVasesList: ImportTripVasesDto[];

  active = false;
  saving = false;
  @ViewChild('ViewImportedVasesModal', { static: false }) modal: ModalDirective;

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
    this.ImportShipmentFromExcelService.createTripVasesFromDto(this.ImportedVasesList)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }
}
