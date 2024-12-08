import { NgModule } from '@angular/core';
import { NavigationEnd, RouteConfigLoadEnd, RouteConfigLoadStart, Router, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from './shared/common/auth/auth-route-guard';
import { NotificationsComponent } from './shared/layout/notifications/notifications.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { RequiredDocumentFilesComponent } from '@app/admin/required-document-files/required-document-files.component';
import { RequiredDocumentsGuard } from '@app/shared/common/required-documents/required-documents.guard';
import { NotRequiredDocumentFilesComponent } from '@app/admin/not-required-document-files/not-required-document-files.component';
import { RatingPageComponent } from '@app/rating-page/rating-page.component';

@NgModule({
  imports: [
    RouterModule.forChild([
      {
        path: 'app',
        component: AppComponent,
        canActivate: [AppRouteGuard],
        canActivateChild: [AppRouteGuard],
        children: [
          {
            path: '',
            children: [
              { path: 'notifications', component: NotificationsComponent },
              { path: '', redirectTo: '/app/main/dashboard', pathMatch: 'full' },
            ],
          },
          {
            path: 'main',
            loadChildren: () => import('app/main/main.module').then((m) => m.MainModule), //Lazy load main module
            data: { preload: true },
          },
          {
            path: 'admin',
            loadChildren: () => import('app/admin/admin.module').then((m) => m.AdminModule), //Lazy load admin module
            data: { preload: true },
            canLoad: [AppRouteGuard],
          },
          {
            path: '**',
            redirectTo: 'notifications',
          },
        ],
      },
      {
        path: 'tenantRequiredDocuments',
        component: AppComponent,
        canActivate: [RequiredDocumentsGuard],
        canActivateChild: [RequiredDocumentsGuard],
        canLoad: [RequiredDocumentsGuard],
        children: [{ path: 'tenantRequiredDocuments', component: RequiredDocumentFilesComponent, data: { permission: 'Pages.DocumentFiles' } }],
      },
      {
        path: 'tenantNotRequiredDocuments',
        component: AppComponent,
        canLoad: [AppRouteGuard],
        children: [{ path: 'tenantNotRequiredDocuments', component: NotRequiredDocumentFilesComponent, data: { permission: 'Pages.DocumentFiles' } }],
      },
    ]),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {
  constructor(private router: Router, private spinnerService: NgxSpinnerService) {
    router.events.subscribe((event) => {
      if (event instanceof RouteConfigLoadStart) {
        spinnerService.show();
      }

      if (event instanceof RouteConfigLoadEnd) {
        spinnerService.hide();
      }

      if (event instanceof NavigationEnd) {
        document.querySelector('meta[property=og\\:url').setAttribute('content', window.location.href);
      }
    });
  }
}
