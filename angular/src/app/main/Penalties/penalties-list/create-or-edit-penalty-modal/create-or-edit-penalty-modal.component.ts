import { Component, EventEmitter, Injector, OnInit, Output, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { CreateOrEditPenaltyDto, GetAllCompanyForDropDownDto, PenaltiesServiceProxy } from '@shared/service-proxies/service-proxies';
import { isNotNullOrUndefined } from 'codelyzer/util/isNotNullOrUndefined';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'createOrEditPenaltyModal',
  templateUrl: './create-or-edit-penalty-modal.component.html',
  styleUrls: ['./create-or-edit-penalty-modal.component.css'],
})
export class CreateOrEditPenaltyModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  allCompanies: GetAllCompanyForDropDownDto[];
  active = false;
  saving: boolean;
  form: CreateOrEditPenaltyDto;

  constructor(inject: Injector, private _PenaltiesServiceProxy: PenaltiesServiceProxy) {
    super(inject);
  }
  ngOnInit(): void {
    this.loadDropDowns();
  }
  loadDropDowns() {
    this.getAllCompaniesForDropDown();
  }
  getAllCompaniesForDropDown() {
    this._PenaltiesServiceProxy.getAllCompanyForDropDown().subscribe((res) => {
      this.allCompanies = res;
    });
  }
  save() {
    this.saving = true;
    this._PenaltiesServiceProxy
      .createOrEdit(this.form)
      .pipe(
        finalize(() => {
          this.saving = false;
          this.notify.success('SavedSuccessfully');
          this.modalSave.emit();
        })
      )
      .subscribe(() => {
        this.close();
      });
  }
  show(id?: number) {
    this.active = true;
    this.form = new CreateOrEditPenaltyDto();
    if (isNotNullOrUndefined(id)) {
      //edit
      this._PenaltiesServiceProxy.getPenaltyForEditDto(id).subscribe((res) => {
        this.form = res;
      });
    }
    this.modal.show();
    console.log(this.form);
  }
  close() {
    this.active = false;
    this.modal.hide();
  }
}
