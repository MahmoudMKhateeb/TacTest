import { Component, EventEmitter, Injector, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ImportTripDto, ImportShipmentFromExcelServiceProxy } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-view-imported-trips-from-excel-modal',
  templateUrl: './view-imported-trips-from-excel-modal.component.html',
  styleUrls: ['./view-imported-trips-from-excel-modal.component.css'],
})
export class ViewImportedTripsFromExcelModalComponent extends AppComponentBase implements OnInit, OnDestroy {
  @Input() ImportedTripsList: ImportTripDto[];
  active = false;
  saving = false;
  @ViewChild('ViewImportedTripsModal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  constructor(injector: Injector, private ImportShipmentFromExcelService: ImportShipmentFromExcelServiceProxy) {
    super(injector);
  }
  ngOnDestroy(): void {
    throw new Error('Method not implemented.');
  }

  ngOnInit(): void {
    console.log('ImportedTripsList ', this.ImportedTripsList);
  }

  close(): void {
    this.modal.hide();
  }

  show() {
    this.modal.show();
  }

  save() {
    this.ImportShipmentFromExcelService.createShipmentsFromDto(this.ImportedTripsList)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(null);
      });
  }

  checkIfExceptions() {
    return this.ImportedTripsList.some((r) => r.exception);
  }
}
