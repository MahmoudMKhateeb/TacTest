import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { TermAndConditionsServiceProxy, CreateOrEditTermAndConditionDto, SelectItemDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import { DomSanitizer } from '@angular/platform-browser';
declare var Quill: any;

@Component({
  selector: 'createOrEditTermAndConditionModal',
  templateUrl: './create-or-edit-termAndCondition-modal.component.html',
})
export class CreateOrEditTermAndConditionModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  editionsList: SelectItemDto[] = [];
  termAndCondition: CreateOrEditTermAndConditionDto = new CreateOrEditTermAndConditionDto();

  constructor(injector: Injector, protected sanitizer: DomSanitizer, private _termAndConditionsServiceProxy: TermAndConditionsServiceProxy) {
    super(injector);
    var fontSizeStyle = Quill.import('attributors/style/size');
    fontSizeStyle.whitelist = ['8px', '9px', '10px', '12px', '14px', '16px', '18px', '20px', '22px', '24px', '26px'];
    Quill.register(fontSizeStyle, true);
  }

  show(termAndConditionId?: number): void {
    if (!termAndConditionId) {
      this.termAndCondition = new CreateOrEditTermAndConditionDto();
      this.termAndCondition.id = termAndConditionId;
      this._termAndConditionsServiceProxy.getDocumentEntitiesForTableDropdown().subscribe((result) => {
        this.editionsList = result;
        this.termAndCondition.editionId = 1;
      });
      this.active = true;
      this.modal.show();
    } else {
      this._termAndConditionsServiceProxy.getTermAndConditionForEdit(termAndConditionId).subscribe((result) => {
        this.termAndCondition = result.termAndCondition;
        this._termAndConditionsServiceProxy.getDocumentEntitiesForTableDropdown().subscribe((result) => {
          this.editionsList = result;
          this.active = true;
          this.modal.show();
        });
      });
    }
  }

  save(): void {
    this.saving = true;
    if (this.termAndCondition.content == null) {
      this.notify.error('please make sure you compleate all needed data!');
    }
    this._termAndConditionsServiceProxy
      .createOrEdit(this.termAndCondition)
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

  close(): void {
    this.active = false;
    this.modal.hide();
  }
  getEditionsList() {
    this._termAndConditionsServiceProxy.getDocumentEntitiesForTableDropdown().subscribe((result) => {
      this.editionsList = result;
    });
  }
  numberOnly(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }
}
