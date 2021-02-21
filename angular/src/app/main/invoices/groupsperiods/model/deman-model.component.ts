import { Component, ViewChild, Injector, Output, EventEmitter, Input, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import { GroupPeriodServiceProxy, GroupPeriodDemandCreateInput } from '@shared/service-proxies/service-proxies';

const toBase64 = (file) =>
  new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = (error) => reject(error);
  });

@Component({
  selector: 'deman-model',
  templateUrl: './deman-model.component.html',
})
export class DemanModelComponent extends AppComponentBase {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  active = false;
  saving = false;
  File: GroupPeriodDemandCreateInput;
  DocumentBase64: string;
  constructor(injector: Injector, private _GroupService: GroupPeriodServiceProxy) {
    super(injector);
  }

  show(groupid: number): void {
    this.File = new GroupPeriodDemandCreateInput();
    this.File.id = groupid;
    this.active = true;
    this.modal.show();
  }

  save(): void {
    this.saving = true;
    console.log(this.File);
    this._GroupService
      .demand(this.File)
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
    this.File.documentBase64 = String(await toBase64(event.target.files[0]));
  }
}
