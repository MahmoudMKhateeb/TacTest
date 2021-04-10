import { Component, ViewChild, Injector, Output, EventEmitter, Input, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import { GroupPeriodServiceProxy, GroupPeriodClaimCreateInput } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'deman-model',
  templateUrl: './deman-model.component.html',
})
export class DemanModelComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  active = false;
  saving = false;
  File: GroupPeriodClaimCreateInput;
  constructor(injector: Injector, private _GroupService: GroupPeriodServiceProxy) {
    super(injector);
  }

  show(groupid: number): void {
    this.File = new GroupPeriodClaimCreateInput();
    this.File.id = groupid;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
    console.log(this.File);
    this._GroupService
      .claim(this.File)
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

  async fileChangeEvent(event: any): Promise<void> {
    let file = event.target.files[0];
    this.File.documentBase64 = String(await this.toBase64(file));
    this.File.documentContentType = file.type;
    this.File.documentName = file.name;
  }
}
