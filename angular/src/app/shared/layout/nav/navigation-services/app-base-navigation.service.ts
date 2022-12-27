import { AppSessionService } from '@shared/common/session/app-session.service';
import { Injectable } from '@angular/core';
import { AppMenu } from '../app-menu';
import { AppMenuItem } from '../app-menu-item';
import { FeatureCheckerService } from '@node_modules/abp-ng2-module';
import { PermissionCheckerService } from 'abp-ng2-module';
import { isNotNullOrUndefined } from '@node_modules/codelyzer/util/isNotNullOrUndefined';

@Injectable({ providedIn: 'root' })
export class AppBaseNavigationService {
  constructor(
    private permissionCheckerService: PermissionCheckerService,
    private appSessionService: AppSessionService,
    private featureCheckerService: FeatureCheckerService
  ) {}

  getMenu(): AppMenu {
    let menu = new AppMenu('', '', []);
    return menu;
  }

  checkChildMenuItemPermission(menuItem): boolean {
    for (let i = 0; i < menuItem.items.length; i++) {
      let subMenuItem = menuItem.items[i];

      if (subMenuItem.permissionName === '' || subMenuItem.permissionName === null) {
        if (subMenuItem.route) {
          return true;
        }
      } else if (this.permissionCheckerService.isGranted(subMenuItem.permissionName)) {
        return true;
      }

      if (subMenuItem.items && subMenuItem.items.length) {
        let isAnyChildItemActive = this.checkChildMenuItemPermission(subMenuItem);
        if (isAnyChildItemActive) {
          return true;
        }
      }
    }
    return false;
  }

  showMenuItem(menuItem: AppMenuItem): boolean {
    if (
      menuItem.permissionName === 'Pages.Administration.Tenant.SubscriptionManagement' &&
      this.appSessionService.tenant &&
      !this.appSessionService.tenant.edition
    ) {
      return false;
    }

    let hideMenuItem = false;

    if (menuItem.requiresAuthentication && !this.appSessionService.user) {
      hideMenuItem = true;
    }

    if (menuItem.permissionName && !this.permissionCheckerService.isGranted(menuItem.permissionName)) {
      hideMenuItem = true;
    }

    if (this.appSessionService.tenant || !abp.multiTenancy.ignoreFeatureCheckForHostUsers) {
      if (menuItem.hasFeatureDependency() && !menuItem.featureDependencySatisfied()) {
        hideMenuItem = true;
      }
    }

    if (!hideMenuItem && menuItem.items && menuItem.items.length) {
      return this.checkChildMenuItemPermission(menuItem);
    }

    return !hideMenuItem;
  }

  /**
   * Returns all menu items recursively
   */
  getAllMenuItems(): AppMenuItem[] {
    let menu = this.getMenu();

    let allMenuItems: AppMenuItem[] = [];
    menu.items.forEach((menuItem) => {
      allMenuItems = allMenuItems.concat(this.getAllMenuItemsRecursive(menuItem));
    });

    return allMenuItems;
  }

  private getAllMenuItemsRecursive(menuItem: AppMenuItem): AppMenuItem[] {
    if (!menuItem.items) {
      return [menuItem];
    }

    let menuItems = [menuItem];
    menuItem.items.forEach((subMenu) => {
      menuItems = menuItems.concat(this.getAllMenuItemsRecursive(subMenu));
    });

    return menuItems;
  }

  isEnabled(featureName: string) {
    return this.featureCheckerService.isEnabled(featureName);
  }
}
