import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { inject } from '@angular/core/testing';
import { AppComponentBase } from '@shared/common/app-component-base';
import { InvoiceNoteServiceProxy, NoteInputDto } from '@shared/service-proxies/service-proxies';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-note-modal',
  templateUrl: './note-modal.component.html',
})
export class NoteModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  saving = false;
  active = false;
  noteInput = new NoteInputDto();
  constructor(inject: Injector, private _invoiceNoteServiceProxy: InvoiceNoteServiceProxy) {
    super(inject);
  }
  ngOnInit(): void {}
  show(id: number): void {
    this._invoiceNoteServiceProxy.getNote(id).subscribe((result) => {
      this.noteInput = result;
    });
    this.active = true;
    this.noteInput.id = id;
    this.modal.show();
  }

  close(): void {
    this.modal.hide();
    this.noteInput = new NoteInputDto();
    this.active = false;
  }
  save(): void {
    this.saving = true;
    this._invoiceNoteServiceProxy
      .addNote(this.noteInput)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
      });
  }
}
