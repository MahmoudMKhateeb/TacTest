import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router, CanActivateChild, CanLoad } from '@angular/router';
import { Observable } from 'rxjs';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { of, Subject } from '@node_modules/rxjs';
import { PermissionCheckerService, RefreshTokenService } from '@node_modules/abp-ng2-module';
import { AppSessionService } from '@shared/common/session/app-session.service';

@Injectable({
  providedIn: 'root',
})
export class RequiredDocumentsGuard implements CanActivate, CanActivateChild, CanLoad {
  constructor(
    private _permissionChecker: PermissionCheckerService,
    private _router: Router,
    private _sessionService: AppSessionService,
    private _refreshTokenService: RefreshTokenService
  ) {}

  canActivateInternal(data: any, state: RouterStateSnapshot): Observable<boolean> {
    console.log('canActivateInternal data', data['permission']);
    if (UrlHelper.isInstallUrl(location.href)) {
      return of(true);
    }

    if (!this._sessionService.user) {
      let sessionObservable = new Subject<any>();

      this._refreshTokenService.tryAuthWithRefreshToken().subscribe(
        (autResult: boolean) => {
          if (autResult) {
            sessionObservable.next(true);
            sessionObservable.complete();
            location.reload();
          } else {
            sessionObservable.next(false);
            sessionObservable.complete();
            this._router.navigate(['/account/login']);
          }
        },
        (error) => {
          sessionObservable.next(false);
          sessionObservable.complete();
          this._router.navigate(['/account/login']);
        }
      );
      return sessionObservable;
    }

    if (!data || !data['permission']) {
      return of(true);
    }

    if (this._permissionChecker.isGranted(data['permission'])) {
      return of(true);
    }

    this._router.navigate([this.selectBestRoute()]);
    return of(false);
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.canActivateInternal(route.data, state);
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
    return this.canActivate(route, state);
  }

  canLoad(route: any): Observable<boolean> | Promise<boolean> | boolean {
    return this.canActivateInternal(route.data, null);
  }

  selectBestRoute(): string {
    if (!this._sessionService.user) {
      return '/account/login';
    }

    if (this._permissionChecker.isGranted('Pages.Administration.Host.Dashboard')) {
      return '/app/admin/hostDashboard';
    }

    if (this._permissionChecker.isGranted('Pages.Tenant.Dashboard')) {
      return '/app/main/dashboard';
    }

    if (this._permissionChecker.isGranted('Pages.Tenants')) {
      return '/app/admin/tenants';
    }

    if (this._permissionChecker.isGranted('Pages.Administration.Users')) {
      return '/app/admin/users';
    }

    return '/app/notifications';
  }
}
