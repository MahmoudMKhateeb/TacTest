import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';
import {
  ShippingRequestReasonAccidentServiceProxy,
  CreateOrEditShippingRequestReasonAccidentDto,
  ShippingRequestReasonAccidentTranslationDto,
} from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';

@Component({
  selector: 'accident-reason-modal',
  templateUrl: './create-or-edit-reason-modal.component.html',
})
export class AccidentReasonComponentModalComponent extends AppComponentBase implements OnInit {
  @Output() modalSave: EventEmitter<any> = new EventEmitter<any>();

  @ViewChild('modal', { static: false }) modal: ModalDirective;
  @ViewChild('inputName', { static: false }) nameInput: ElementRef;

  reason: CreateOrEditShippingRequestReasonAccidentDto;
  languages: abp.localization.ILanguageInfo[];
  Translations: ShippingRequestReasonAccidentTranslationDto[];
  active: boolean = false;
  saving: boolean = false;

  Specifiedtime: Date = new Date();
  constructor(injector: Injector, private _Service: ShippingRequestReasonAccidentServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.languages = _.filter(this.localization.languages, (l) => l.isDisabled === false);
  }

  public show(id: number | null): void {
    this.Translations = [];
    if (id == null) {
      this.reason = new CreateOrEditShippingRequestReasonAccidentDto();
      this.PopulateTranslations([]);
      this.active = true;
      this.modal.show();
    } else {
      this._Service.getForEdit(id).subscribe((result) => {
        this.reason = result;
        this.PopulateTranslations(this.reason.translations);
        this.active = true;
        this.modal.show();
      });
    }
  }
  private PopulateTranslations(Translations: ShippingRequestReasonAccidentTranslationDto[]) {
    this.languages.forEach((r) => {
      let translation = new ShippingRequestReasonAccidentTranslationDto();
      translation.language = r.name;
      translation.displayName = r.displayName;
      translation.icon = r.icon;
      let lang = _.find(Translations, (t) => t.language == r.name);
      if (lang != null) translation.name = lang.name;
      this.Translations.push(translation);
    });
  }
  onShown(): void {
    this.nameInput.nativeElement.focus();
  }

  save(): void {
    this.saving = true;
    this.reason.translations = this.Translations;
    this._Service
      .createOrEdit(this.reason)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.close();
        this.modalSave.emit(this.reason);
      });
  }

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
