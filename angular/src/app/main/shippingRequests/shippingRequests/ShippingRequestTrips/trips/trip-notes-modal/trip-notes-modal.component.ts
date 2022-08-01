import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { NotesComponent } from '../../../notes/notes.component';

@Component({
  selector: 'app-trip-notes-modal',
  templateUrl: './trip-notes-modal.component.html',
  styleUrls: ['./trip-notes-modal.component.css'],
})
export class TripNotesModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('TripNotesModal', { static: true }) modal: ModalDirective;
  @ViewChild('NotesComponent') public NotesComponent: NotesComponent;

  @Input() tripId: number;
  @Input() type: string;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {}

  show() {
    this.NotesComponent.tripId = this.tripId;
    this.NotesComponent.type = this.type;
    this.NotesComponent.getData();
    this.modal.show();
  }

  close(): void {
    this.modal.hide();
  }
}
