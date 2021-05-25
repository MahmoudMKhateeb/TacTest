import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import {
  GoodCategoriesServiceProxy,
  CreateOrEditGoodCategoryDto,
  GetAllGoodsCategoriesForDropDownOutput,
  GoodCategoryTranslationDto,
} from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';

@Component({
  selector: 'createOrEditGoodCategoryModal',
  templateUrl: './create-or-edit-goodCategory-modal.component.html',
})
export class CreateOrEditGoodCategoryModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();
  active = false;
  saving = false;
  languages: abp.localization.ILanguageInfo[];
  Translations: GoodCategoryTranslationDto[];
  goodCategory: CreateOrEditGoodCategoryDto = new CreateOrEditGoodCategoryDto();
  AllGoodsCategoriesForDropDown: GetAllGoodsCategoriesForDropDownOutput[];

  constructor(injector: Injector, private _goodCategoriesServiceProxy: GoodCategoriesServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.languages = _.filter(this.localization.languages, (l) => l.isDisabled === false);
  }
  AllCategoriesForFatherDropDown() {
    this._goodCategoriesServiceProxy.getAllGoodsCategoriesForDropDown().subscribe((result) => {
      this.AllGoodsCategoriesForDropDown = result;
      console.log('Got them');
    });
  }

  show(goodCategoryId?: number): void {
    this.Translations = [];
    this.AllCategoriesForFatherDropDown(); //in case of Create/Edit Get AllCategoriesForFatherDropDown
    if (!goodCategoryId) {
      //console.log('Create'); //TODO: Remove this line
      this.goodCategory = new CreateOrEditGoodCategoryDto();
      this.goodCategory.id = goodCategoryId;
      this.active = true;
      this.PopulateTranslations([]);
      this.modal.show();
    } else {
      // console.log('edit'); //TODO: Remove this line
      this._goodCategoriesServiceProxy.getGoodCategoryForEdit(goodCategoryId).subscribe((result) => {
        this.goodCategory = result.goodCategory;
        this.PopulateTranslations(this.goodCategory.translations);
        this.active = true;
        this.modal.show();
        console.log(this.goodCategory);
      });
      console.log(this.goodCategory);
    }
  }

  private PopulateTranslations(Translations: GoodCategoryTranslationDto[]) {
    this.languages.forEach((r) => {
      let translation = new GoodCategoryTranslationDto();
      translation.languageDisplayName = r.displayName;
      translation.language = r.name;
      translation.icon = r.icon;
      translation.displayName = _.find(Translations, (t) => t.language == r.name)?.displayName;
      this.Translations.push(translation);
    });
  }
  save(): void {
    this.goodCategory.translations = this.Translations;
    this.saving = true;
    //console.log(this.goodCategory); //TODO: Remove this line

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
