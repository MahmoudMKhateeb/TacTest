import { Injectable, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { EntityDtoOfGuid, NotificationServiceProxy, PriceOfferChannel } from '@shared/service-proxies/service-proxies';
import * as moment from 'moment';
import * as Push from 'push.js'; // if using ES6
import { NotificationSettingsModalComponent } from './notification-settings-modal.component';
import { AppConsts } from '@shared/AppConsts';

export interface IFormattedUserNotification {
  userNotificationId: string;
  text: string;
  time: string;
  creationTime: Date;
  icon: string;
  state: String;
  data: any;
  url: string;
  isUnread: boolean;
  severity: abp.notifications.severity;
  iconFontClass: string;
}

@Injectable()
export class UserNotificationHelper extends AppComponentBase {
  settingsModal: NotificationSettingsModalComponent;

  constructor(injector: Injector, private _notificationService: NotificationServiceProxy) {
    super(injector);
  }

  getUrl(userNotification: abp.notifications.IUserNotification): string {
    switch (userNotification.notification.notificationName) {
      case 'App.NewUserRegistered':
        return '/app/admin/users?filterText=' + userNotification.notification.data.properties.emailAddress;
      case 'App.NewTenantRegistered':
        return '/app/admin/tenants?filterText=' + userNotification.notification.data.properties.tenancyName;
      case 'App.GdprDataPrepared':
        return (
          AppConsts.remoteServiceBaseUrl +
          '/File/DownloadBinaryFile?id=' +
          userNotification.notification.data.properties.binaryObjectId +
          '&contentType=application/zip&fileName=collectedData.zip'
        );
      case 'App.DownloadInvalidImportUsers':
        return (
          AppConsts.remoteServiceBaseUrl +
          '/File/DownloadTempFile?fileToken=' +
          userNotification.notification.data.properties.fileToken +
          '&fileType=' +
          userNotification.notification.data.properties.fileType +
          '&fileName=' +
          userNotification.notification.data.properties.fileName
        );
      //Add your custom notification names to navigate to a URL when user clicks to a notification.
      // Document managment module
      case 'App.RejectedSubmittedDocument':
        return '/app/main/documentFiles/documentFiles';

      case 'App.AcceptedSubmittedDocument':
        return '/app/main/documentFiles/documentFiles';

      // Shipping request module
      case 'App.ShippingRequestAccidentsOccure':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.TachyonDealOfferRejectedByShipper':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.SendDriectRequestForCarrier':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;

      case 'App.DriectRequestCarrierRespone':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShippingRequest.trip.Accident.Cancel':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.DriverRejectTrip':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;

      case 'App.DriverAcceptTrip':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.App.ShipperReminderToCompelteTrips':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.CancelShipment':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.DeclineDriectRequest':
        return `/app/main/directrequest/list?id=${userNotification.notification.data.properties.id}`;
      case 'App.InvoiceShipperGenerated':
        return `/app/main/invoices/detail/${userNotification.notification.data.properties.invoiceid}`;
      case 'App.PendingOffer':
        return `/app/main/offers/list?id=${userNotification.notification.data.properties.id}`;
      case 'App.RejectedOffer':
        return this.getRejectedOfferUrl(userNotification);
      case 'App.SendDriectRequest':
        return `/app/main/directrequest/list?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShipperAcceptedOffer':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShipperReminderToCompelteTrips':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShippingRequestNotifyCarrirerWhenShipperAccepted':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShippingRequestSendOfferWhenUpdatePrice':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}&offerid=${userNotification.notification.data.properties.offerId}`;
      case 'App.SubmitInvoiceGenerated':
        return `/app/main/invoices/submitinvoices/detail/${userNotification.notification.data.properties.id}`;
      case 'App.SubmitInvoiceOnAccepted':
        return `/app/main/invoices/submitinvoices/detail/${userNotification.notification.data.properties.id}`;
      case 'App.ShippingRequestAsBidWithSameTruck':
        return `/app/main/marketplace/list`;
      case 'App.NotificationWhenTripDetailsChanged':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShippingRequest.Accident':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.NotifyShipperWhenTripUpdated':
        return (
          `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}` +
          `&tripId=${userNotification.notification.data.properties.updatedTripId}`
        );
      //Redirect Invoice Notification
      case 'App.InvoiceShipperGenerated':
        return '/app/main/invoices/detail/' + userNotification.notification.data.properties.invoiceid;
      case 'App.SubmitInvoiceOnClaim':
        return '/app/main/invoices/detail/' + userNotification.notification.data.properties.invoiceid;
      case 'App.SubmitInvoiceOnRejected':
        return '/app/main/invoices/detail/' + userNotification.notification.data.properties.invoiceid;

      case 'App.TachyonDealOfferAcceptedByShipper':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;

      case 'App.TMSAcceptedOffer':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;

      case 'App.NotifyTachyonDealWhenTripUpdated':
        return (
          `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}` +
          `&tripId=${userNotification.notification.data.properties.updatedTripId}`
        );
      case 'App.NotifyCarrierWhenTripUpdated':
        return (
          `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}` +
          `&tripId=${userNotification.notification.data.properties.id}`
        );
      case 'App.NotifyShipperWhenTripUpdated':
        return (
          `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}` +
          `&tripId=${userNotification.notification.data.properties.updatedTripId}`
        );

      case 'App.NotifyCarrierWhenPostPriceSrUpdated':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}`;

      case 'App.NotifyShipperForPostPriceSrUpdateAction':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}`;

      case 'App.NotifyShipperWhenRequestChangePrice':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}`;

      case 'App.ShippingRequest.ShippingRequestTripCanceled':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShippingRequest.ShippingRequestTripRejectCancelByTachyonDealer':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.ShippingRequest.ShippingRequestTripNeedsCancelApproval':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.NotifyOfferOwnerWhenMarketplaceShippingRequestUpdated':
        return `/app/main/marketplace/list?srId=${userNotification.notification.data.properties.srId}`;
      case 'App.NotifyOfferOwnerWhenDirectRequestShippingRequestUpdated':
        return `/app/main/directrequest/list?srId=${userNotification.notification.data.properties.srId}`;

      //New Offer Redirection
      case 'App.ShippingRequestSendOfferWhenAddPrice':
        return (
          '/app/main/shippingRequests/shippingRequests/view?id=' +
          userNotification.notification.data.properties.id +
          '&offerid=' +
          userNotification.notification.data.properties.offerid
        );
      case 'App.PricePackageOfferWasCreated':
        return `/app/main/directrequest/list?directRequestId=${userNotification.notification.data.properties.directRequestId}`;
      case 'App.ShippingRequestAsBidWithSameTruck':
        return `/app/main/marketplace/list?srId=${userNotification.notification.data.properties.shippingRequestId}`;
      case 'App.Trip.Accident.Resolved':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.id}`;
      case 'App.RejectedPostPriceOffer':
        return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}`;
    }

    //No url for this notification
    return '';
  }

  /* PUBLIC functions ******************************************/

  getUiIconBySeverity(severity: abp.notifications.severity): string {
    switch (severity) {
      case abp.notifications.severity.SUCCESS:
        return 'fas fa-check-circle';
      case abp.notifications.severity.WARN:
        return 'fas fa-exclamation-triangle';
      case abp.notifications.severity.ERROR:
        return 'fas fa-exclamation-circle';
      case abp.notifications.severity.FATAL:
        return 'fas fa-bomb';
      case abp.notifications.severity.INFO:
      default:
        return 'fas fa-info-circle';
    }
  }

  getIconFontClassBySeverity(severity: abp.notifications.severity): string {
    switch (severity) {
      case abp.notifications.severity.SUCCESS:
        return ' text-success';
      case abp.notifications.severity.WARN:
        return ' text-warning';
      case abp.notifications.severity.ERROR:
        return ' text-danger';
      case abp.notifications.severity.FATAL:
        return ' text-danger';
      case abp.notifications.severity.INFO:
      default:
        return ' text-info';
    }
  }

  format(userNotification: abp.notifications.IUserNotification, truncateText?: boolean): IFormattedUserNotification {
    let formatted: IFormattedUserNotification = {
      userNotificationId: userNotification.id,
      text: abp.notifications.getFormattedMessageFromUserNotification(userNotification),
      time: moment(userNotification.notification.creationTime).format('YYYY-MM-DD HH:mm:ss'),
      creationTime: userNotification.notification.creationTime,
      icon: this.getUiIconBySeverity(userNotification.notification.severity),
      state: abp.notifications.getUserNotificationStateAsString(userNotification.state),
      data: userNotification.notification.data,
      url: this.getUrl(userNotification),
      isUnread: userNotification.state === abp.notifications.userNotificationState.UNREAD,
      severity: userNotification.notification.severity,
      iconFontClass: this.getIconFontClassBySeverity(userNotification.notification.severity),
    };

    if (truncateText || truncateText === undefined) {
      formatted.text = abp.utils.truncateStringWithPostfix(formatted.text, 100);
    }

    return formatted;
  }

  show(userNotification: abp.notifications.IUserNotification): void {
    let url = this.getUrl(userNotification);
    //Application notification
    abp.notifications.showUiNotifyForUserNotification(userNotification, {
      onOpen: (toast) => {
        toast.addEventListener('click', () => {
          //Take action when user clicks to live toastr notification
          if (url) {
            location.href = url;
          }
        });
      },
    });
    if (Push.default.Permission.has()) {
      //Desktop notification
      Push.default.create('TACHYON', {
        body: this.format(userNotification).text,
        icon: abp.appPath + 'assets/common/images/app-logo-on-dark-sm.svg',
        timeout: 6000,
        onClick: function () {
          window.focus();
          this.close();
        },
      });
    }
  }

  setAllAsRead(callback?: () => void): void {
    this._notificationService.setAllNotificationsAsRead().subscribe(() => {
      abp.event.trigger('app.notifications.refresh');
      if (callback) {
        callback();
      }
    });
  }

  setAsRead(userNotificationId: string, callback?: (userNotificationId: string) => void): void {
    const input = new EntityDtoOfGuid();
    input.id = userNotificationId;
    this._notificationService.setNotificationAsRead(input).subscribe(() => {
      abp.event.trigger('app.notifications.read', userNotificationId);
      if (callback) {
        callback(userNotificationId);
      }
    });
  }

  openSettingsModal(): void {
    this.settingsModal.show();
  }

  getRejectedOfferUrl(userNotification): string {
    switch (userNotification.notification.data.properties.channel) {
      case PriceOfferChannel.MarketPlace:
        return `/app/main/marketplace/list?srId=${userNotification.notification.data.properties.srId}`;
      case PriceOfferChannel.DirectRequest:
        return `app/main/directrequest/list?srId=${userNotification.notification.data.properties.srId}`;
      case PriceOfferChannel.TachyonManageService:
        if (this.isTachyonDealer) {
          return `/app/main/shippingRequests/shippingRequests/view?id=${userNotification.notification.data.properties.srId}`;
        }
        return `/app/main/marketplace/list?srId=${userNotification.notification.data.properties.srId}`;
    }

    return '';
  }
}
