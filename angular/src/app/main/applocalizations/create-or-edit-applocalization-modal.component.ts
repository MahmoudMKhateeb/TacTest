import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NgForm } from '@angular/forms';

import {
  AppLocalizationServiceProxy,
  CreateOrEditAppLocalizationDto,
  AppLocalizationTranslationDto,
  TerminologyVersion,
  TerminologyPlatForm,
  TerminologyAppVersion,
} from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';
import { EnumToArrayPipe } from '../../../shared/common/pipes/enum-to-array.pipe';

@Component({
  selector: 'applocalization-modal',
  templateUrl: './create-or-edit-applocalization-modal.component.html',
  providers: [EnumToArrayPipe],
})
export class ApplocalizationModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('nameInput', { static: false }) nameInput: ElementRef;
  @ViewChild('Form', { static: false }) Form: NgForm;

  AppLocalization: CreateOrEditAppLocalizationDto;
  languages: abp.localization.ILanguageInfo[];
  Translations: AppLocalizationTranslationDto[];
  active: boolean = false;
  saving: boolean = false;
  terminologyVersion: any;
  terminologyPlatForm: any;
  terminologyAppVersion: any;
  constructor(injector: Injector, private _Service: AppLocalizationServiceProxy, private enumToArray: EnumToArrayPipe) {
    super(injector);
  }
  ngOnInit(): void {
    this.languages = _.filter(this.localization.languages, (l) => l.isDisabled === false && l.displayName != 'English');
    this.terminologyVersion = this.enumToArray.transform(TerminologyVersion);
    this.terminologyPlatForm = this.enumToArray.transform(TerminologyPlatForm);
    this.terminologyAppVersion = this.enumToArray.transform(TerminologyAppVersion);
  }

  public show(id: number | null): void {
    this.Translations = [];
    if (id == null) {
      this.AppLocalization = new CreateOrEditAppLocalizationDto();
      this.PopulateTranslations([]);
      this.active = true;
      this.modal.show();
    } else {
      this._Service.getForEdit(id).subscribe((result) => {
        this.AppLocalization = result;
        this.PopulateTranslations(this.AppLocalization.translations);
        this.active = true;
        this.modal.show();
      });
    }
  }
  private PopulateTranslations(Translations: AppLocalizationTranslationDto[]) {
    this.languages.forEach((r) => {
      let translation = new AppLocalizationTranslationDto();
      translation.language = r.name;
      translation.displayName = r.displayName;
      translation.icon = r.icon;
      let lang = _.find(Translations, (t) => t.language == r.name);
      if (lang != null) translation.value = lang.value;
      this.Translations.push(translation);
    });
  }
  onShown(): void {
    this.nameInput.nativeElement.focus();
  }

  save(): void {
    this.saving = true;
    this.AppLocalization.translations = _.filter(this.Translations, (t) => t.value != null);
    this._Service
      .createOrEdit(this.AppLocalization)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(this.AppLocalization);
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
