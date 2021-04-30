import { Injector, Pipe, PipeTransform } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { LocalizationService } from 'abp-ng2-module';
import { TerminologieServiceProxy } from 'shared/service-proxies/terminologies-ervice-proxy';
const capitalize = (s) => {
  if (typeof s !== 'string') return '';
  return s.charAt(0).toUpperCase() + s.slice(1);
};
@Pipe({
  name: 'localize',
})
export class LocalizePipe implements PipeTransform {
  localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;

  localization: LocalizationService;

  constructor(injector: Injector, private terminologieServiceProxy: TerminologieServiceProxy) {
    this.localization = injector.get(LocalizationService);
  }

  l(key: string, ...args: any[]): string {
    key = capitalize(key);
    args.unshift(key);
    args.unshift(this.localizationSourceName);
    this.terminologieServiceProxy.Add(key);
    return this.ls.apply(this, args);
  }

  ls(sourcename: string, key: string, ...args: any[]): string {
    let localizedText = this.localization.localize(key, sourcename);

    if (!localizedText) {
      localizedText = key;
    }

    if (!args || !args.length) {
      return localizedText;
    }

    args.unshift(localizedText);
    return abp.utils.formatString.apply(this, this.flattenDeep(args));
  }

  transform(key: string, ...args: any[]): string {
    return this.l(key, args);
  }

  flattenDeep(array) {
    return array.reduce((acc, val) => (Array.isArray(val) ? acc.concat(this.flattenDeep(val)) : acc.concat(val)), []);
  }
}
