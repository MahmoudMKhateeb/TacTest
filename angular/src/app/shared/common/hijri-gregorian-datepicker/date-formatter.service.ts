import { Injectable } from '@angular/core';
import { NgbDateStruct, NgbDateParserFormatter, NgbDate } from '@ng-bootstrap/ng-bootstrap';

// import * as momentjs from 'moment';
// const moment = momentjs;
import * as moment from 'moment';

import * as moment_ from 'moment-hijri';
import * as _ from 'lodash';
const momentHijri = moment_;

@Injectable({
  providedIn: 'root',
})
export class DateFormatterService {
  constructor(private parserFormatter: NgbDateParserFormatter) {}

  ToString(date: NgbDateStruct): string {
    const dateStr = this.parserFormatter.format(date);
    return dateStr;
  }

  ToHijriDateStruct(hijriDate: string, format: string): NgbDate {
    const hijriMomentDate = momentHijri(hijriDate, format); // Parse a Hijri date based on format.

    const day = hijriMomentDate.iDate();
    const month = +hijriMomentDate.iMonth() + 1;
    const year = hijriMomentDate.iYear();

    const ngDate = new NgbDate(+year, month, +day);
    return ngDate;
  }

  ToGregorianDateStruct(gregorianDate: string, format: string): NgbDate {
    const momentDate = moment(gregorianDate, format); // Parse a Gregorian date based on format.

    const day = momentDate.date();
    const month = +momentDate.month() + 1;
    const year = momentDate.year();

    const ngDate = new NgbDate(+year, +month, +day);
    return ngDate;
  }

  ToHijri(date: NgbDateStruct): NgbDateStruct {
    if (!date) {
      return null;
    }
    const dateStr = this.ToString(date);
    const day = momentHijri(dateStr, 'D/M/YYYY').iDate();
    const month = momentHijri(dateStr, 'D/M/YYYY').iMonth() + 1;
    const year = momentHijri(dateStr, 'D/M/YYYY').iYear();
    const ngDate = new NgbDate(+year, +month, +day);
    return ngDate;
  }

  ToGregorian(date: NgbDateStruct) {
    if (!date) {
      return null;
    }

    const dateStr = this.ToString(date);

    const day = momentHijri(dateStr, 'iD/iM/iYYYY').format('D');
    const month = momentHijri(dateStr, 'iD/iM/iYYYY').format('M');
    const year = momentHijri(dateStr, 'iD/iM/iYYYY').format('Y');
    const ngDate = new NgbDate(+year, +month, +day);
    return ngDate;
  }

  GetTodayHijri() {
    const todayHijri = momentHijri().locale('en').format('iYYYY/iM/iD');
    const TodayDate = this.ToHijriDateStruct(todayHijri, 'iYYYY/iM/iD');
    return TodayDate.year + '-' + TodayDate.month + '-' + TodayDate.day;
  }

  GetTodayGregorian(): NgbDateStruct {
    const todayGregorian = moment().locale('en').format('YYYY/M/D');
    return this.ToGregorianDateStruct(todayGregorian, 'YYYY/M/D');
  }

  MomentToNgbDateStruct(value: moment.Moment): NgbDateStruct {
    return value && moment.isMoment(value)
      ? {
          day: value.date(),
          month: value.month() + 1,
          year: value.year(),
        }
      : null;
  }
  NgbDateStructToMoment(date: NgbDateStruct): moment.Moment {
    return date && _.isInteger(date.year) && _.isInteger(date.month) && _.isInteger(date.day)
      ? moment({
          year: date.year,
          month: date.month - 1,
          date: date.day,
          hour: 12,
        })
      : null;
  }
}
