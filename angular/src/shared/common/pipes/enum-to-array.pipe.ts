import { Pipe, Injector, PipeTransform } from '@angular/core';
import { LocalizationService } from 'abp-ng2-module';
import { AppConsts } from '@shared/AppConsts';

@Pipe({
  name: 'enumToArray',
})
export class EnumToArrayPipe implements PipeTransform {
  localizationSourceName = AppConsts.localization.defaultLocalizationSourceName;
  localization: LocalizationService;

  constructor(injector: Injector) {
    this.localization = injector.get(LocalizationService);
  }
  transform(value): any {
    let keys = [];
    for (var enumMember in value) {
      if (!isNaN(parseInt(enumMember, 10))) {
        keys.push({ key: enumMember, value: this.localization.localize(value[enumMember], this.localizationSourceName) });
        // Uncomment if you want log
        // console.log("enum member: ", value[enumMember]);
      }
    }
    return keys;
  }
}
