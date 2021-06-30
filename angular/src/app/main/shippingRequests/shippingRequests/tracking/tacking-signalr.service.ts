import { Injectable, Injector, NgZone } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { HubConnection } from '@microsoft/signalr';
import { TrackingListDto } from '@shared/service-proxies/service-proxies';

@Injectable()
export class TrackingSignalrService extends AppComponentBase {
  constructor(injector: Injector, public _zone: NgZone) {
    super(injector);
  }

  Hub: HubConnection;
  isConnected = false;

  configureConnection(connection): void {
    // Set the common hub
    this.Hub = connection;

    // Reconnect loop
    let reconnectTime = 5000;
    let tries = 1;
    let maxTries = 8;
    function start() {
      return new Promise(function (resolve, reject) {
        if (tries > maxTries) {
          reject();
        } else {
          connection
            .start()
            .then(resolve)
            .then(() => {
              reconnectTime = 5000;
              tries = 1;
            })
            .catch(() => {
              setTimeout(() => {
                start().then(resolve);
              }, reconnectTime);
              reconnectTime *= 2;
              tries += 1;
            });
        }
      });
    }

    // Reconnect if hub disconnects
    connection.onclose((e) => {
      this.isConnected = false;

      if (e) {
        abp.log.debug('connection closed with error: ' + e);
      } else {
        abp.log.debug('disconnected');
      }

      start().then(() => {
        this.isConnected = true;
      });
    });

    // Register to get notifications
    this.registerChatEvents(connection);
  }

  registerChatEvents(connection): void {
    connection.on('tracking', (trigger, data) => {
      console.log(trigger);
      abp.event.trigger(trigger, {
        data: data,
      });
    });
  }

  sendMessage(messageData, callback): void {
    if (!this.isConnected) {
      if (callback) {
        callback();
      }

      abp.notify.warn(this.l('IsNotConnectedWarning'));
      return;
    }

    this.Hub.invoke('sendMessage', messageData)
      .then((result) => {
        if (result) {
          abp.notify.warn(result);
        }

        if (callback) {
          callback();
        }
      })
      .catch((error) => {
        abp.log.error(error);

        if (callback) {
          callback();
        }
      });
  }

  init(): void {
    this._zone.runOutsideAngular(() => {
      abp.signalr.connect();
      abp.signalr
        .startConnection(abp.appPath + 'signalr', (connection) => {
          this.configureConnection(connection);
        })
        .then(() => {
          abp.event.trigger('app.chat.connected');
          this.isConnected = true;
        });
    });
  }
}
