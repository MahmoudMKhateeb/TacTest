import { mergeMap as _observableMergeMap, catchError as _observableCatch } from 'rxjs/operators';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
import { Injectable, Inject, Optional } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse, HttpResponseBase } from '@angular/common/http';
import { Router } from '@angular/router';
import { ApiException, API_BASE_URL } from './service-proxies';
import * as _ from 'lodash';

@Injectable()
export class TerminologieServiceProxy {
  private http: HttpClient;
  private baseUrl: string;
  private delay = 1;
  terminologie: Terminologie[] = [];
  protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

  constructor(@Inject(HttpClient) http: HttpClient, private router: Router, @Optional() @Inject(API_BASE_URL) baseUrl?: string) {
    this.http = http;
    this.baseUrl = baseUrl ? baseUrl : '';
  }

  async createOrEdit(key: string): Promise<any> {
    let url_ = this.baseUrl + '/api/services/app/AppLocalization/CreateOrUpdateKeyLog';
    url_ = url_.replace(/[?&]$/, '');

    const content_ = { key, pageurl: this.router.url };

    let options_: any = {
      body: content_,
      observe: 'response',
      responseType: 'blob',
      headers: new HttpHeaders({
        'Content-Type': 'application/json-patch+json',
      }),
    };
    this.delay += 250;
    await delay(this.delay);
    if (this.delay == 1000) this.delay = 1;
    return await this.http.request('post', url_, options_);
  }

  Add(key: string): void {
    return;
    if (key == '' || key == null || key === undefined) return;
    let PageUrl = this.router.url;
    if (_.findIndex(this.terminologie, (l) => l.Key === key && l.PageUrl === PageUrl) == -1) {
      var lang = new Terminologie();
      lang.Key = key;
      lang.PageUrl = PageUrl;
      this.terminologie.push(lang);
      //console.log(this.terminologie);
      this.createOrEdit(key).then((r) => r.subscribe());
    }
  }
}
function delay(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}
export class Terminologie {
  Key: string;
  PageUrl: string;
}
