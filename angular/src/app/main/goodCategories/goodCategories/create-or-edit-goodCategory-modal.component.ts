import { Component, ViewChild, Injector, Output, EventEmitter } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  GoodCategoriesServiceProxy,
  CreateOrEditGoodCategoryDto,
  GetAllGoodsCategoriesForDropDownOutput,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';

@Component({
  selector: 'createOrEditGoodCategoryModal',
  templateUrl: './create-or-edit-goodCategory-modal.component.html',
})
export class CreateOrEditGoodCategoryModalComponent extends AppComponentBase {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving = false;
  goodCategory: CreateOrEditGoodCategoryDto = new CreateOrEditGoodCategoryDto();
  AllGoodsCategoriesForDropDown: GetAllGoodsCategoriesForDropDownOutput[];

  constructor(injector: Injector, private _goodCategoriesServiceProxy: GoodCategoriesServiceProxy) {
    super(injector);
  }

  AllCategoriesForFatherDropDown() {
    this._goodCategoriesServiceProxy.getAllGoodsCategoriesForDropDown().subscribe((result) => {
      this.AllGoodsCategoriesForDropDown = result;
      console.log('Got them');
    });
  }

  show(goodCategoryId?: number): void {
    this.AllCategoriesForFatherDropDown(); //in case of Create/Edit Get AllCategoriesForFatherDropDown
    if (!goodCategoryId) {
      console.log('Create'); //TODO: Remove this line
      this.goodCategory = new CreateOrEditGoodCategoryDto();
      this.goodCategory.id = goodCategoryId;
      this.active = true;
      this.modal.show();
    } else {
      console.log('edit'); //TODO: Remove this line
      this._goodCategoriesServiceProxy.getGoodCategoryForEdit(goodCategoryId).subscribe((result) => {
        this.goodCategory = result.goodCategory;

        this.active = true;
        this.modal.show();
        console.log(this.goodCategory);
      });
      console.log(this.goodCategory);
    }
  }

  save(): void {
    this.saving = true;
    console.log(this.goodCategory); //TODO: Remove this line

    this._goodCategoriesServiceProxy
      .createOrEdit(this.goodCategory)
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
}
