import { Injectable } from '@angular/core';
import { Router, Resolve, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { InvoiceServiceProxy, InvoiceInfoDto } from '@shared/service-proxies/service-proxies';
import { Observable, of, EMPTY } from 'rxjs';
import { mergeMap, take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class InvoiceDetailResolverService implements Resolve<InvoiceInfoDto> {
  Result: any;
  constructor(private InvoiceServ: InvoiceServiceProxy, private router: Router) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<InvoiceInfoDto> | Observable<never> {
    let Router = route.paramMap.get('router');
    let id = route.paramMap.get('id');
    return this.InvoiceServ.getById(parseInt(id)).pipe(
      take(1),
      mergeMap((res) => {
        if (res) {
          return of(res);
        } else {
          // id not found
          this.router.navigate([`/`]);
          return EMPTY;
        }
      })
    );
  }
}
