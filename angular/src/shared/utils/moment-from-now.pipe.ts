import { Inject, LOCALE_ID, Pipe, PipeTransform } from '@angular/core';

import * as moment from '@node_modules/moment';
import * as moment_ from 'moment-hijri';

@Pipe({ name: 'momentFromNow' })
export class MomentFromNowPipe implements PipeTransform {
  constructor(@Inject(LOCALE_ID) public locale: string) {}
  transform(value: moment_.MomentInput) {
    if (!value) {
      return '';
    }
    //checking for locale if 'ar' use arabic formatter
    if (this.locale === 'ar') {
      return moment(moment_(moment_(value)._i, 'iYYYY-iMM-iDD HH:mm:ss').format('YYYY-MM-DD HH:mm:ss').toString()).fromNow();
    }
    return moment(value).fromNow();
  }
}
