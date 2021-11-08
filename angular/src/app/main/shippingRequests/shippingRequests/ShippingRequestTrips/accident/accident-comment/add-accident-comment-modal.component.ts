import { Component, EventEmitter, Injector, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ModalDirective } from 'ngx-bootstrap/modal';
import {
  CreateOrEditShippingRequestTripAccidentCommentDto,
  CreateOrEditShippingRequestTripAccidentDto,
  ProfileServiceProxy,
  ShippingRequestTripAccidentCommentListDto,
  ShippingRequestTripAccidentCommentsServiceProxy,
} from '@shared/service-proxies/service-proxies';
import { FileDownloadService } from '@shared/utils/file-download.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-add-accident-comment-modal',
  templateUrl: './add-accident-comment-modal.component.html',
  styleUrls: ['./add-accident-comment-modal.component.css'],
})
export class AddAccidentCommentModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('AddTripAccidentCommentModal', { static: false }) public modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving: boolean = false;
  comments: CreateOrEditShippingRequestTripAccidentCommentDto;
  accidentComments: ShippingRequestTripAccidentCommentListDto[] = undefined;
  @Input() allReasons: any;
  constructor(
    injector: Injector,
    private _profileServiceProxy: ProfileServiceProxy,
    private _shippingRequestTripAccidentComment: ShippingRequestTripAccidentCommentsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {}

  getAll(accidentId: number) {
    this.primengTableHelper.showLoadingIndicator();

    this._shippingRequestTripAccidentComment.getAll(accidentId, undefined).subscribe((result) => {
      this.accidentComments = result.items;
      console.log('log ', this.accidentComments);
      this.comments.accidentId = accidentId;
      this.primengTableHelper.hideLoadingIndicator();
      this.active = true;
      this.modal.show();
    });
  }

  show(accidentId: number): void {
    this.comments = new CreateOrEditShippingRequestTripAccidentCommentDto();
    this.comments.accidentId = accidentId;
    this.getAll(accidentId);
    //this.scrollToDirectRequests("comments");
  }

  save(): void {
    this.saving = true;
    this._shippingRequestTripAccidentComment
      .createOrEdit(this.comments)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.getAll(this.comments.accidentId);
        this.modalSave.emit();
        this.comments.comment = ' ';
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
  getReasonDisplayname(id: number) {
    return this.allReasons ? this.allReasons.find((x) => x.id == id)?.displayName : 0;
  }
}
