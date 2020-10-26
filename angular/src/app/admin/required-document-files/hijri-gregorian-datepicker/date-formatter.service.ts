import { Injectable } from '@angular/core';
import { NgbDateStruct, NgbDateParserFormatter, NgbDate } from '@ng-bootstrap/ng-bootstrap';

import * as momentjs from 'moment';
const moment = momentjs;

import * as moment_ from 'moment-hijri';
const momentHijri = moment_;

@Injectable()
export class DateFormatterService {
  private InComingGeoFormat: any;
  private InComingHijriFormat: any;

  constructor(private parserFormatter: NgbDateParserFormatter) {}
  SetFormat(InComingGeoFormat: string, InComingHijriFormat: string) {
    this.InComingGeoFormat = InComingGeoFormat;
    this.InComingHijriFormat = InComingHijriFormat;
  }

  ToString(date: NgbDateStruct): string {
    const dateStr = this.parserFormatter.format(date);
    return dateStr;
  }

  ToHijri(date: NgbDateStruct): NgbDateStruct {
    if (!date) {
      return null;
    }
    const dateStr = this.ToString(date);
    const day = momentHijri(dateStr, this.InComingGeoFormat).iDate();
    const month = momentHijri(dateStr, this.InComingGeoFormat).iMonth() + 1;
    const year = momentHijri(dateStr, this.InComingGeoFormat).iYear();
    const ngDate = new NgbDate(+year, +month, +day);
    return ngDate;
  }

  ToGregorian(date: NgbDateStruct) {
    if (!date) {
      return null;
    }
    const dateStr = this.ToString(date);
    const day = momentHijri(dateStr, this.InComingHijriFormat).format('DD');
    const month = momentHijri(dateStr, this.InComingHijriFormat).format('MM');
    const year = momentHijri(dateStr, this.InComingHijriFormat).format('YYYY');
    const ngDate = new NgbDate(+year, +month, +day);
    return ngDate;
  }
}
