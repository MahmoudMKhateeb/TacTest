import { Component, Injector, OnInit, ViewEncapsulation, NgZone, Input } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { NotificationServiceProxy, UserNotification } from '@shared/service-proxies/service-proxies';
import { IFormattedUserNotification, UserNotificationHelper } from './UserNotificationHelper';
import * as _ from 'lodash';
import { UrlHelper } from '@shared/helpers/UrlHelper';
import { Router } from '@angular/router';

@Component({
  templateUrl: './header-notifications.component.html',
  selector: 'header-notifications',
  styleUrls: ['header-notifications.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class HeaderNotificationsComponent extends AppComponentBase implements OnInit {
  notifications: IFormattedUserNotification[] = [];
  unreadNotificationCount = 0;
  @Input() isDropup = false;
  @Input() customStyle = 'btn btn-icon btn-dropdown btn-clean btn-lg mr-1';

  constructor(
    injector: Injector,
    private _notificationService: NotificationServiceProxy,
    private _userNotificationHelper: UserNotificationHelper,
    public _zone: NgZone,
    private _router: Router
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadNotifications();
    this.registerToEvents();
  }

  loadNotifications(): void {
    if (UrlHelper.isInstallUrl(location.href)) {
      return;
    }

    this._notificationService.getUserNotifications(undefined, undefined, undefined, 3, 0).subscribe((result) => {
      this.unreadNotificationCount = result.unreadCount;
      this.notifications = [];
      _.forEach(result.items, (item: UserNotification) => {
        this.notifications.push(this._userNotificationHelper.format(<any>item));
      });
    });
  }

  registerToEvents() {
    let self = this;

    function onNotificationReceived(userNotification) {
      self._userNotificationHelper.show(userNotification);
      self.loadNotifications();
    }

    abp.event.on('abp.notifications.received', (userNotification) => {
      self._zone.run(() => {
        onNotificationReceived(userNotification);
      });
    });

    function onNotificationsRefresh() {
      self.loadNotifications();
    }

    abp.event.on('app.notifications.refresh', () => {
      self._zone.run(() => {
        onNotificationsRefresh();
      });
    });

    function onNotificationsRead(userNotificationId) {
      let notification = _.findIndex(self.notifications, (n) => n.userNotificationId == userNotificationId);
      self.notifications[notification].state = 'READ';
      self.notifications[notification].isUnread = false;

      self.unreadNotificationCount -= 1;
    }

    abp.event.on('app.notifications.read', (userNotificationId) => {
      self._zone.run(() => {
        onNotificationsRead(userNotificationId);
      });
    });
  }

  setAllNotificationsAsRead(): void {
    this._userNotificationHelper.setAllAsRead();
  }

  openNotificationSettingsModal(): void {
    this._userNotificationHelper.openSettingsModal();
  }

  setNotificationAsRead(userNotification: IFormattedUserNotification): void {
    this._userNotificationHelper.setAsRead(userNotification.userNotificationId);
  }

  gotoUrl(url): void {
    if (url) {
      location.href = url;
    }
  }
}
