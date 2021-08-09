import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import * as _ from 'lodash';

@Component({
  selector: 'language-switch',
  templateUrl: './language-switch.component.html',
  styles: ['.language-switch-btn { width: auto; height: auto; }'],
})
export class LanguageSwitchComponent extends AppComponentBase implements OnInit {
  currentLanguage: abp.localization.ILanguageInfo;
  languages: abp.localization.ILanguageInfo[] = [];

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit(): void {
    let languageallow: string[] = ['en', 'ar-EG'];
    this.languages = _.filter(this.localization.languages, (l) => l.isDisabled === false && languageallow.includes(l.name));
    this.currentLanguage = abp.localization.currentLanguage;
  }

  changeLanguage(language: abp.localization.ILanguageInfo) {
    abp.utils.setCookieValue(
      'Abp.Localization.CultureName',
      language.name,
      new Date(new Date().getTime() + 5 * 365 * 86400000), // 5 year
      abp.appPath
    );

    location.reload();
  }
}
