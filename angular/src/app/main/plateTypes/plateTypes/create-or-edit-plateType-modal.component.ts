import { Component, ViewChild, Injector, Output, EventEmitter, OnInit } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { PlateTypesServiceProxy, CreateOrEditPlateTypeDto, PlateTypeTranslationDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as moment from 'moment';
import * as _ from 'lodash';

@Component({
  selector: 'createOrEditPlateTypeModal',
  templateUrl: './create-or-edit-plateType-modal.component.html',
})
export class CreateOrEditPlateTypeModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('createOrEditModal', { static: true }) modal: ModalDirective;

  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  active = false;
  saving = false;
  languages: abp.localization.ILanguageInfo[];
  plateType: CreateOrEditPlateTypeDto = new CreateOrEditPlateTypeDto();
  Translations: PlateTypeTranslationDto[];

  constructor(injector: Injector, private _plateTypesServiceProxy: PlateTypesServiceProxy) {
    super(injector);
  }

  ngOnInit(): void {
    this.languages = _.filter(this.localization.languages, (l) => l.isDisabled === false);
  }

  show(plateTypeId?: number): void {
    this.Translations = [];
    if (!plateTypeId) {
      this.plateType = new CreateOrEditPlateTypeDto();
      this.plateType.id = plateTypeId;
      this.PopulateTranslations([]);
      this.active = true;
      this.modal.show();
    } else {
      this._plateTypesServiceProxy.getPlateTypeForEdit(plateTypeId).subscribe((result) => {
        this.plateType = result.plateType;
        this.PopulateTranslations(this.plateType.translations);
        this.active = true;
        this.modal.show();
      });
    }
  }

  private PopulateTranslations(Translations: PlateTypeTranslationDto[]) {
    this.languages.forEach((r) => {
      let translation = new PlateTypeTranslationDto();
      translation.languageDisplayName = r.displayName;
      translation.language = r.name;
      translation.icon = r.icon;
      translation.displayName = _.find(Translations, (t) => t.language == r.name)?.displayName;
      this.Translations.push(translation);
    });
  }

  save(): void {
    this.saving = true;
    this.plateType.translations = this.Translations;
    this._plateTypesServiceProxy
      .createOrEdit(this.plateType)
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
