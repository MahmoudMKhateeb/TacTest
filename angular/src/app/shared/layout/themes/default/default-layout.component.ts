import { Injector, Component, OnInit, Inject } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { ThemesLayoutBaseComponent } from '@app/shared/layout/themes/themes-layout-base.component';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { DOCUMENT } from '@angular/common';
import { OffcanvasOptions } from '@metronic/app/core/_base/layout/directives/offcanvas.directive';
import { AppConsts } from '@shared/AppConsts';
import { ToggleOptions } from '@metronic/app/core/_base/layout/directives/toggle.directive';
import { Router } from '@angular/router';

@Component({
  templateUrl: './default-layout.component.html',
  selector: 'default-layout',
  animations: [appModuleAnimation()],
})
export class DefaultLayoutComponent extends ThemesLayoutBaseComponent implements OnInit {
  menuCanvasOptions: OffcanvasOptions = {
    baseClass: 'aside',
    overlay: true,
    closeBy: 'kt_aside_close_btn',
    toggleBy: 'kt_aside_mobile_toggle',
  };

  userMenuToggleOptions: ToggleOptions = {
    target: this.document.body,
    targetState: 'topbar-mobile-on',
    toggleState: 'active',
  };

  remoteServiceBaseUrl: string = AppConsts.remoteServiceBaseUrl;
  isDashboard = false;

  constructor(injector: Injector, @Inject(DOCUMENT) private document: Document, private router: Router) {
    super(injector);
  }

  ngOnInit() {
    this.installationMode = UrlHelper.isInstallUrl(location.href);
    this.isDashboard = location.href.toLowerCase().search('dashboard') > -1;
    this.router.events.subscribe((res) => {
      this.isDashboard = location.href.toLowerCase().search('dashboard') > -1;
    });
  }
}
