import { Component, Injector, Input, OnInit } from '@angular/core';
import CustomStore from '@node_modules/devextreme/data/custom_store';
import { LoadOptions } from '@node_modules/devextreme/data/load_options';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ActorSubmitInvoiceServiceProxy, SubmitInvoiceInfoDto } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-actor-submit-invoice-item-detail',
  templateUrl: './actor-submit-invoice-item-detail.component.html',
  styleUrls: ['./actor-submit-invoice-item-detail.component.css'],
})
export class ActorSubmitInvoiceItemDetailComponent extends AppComponentBase implements OnInit {
  actorInvoiceItemsDataSource: any;
  @Input() actorSubmitInvoiceId: number;
  actorSubmitInvoiceInfo: SubmitInvoiceInfoDto;

  constructor(private _injector: Injector, private _actorSubmitInvoiceServiceProxy: ActorSubmitInvoiceServiceProxy) {
    super(_injector);
  }

  ngOnInit(): void {
    this.actorSubmitInvoiceInfo = new SubmitInvoiceInfoDto();

    let self = this;
    this.actorInvoiceItemsDataSource = {};
    this.actorInvoiceItemsDataSource.store = new CustomStore({
      load(loadOptions: LoadOptions) {
        console.log(JSON.stringify(loadOptions));
        return self._actorSubmitInvoiceServiceProxy
          .getActorSubmitInvoiceDetails(self.actorSubmitInvoiceId)
          .toPromise()
          .then((response) => {
            self.actorSubmitInvoiceInfo = response;
            return {
              data: response.items,
              totalAmount: response.totalAmount,
            };
          })
          .catch((error) => {
            console.log(error);
            throw new Error('Data Loading Error');
          });
      },
    });
  }
}
