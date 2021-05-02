import { Component, ViewChild, Injector, Output, EventEmitter, OnInit, ElementRef } from '@angular/core';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/common/app-component-base';

import { AppLocalizationServiceProxy, AppLocalizationForViewDto, AppLocalizationTranslationDto } from '@shared/service-proxies/service-proxies';
import * as _ from 'lodash';

@Component({
  selector: 'view-applocalization-modal',
  templateUrl: './view-applocalization-modal.component.html',
})
export class ViewApplocalizationModalComponent extends AppComponentBase implements OnInit {
  @ViewChild('modal', { static: false }) modal: ModalDirective;
  Terminologie: AppLocalizationForViewDto;
  languages: abp.localization.ILanguageInfo[];
  Translations: AppLocalizationTranslationDto[];
  active: boolean = false;

  constructor(injector: Injector, private _Service: AppLocalizationServiceProxy) {
    super(injector);
  }
  ngOnInit(): void {
    this.Terminologie = new AppLocalizationForViewDto();
    this.languages = _.filter(this.localization.languages, (l) => l.isDisabled === false && l.displayName != 'English');
  }

  public show(id: number): void {
    this.Translations = [];
    this._Service.getForView(id).subscribe((result) => {
      this.Terminologie = result;
      this.PopulateTranslations(this.Terminologie.translations);
      this.active = true;
      this.modal.show();
    });
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

  close(): void {
    this.modal.hide();
    this.active = false;
  }
}
